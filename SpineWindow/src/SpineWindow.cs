using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpineWindow
{
    public enum SpineWindowType
    {
        AzurLaneSD = 0,
        AzurLaneDynamic = 1,
        ArknightsDynamic = 2,
    }

    public enum BackgroudColor
    {
        Black = 0,
        White = 1,
        Gray = 2
    }

    public abstract class SpineWindow: IDisposable
    {
        /// <summary>
        /// 创建指定类型 Spine 窗口
        /// </summary>
        /// <param name="type">窗口类型</param>
        /// <param name="slotCount">可供加载的 Spine 最大数量</param>
        /// <returns></returns>
        public static SpineWindow New(SpineWindowType type, uint slotCount = 10)
        {
            return type switch
            {
                SpineWindowType.AzurLaneSD => new AzurLaneSD(slotCount),
                SpineWindowType.AzurLaneDynamic => new AzurLaneDynamic(slotCount),
                SpineWindowType.ArknightsDynamic => new ArknightsDynamic(slotCount),
                _ => new AzurLaneSD(slotCount),
            };
        }

        /// <summary>
        /// 窗口类型
        /// </summary>
        public SpineWindowType Type 
        { 
            get
            {
                var t = GetType();
                if (t == typeof(AzurLaneSD))
                    return SpineWindowType.AzurLaneSD;
                if (t == typeof(AzurLaneDynamic))
                    return SpineWindowType.AzurLaneDynamic;
                if (t == typeof(ArknightsDynamic))
                    return SpineWindowType.ArknightsDynamic;
                throw new InvalidOperationException($"Unknown SpineWindow type {this}");
            }
        }

        /// <summary>
        /// SpineWindow 基类, 提供 Spine 装载和动画交互
        /// </summary>
        /// <param name="slotCount">最大 Spine 装载数量</param>
        public SpineWindow(uint slotCount)
        {
            spineSlots = new Spine.Spine[slotCount];
            colorTables = new Dictionary<uint, uint>[slotCount];
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();
        }

        ~SpineWindow()
        {
            if (window is not null)
                Dispose();
        }

        public void Dispose()
        {
            if (window is not null)
            {
                cancelTokenSrc.Cancel();
                windowLoopTask.Wait();
                window = null;
                windowLoopTask = null;
            }
        }

        /// <summary>
        /// Spine 对象装载数组
        /// </summary>
        protected Spine.Spine?[] spineSlots;

        /// <summary>
        /// 窗口可用最大 Spine 装载数
        /// </summary>
        public int SlotCount { get => spineSlots.Length; }

        private Dictionary<uint, uint>[] colorTables;

        /// <summary>
        /// 资源文件夹, 提供语音等资源的位置
        /// </summary>
        public string? ResFolder 
        { 
            get
            {
                string? v = null;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = Path.GetDirectoryName(Path.GetFullPath(sp.SkelPath)); break; } }
                mutex.ReleaseMutex();
                return v;
            }
        }

        /// <summary>
        /// 获取当前正在运行的 Spine 版本
        /// </summary>
        public string SpineVersion
        {
            get
            {
                var v = "3.8.x";
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.Version; break; } }
                mutex.ReleaseMutex();
                return v;
            }
        }

        /// <summary>
        /// 控制 Spine 是否水平翻转
        /// </summary>
        public bool SpineFlip 
        { 
            get
            {
                var v = false;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.FlipX; break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.FlipX = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 控制 Spine 缩放比例
        /// </summary>
        public float SpineScale
        {
            get
            {
                var v = 1f;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.Scale; break; } }
                mutex.ReleaseMutex(); 
                return v; 
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.Scale = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Spine 是否使用预乘 Alpha
        /// </summary>
        public bool SpineUsePMA
        {
            get
            {
                var v = false;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = sp.UsePremultipliedAlpha; break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.UsePremultipliedAlpha = value; } }
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Spine 位置
        /// </summary>
        public SFML.System.Vector2f SpinePosition
        {
            get
            {
                var v = SpinePositionReg;
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { v = new SFML.System.Vector2f(sp.X, sp.Y); break; } }
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                foreach (var sp in spineSlots) { if (sp is not null) { sp.X = value.X; sp.Y = value.Y; } }
                mutex.ReleaseMutex();
                SpinePositionReg = value;
            }
        }

        /// <summary>
        /// Spine 在注册表中存储的位置
        /// </summary>
        public SFML.System.Vector2f SpinePositionReg
        {
            get
            {
                SFML.System.Vector2f ret = new(0, 0);
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    { 
                        float.TryParse(spkey.GetValue("SpinePositionX", "0").ToString(), out ret.X);
                        float.TryParse(spkey.GetValue("SpinePositionY", "0").ToString(), out ret.Y);
                    }
                }
                return ret;
            }
            set
            {
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("SpinePositionX", value.X.ToString());
                        spkey.SetValue("SpinePositionY", value.Y.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 加载 Spine 到指定槽位
        /// </summary>
        /// <param name="version">版本字符串, 例如 "3.8.99"</param>
        /// <param name="skelPath">骨骼文件路径, 可以是 skel 或者 json 后缀</param>
        /// <param name="atlasPath">纹理文件路径, 后缀为 atlas</param>
        /// <param name="index">要加载到的槽位</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void LoadSpine(string version, string skelPath, string? atlasPath = null, int index = 0)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            Debug.WriteLine($"Loading spine[{index}]({version}) from {skelPath}, {atlasPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, atlasPath); }
            catch { throw; }

            var originalPosition = SpinePosition;
            spineNew.X = originalPosition.X;
            spineNew.Y = originalPosition.Y;

            mutex.WaitOne();
            spineSlots[index] = spineNew;
            mutex.ReleaseMutex();

            UpdateProperBackgroudColor(index);
            Trigger_SpineLoaded(index);
            Debug.Write("spine animiation: ");
            foreach (var a in spineSlots[index].AnimationNames) Debug.Write($"{a}; "); Debug.WriteLine("");
        }

        /// <summary>
        /// 获取指定槽位的 Spine 资源路径
        /// </summary>
        /// <param name="index">槽位索引</param>
        /// <returns>Skel 文件路径, 不存在则返回 null</returns>
        public string? GetSpineSkelPath(int index)
        {
            if (index >= spineSlots.Length) return null;
            mutex.WaitOne(); var v = spineSlots[index]?.SkelPath; mutex.ReleaseMutex(); return v;
        }

        /// <summary>
        /// 卸载执行槽位 Spine
        /// </summary>
        /// <param name="index">槽位</param>
        /// <exception cref="ArgumentOutOfRangeException">指定的 index 超出最大槽位限制</exception>
        public void UnloadSpine(int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            Debug.WriteLine($"Unload spine[{index}]");
            mutex.WaitOne();
            spineSlots[index] = null;
            mutex.ReleaseMutex();
        }

        private void UpdateProperBackgroudColor()
        {
            for (int i = 0; i < SlotCount; i++)
                UpdateProperBackgroudColor(i);
        }

        private void UpdateProperBackgroudColor(int index)
        {
            // TODO: 优化查找时间
            var colors = new Dictionary<uint, uint>();
            List<string> paths = [];

            mutex.WaitOne();
            var sp = spineSlots[index];
            if (sp is not null)
                paths.AddRange(sp.PngPaths);
            mutex.ReleaseMutex();

            if (paths.Count <= 0)
                return;

            foreach (var p in paths)
            {
                var png = new SFML.Graphics.Image(p);
                for (uint i = 0; i < png.Size.X; i++)
                {
                    for (uint j = 0; j < png.Size.Y; j++)
                    {
                        var c = png.GetPixel(i, j);
                        if (c.A <= 0) continue;
                        c.A = 0;
                        var k = c.ToInteger();
                        if (colors.ContainsKey(k)) colors[k] += 1;
                        else colors[k] = 1;
                    }
                }
            }

            colorTables[index] = colors;

            if (colors.Count <= 0)
                return;

            var rnd = new Random();
            var bestColor = SFML.Graphics.Color.Transparent;
            uint bestColorSameCount = uint.MaxValue;
            var tmpColor = SFML.Graphics.Color.Transparent;
            for (int i = 0; i < 10; i++)
            {
                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                switch (backgroudColor)
                {
                    case BackgroudColor.Black:
                        tmpColor.R = tmpColor.B = (byte)rnd.Next(0, 20);
                        tmpColor.G = (byte)rnd.Next(0, 20);
                        break;
                    case BackgroudColor.White:
                        tmpColor.R = tmpColor.B = (byte)rnd.Next(235, 255);
                        tmpColor.G = (byte)rnd.Next(235, 255);
                        break;
                    case BackgroudColor.Gray:
                        tmpColor.R = tmpColor.B = (byte)rnd.Next(118, 138);
                        tmpColor.G = (byte)rnd.Next(118, 138);
                        break;
                }

                var k = tmpColor.ToInteger();
                uint count = 0;
                uint tmp = 0;
                foreach (var table in colorTables)
                {
                    if (table is not null && table.TryGetValue(k, out tmp))
                        count += tmp;
                }
                if (count < bestColorSameCount)
                {
                    bestColor = tmpColor;
                    bestColorSameCount = count;
                }
                else
                {
                    bestColor = tmpColor;
                    bestColorSameCount = 0;
                    break;
                }
            }

            Debug.WriteLine($"Background Color: {bestColor}, Count: {bestColorSameCount}");

            mutex.WaitOne();
            clearColor = bestColor;
            mutex.ReleaseMutex();
            Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, Opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
        }

        /// <summary>
        /// 互斥锁, 用于同步临界数据
        /// </summary>
        protected Mutex mutex = new();

        /// <summary>
        /// 取消令牌, 用于结束窗口线程
        /// </summary>
        private CancellationTokenSource cancelTokenSrc = new();

        /// <summary>
        /// 窗口创建事件, 用于同步等待窗口创建完成
        /// </summary>
        private ManualResetEvent windowCreatedEvent = new(false);

        /// <summary>
        /// 计时器, 计算每一帧的时间间隔
        /// </summary>
        private SFML.System.Clock clock = new();

        /// <summary>
        /// 窗口背景色, 会影响显示半透明边缘颜色效果
        /// </summary>
        private SFML.Graphics.Color clearColor = new(128, 128, 128);

        /// <summary>
        /// 用于系统 api 设置透明颜色键
        /// </summary>
        private uint crKey { get => BinaryPrimitives.ReverseEndianness(clearColor.ToInteger()); }

        /// <summary>
        /// SFML 窗口对象
        /// </summary>
        protected SFML.Graphics.RenderWindow? window;

        /// <summary>
        /// 窗口循环线程
        /// </summary>
        private Task? windowLoopTask;

        private BackgroudColor backgroudColor = BackgroudColor.Gray;

        /// <summary>
        /// 窗口可见性成员变量
        /// </summary>
        private bool visible = false;

        /// <summary>
        /// 窗口最大帧率成员变量
        /// </summary>
        private uint maxFps = 30;

        /// <summary>
        /// 记录点击时的窗口点击位置
        /// </summary>
        private SFML.System.Vector2i? windowPressedPosition = null;

        /// <summary>
        /// 记录点击时 Spine 的世界位置
        /// </summary>
        private SFML.System.Vector2f? spinePressedPosition = null;

        /// <summary>
        /// 双击行为计时器
        /// </summary>
        private SFML.System.Clock doubleClickClock = new();

        /// <summary>
        /// 是否处于双击检测中
        /// </summary>
        private bool doubleClickChecking = false;

        /// <summary>
        /// 是否处于拖动状态
        /// </summary>
        private bool isDragging = false;

        /// <summary>
        /// 休眠判定超时时间
        /// </summary>
        private const float TimeToSleep = 300f;

        /// <summary>
        /// 最近一次记录的用户上次输入时间间隔
        /// </summary>
        private float lastLastInputTime = 1f;

        /// <summary>
        /// 线程函数
        /// </summary>
        private static void SpineWindowTask(SpineWindow self)
        {
            self.CreateWindow();
            self.WindowLoop();
        }

        /// <summary>
        /// 要使用的背景颜色
        /// </summary>
        public BackgroudColor BackgroudColor 
        {
            get => backgroudColor;
            set { backgroudColor = value; UpdateProperBackgroudColor(); }
        }

        /// <summary>
        /// 具体的背景颜色
        /// </summary>
        public SFML.Graphics.Color ClearColor { get { mutex.WaitOne(); var c = clearColor; mutex.ReleaseMutex(); return c; } }

        /// <summary>
        /// 窗口整体透明度
        /// </summary>
        public byte Opacity
        {
            get
            {
                uint crKey = 0;
                byte bAlpha = 0;
                uint dwFlags = 0;
                Win32.GetLayeredWindowAttributes(window.SystemHandle, ref crKey, ref bAlpha, ref dwFlags);
                return ((dwFlags & Win32.LWA_ALPHA) != 0) ? bAlpha : (byte)255;
            }
            set => Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, value, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
        }

        public bool WallpaperMode 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// 鼠标穿透
        /// </summary>
        public bool MouseClickThrough
        {
            get => (Win32.GetWindowLong(window.SystemHandle, Win32.GWL_EXSTYLE) & Win32.WS_EX_TRANSPARENT) != 0;
            set
            {
                var exStyle = Win32.GetWindowLong(window.SystemHandle, Win32.GWL_EXSTYLE);
                if (value)
                    exStyle |= Win32.WS_EX_TRANSPARENT;
                else
                    exStyle &= ~Win32.WS_EX_TRANSPARENT;
                Win32.SetWindowLong(window.SystemHandle, Win32.GWL_EXSTYLE, exStyle);
            }
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public bool Visible
        {
            get { mutex.WaitOne(); var v = visible; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); visible = value; mutex.ReleaseMutex(); window.SetVisible(value); if (value) Trigger_Show(); }
        }

        /// <summary>
        /// 最大帧率
        /// </summary>
        public uint MaxFps
        {
            get => maxFps;
            set { maxFps = value; window.SetFramerateLimit(value); }
        }

        /// <summary>
        /// 窗口位置
        /// </summary>
        public SFML.System.Vector2i Position 
        { 
            get => window.Position;
            set { window.Position = value; PositionReg = value; }
        }

        /// <summary>
        /// 窗口位置在注册表的存储值
        /// </summary>
        public SFML.System.Vector2i PositionReg
        {
            get
            {
                SFML.System.Vector2i ret = new(0, 0);
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        int.TryParse(spkey.GetValue("PositionX", "0").ToString(), out ret.X);
                        int.TryParse(spkey.GetValue("PositionY", "0").ToString(), out ret.Y);
                    }
                }
                return ret;
            }
            set
            {
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("PositionX", value.X.ToString());
                        spkey.SetValue("PositionY", value.Y.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        public SFML.System.Vector2u Size
        {
            get => window.Size;
            set => window.Size = value; // 会触发 Resized 事件
        }

        /// <summary>
        /// 窗口大小在注册表的存储值
        /// </summary>
        public SFML.System.Vector2u SizeReg
        {
            get
            {
                SFML.System.Vector2u ret = new(0, 0);
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        uint.TryParse(spkey.GetValue("SizeX", "0").ToString(), out ret.X);
                        uint.TryParse(spkey.GetValue("SizeY", "0").ToString(), out ret.Y);
                    }
                }
                return ret;
            }
            set
            {
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey(@"Software\SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("SizeX", value.X.ToString());
                        spkey.SetValue("SizeY", value.Y.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 重置窗口和 Spine 的位置和大小
        /// </summary>
        public void ResetPositionAndSize()
        {
            SpinePosition = new(0, 0);
            Position = new(0, 0);
            Size = new(1000, 1000);
        }

        /// <summary>
        /// 创建窗口
        /// </summary>
        private void CreateWindow()
        {
            // 创建窗口
            window = new(new(1000, 1000), "spine", SFML.Window.Styles.None);

            // 设置窗口特殊属性
            var hWnd = window.SystemHandle;
            var style = Win32.GetWindowLong(hWnd, Win32.GWL_STYLE) | Win32.WS_POPUP;
            var exStyle = Win32.GetWindowLong(hWnd, Win32.GWL_EXSTYLE) | Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_TOPMOST;
            Win32.SetWindowLong(hWnd, Win32.GWL_STYLE, style);
            Win32.SetWindowLong(hWnd, Win32.GWL_EXSTYLE, exStyle);
            Win32.SetLayeredWindowAttributes(hWnd, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            Win32.SetWindowPos(hWnd, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
            //Win32.SetParent(hWnd, 0x004A096E);

            // 设置窗口属性
            window.SetVisible(visible);
            window.SetFramerateLimit(maxFps);
            window.Position = PositionReg;
            window.Size = SizeReg;

            // 最后调整视窗
            var view = window.GetView();
            view.Center = new(0, 200);
            view.Size = new(window.Size.X, -window.Size.Y); // SFML 窗口 y 轴默认向下
            window.SetView(view);

            // 注册窗口事件
            RegisterEvents();
            windowCreatedEvent.Set();
        }

        /// <summary>
        /// 窗口事件循环
        /// </summary>
        private void WindowLoop()
        {
            while (true)
            {
                if (cancelTokenSrc.Token.IsCancellationRequested)
                {
                    window.Close();
                    window = null;
                    break;
                }

                window.DispatchEvents();
                Update();

                mutex.WaitOne(); var v = visible; var c = clearColor; mutex.ReleaseMutex();
                if (v)
                {
                    window.Clear(c);
                    Render();
                    window.Display();
                }
            }
        }

        /// <summary>
        /// 状态逻辑更新
        /// </summary>
        private void Update()
        {
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            mutex.WaitOne();
            foreach (var sp in spineSlots) sp?.Update(delta);
            mutex.ReleaseMutex();
            Trigger_StateUpdated();

            var lastInputTime = (float)Win32.GetLastInputTime().TotalSeconds;
            if (lastInputTime >= TimeToSleep && lastLastInputTime < TimeToSleep)
                Trigger_FallAsleep();
            else if (lastInputTime < TimeToSleep && lastLastInputTime >= TimeToSleep)
                Trigger_WakeUp();
            lastLastInputTime = lastInputTime;
        }

        /// <summary>
        /// 画面渲染更新
        /// </summary>
        private void Render()
        {
            mutex.WaitOne();
            for (int i = spineSlots.Length - 1; i >= 0; i--) { var sp = spineSlots[i];  if (sp is not null) window.Draw(sp); }
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// 注册所有窗口事件
        /// </summary>
        private void RegisterEvents()
        {
            window.Resized += (object? s, SFML.Window.SizeEventArgs e) => Resized(this, e);
            window.MouseButtonPressed += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonPressed(this, e);
            window.MouseMoved += (object? s, SFML.Window.MouseMoveEventArgs e) => MouseMoved(this, e);
            window.MouseButtonReleased += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonReleased(this, e);
            window.MouseWheelScrolled += (object? s, SFML.Window.MouseWheelScrollEventArgs e) => MouseWheelScrolled(this, e);
        }

        /********************************* 封装后的窗口事件 *********************************/

        private void Resized(SFML.Window.SizeEventArgs e)
        {
            // 设置 Size 属性的时候会触发该事件, 但是要窗口事件循环在运行
            var view = window.GetView();
            view.Size = new(window.Size.X, -window.Size.Y);
            window.SetView(view);
            SizeReg = window.Size;
        }

        private void MouseButtonPressed(SFML.Window.MouseButtonEventArgs e)
        {
            // 记录点击位置
            windowPressedPosition = new(e.X, e.Y);
            spinePressedPosition = SpinePosition;

            // 检查双击超时
            if (doubleClickChecking && doubleClickClock.ElapsedTime.AsMilliseconds() > Win32.GetDoubleClickTime())
                doubleClickChecking = false;

            // 事件处理
            switch (e.Button)
            {
                case SFML.Window.Mouse.Button.Left:
                    break;
                case SFML.Window.Mouse.Button.Right:
                    break;
                default:
                    break;
            }
        }

        private void MouseMoved(SFML.Window.MouseMoveEventArgs e)
        {
            // 计算移动距离
            var windowDelta = new SFML.System.Vector2i(0, 0);
            var worldDelta = new SFML.System.Vector2f(0, 0);
            if (windowPressedPosition is not null)
            { 
                var windowSrc = (SFML.System.Vector2i)windowPressedPosition;
                var windowDst = new SFML.System.Vector2i(e.X, e.Y);
                var worldSrc = window.MapPixelToCoords(windowSrc);
                var worldDst = window.MapPixelToCoords(windowDst);
                windowDelta = windowDst - windowSrc;
                worldDelta = worldDst - worldSrc;
            }

            // 获取按键状态
            var leftDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left);
            var rightDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Right);

            // 判断是否开始拖动
            // 任意一个键是按下的且移动距离大于阈值
            if (!isDragging && (leftDown || rightDown) && (Math.Abs(windowDelta.X) > 4 || Math.Abs(windowDelta.Y) > 4))
            { 
                isDragging = true;
                doubleClickChecking = false;
                Trigger_MouseDragBegin(e);
            }

            if (isDragging)
            {
                // 拖动时右键处于按下则显示边框
                var style = Win32.GetWindowLong(window.SystemHandle, Win32.GWL_STYLE);
                if (rightDown && (style & Win32.WS_BORDER) == 0)
                {
                    Win32.SetWindowLong(window.SystemHandle, Win32.GWL_STYLE, style | Win32.WS_BORDER);
                    Win32.SetWindowPos(window.SystemHandle, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_REFRESHLONG);
                }
                else if (!rightDown && (style & Win32.WS_BORDER) != 0)
                {
                    Win32.SetWindowLong(window.SystemHandle, Win32.GWL_STYLE, style & ~Win32.WS_BORDER);
                    Win32.SetWindowPos(window.SystemHandle, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_REFRESHLONG);
                }

                // 如果左键被按下则拖动窗口
                // 否则右键被按下则拖动内部精灵
                if (leftDown)
                {
                    Position = Position + windowDelta;
                }
                else if (rightDown && spinePressedPosition is not null)
                {
                    SpinePosition = (SFML.System.Vector2f)spinePressedPosition + worldDelta;
                }

            }
        }

        private void MouseButtonReleased(SFML.Window.MouseButtonEventArgs e)
        {
            // 清除位置记录
            windowPressedPosition = null;
            spinePressedPosition = null;

            if (isDragging)
            {
                // 拖动过程任意一个键释放都结束拖动
                var style = Win32.GetWindowLong(window.SystemHandle, Win32.GWL_STYLE);
                if ((style & Win32.WS_BORDER) != 0)
                {
                    Win32.SetWindowLong(window.SystemHandle, Win32.GWL_STYLE, style & ~Win32.WS_BORDER);
                    Win32.SetWindowPos(window.SystemHandle, 0, 0, 0, 0, 0, Win32.SWP_REFRESHLONG);
                }

                isDragging = false;
                Trigger_MouseDragEnd(e);
            }
            else
            {
                // 双击检测
                var isDoubleClick = false;
                if (!doubleClickChecking)
                {
                    doubleClickClock.Restart();
                    doubleClickChecking = true;
                }
                else
                {
                    isDoubleClick = doubleClickClock.ElapsedTime.AsMilliseconds() <= Win32.GetDoubleClickTime();
                    if (isDoubleClick)
                        doubleClickChecking = false;
                    else
                        doubleClickClock.Restart();
                }

                // 点击事件处理
                // 双击和单击只触发一个
                if (isDoubleClick)
                {
                    // 双击事件处理
                    switch (e.Button)
                    {
                        case SFML.Window.Mouse.Button.Left:
                            break;
                        case SFML.Window.Mouse.Button.Right:
                            // 右键双击时显示/隐藏缩放框
                            var style = Win32.GetWindowLong(window.SystemHandle, Win32.GWL_STYLE);
                            Win32.SetWindowLong(window.SystemHandle, Win32.GWL_STYLE, style ^ Win32.WS_SIZEBOX);
                            Win32.SetWindowPos(window.SystemHandle, IntPtr.Zero, 0, 0, 0, 0, Win32.SWP_REFRESHLONG);
                            break;
                        default:
                            break;
                    }

                    Trigger_MouseButtonDoubleClick(e);
                }
                else
                {
                    // 单击事件处理
                    switch (e.Button)
                    {
                        case SFML.Window.Mouse.Button.Left:
                            break;
                        case SFML.Window.Mouse.Button.Right:
                            break;
                        default:
                            break;
                    }

                    Trigger_MouseButtonClick(e);
                }
            }
        }

        private void MouseWheelScrolled(SFML.Window.MouseWheelScrollEventArgs e)
        {
            Trigger_MouseWheelScroll(e);
        }

        /********************************* 事件触发器, 子类重写进行逻辑处理 *********************************/

        protected virtual void Trigger_SpineLoaded(int index) { }
        protected virtual void Trigger_StateUpdated() { }
        protected virtual void Trigger_MouseButtonClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseButtonDoubleClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseDragBegin(SFML.Window.MouseMoveEventArgs e) { }
        protected virtual void Trigger_MouseDragEnd(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseWheelScroll(SFML.Window.MouseWheelScrollEventArgs e) { }
        protected virtual void Trigger_WorkBegin() { }
        protected virtual void Trigger_WorkEnd() { }
        protected virtual void Trigger_FallAsleep() {  }
        protected virtual void Trigger_WakeUp() { }
        protected virtual void Trigger_Show() { }

        /********************************* 静态窗口事件 *********************************/

        private static void Resized(SpineWindow self, SFML.Window.SizeEventArgs e) { self.Resized(e); }
        private static void MouseButtonPressed(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonPressed(e); }
        private static void MouseMoved(SpineWindow self, SFML.Window.MouseMoveEventArgs e) { self.MouseMoved(e); }
        private static void MouseButtonReleased(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonReleased(e); }
        private static void MouseWheelScrolled(SpineWindow self, SFML.Window.MouseWheelScrollEventArgs e) { self.MouseWheelScrolled(e); }
    }

    /// <summary>
    /// Win32 Sdk 包装类
    /// </summary>
    internal static class Win32
    {
        public const int GWL_STYLE = -16;
        public const int WS_SIZEBOX = 0x40000;
        public const int WS_BORDER = 0x800000;
        public const int WS_POPUP = unchecked((int)0x80000000);

        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOPMOST = 0x8;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_WINDOWEDGE = 0x100;
        public const int WS_EX_CLIENTEDGE = 0x200;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;

        public const uint LWA_COLORKEY = 0x1;
        public const uint LWA_ALPHA = 0x2;

        public const IntPtr HWND_TOPMOST = -1;

        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_FRAMECHANGED = 0x0020;
        public const uint SWP_REFRESHLONG = SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED;

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetLayeredWindowAttributes(IntPtr hWnd, ref uint crKey, ref byte bAlpha, ref uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint pcrKey, byte pbAlpha, uint pdwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static TimeSpan GetLastInputTime()
        {
            LASTINPUTINFO lastInputInfo = new();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);

            uint idleTimeMillis = 1000;
            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint tickCount = (uint)Environment.TickCount;
                uint lastInputTick = lastInputInfo.dwTime;
                idleTimeMillis = tickCount - lastInputTick;
            }          
            return TimeSpan.FromMilliseconds(idleTimeMillis);
        }
    }
}

