using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpineWindow
{
    /// <summary>
    /// 窗体类型枚举量
    /// </summary>
    public enum SpineWindowType
    {
        AzurLaneSD = 0,                 // 碧蓝航线_后宅小人
        AzurLaneDynamic = 1,            // 碧蓝航线_动态立绘
        ArknightsDynamic = 2,           // 明日方舟_动态立绘
        ArknightsBuild = 3,             // 明日方舟_基建小人
        ArknightsBattle = 4,            // 明日方舟_战斗小人
    }

    /// <summary>
    /// 自动背景颜色类型
    /// </summary>
    public enum AutoBackgroudColorType
    {
        None = 0,
        Black = 1,
        White = 2,
        Gray = 3,
    }

    /// <summary>
    /// SpineWindow 抽象基类
    /// </summary>
    public abstract partial class SpineWindow : IDisposable
    {
#if DEBUG
        protected const string ClassName = "SpineWindow_d";
#else
        protected const string ClassName = "SpineWindow";
#endif

        /// <summary>
        /// 互斥锁, 用于同步临界数据
        /// </summary>
        protected Mutex mutex = new();

        /// <summary>
        /// 创建指定类型 Spine 窗口
        /// </summary>
        public static SpineWindow New(SpineWindowType type, uint slotCount)
        {
            return type switch
            {
                SpineWindowType.AzurLaneSD => new AzurLaneSD(slotCount),
                SpineWindowType.AzurLaneDynamic => new AzurLaneDynamic(slotCount),
                SpineWindowType.ArknightsDynamic => new ArknightsDynamic(slotCount),
                SpineWindowType.ArknightsBuild => new ArknightsBuild(slotCount),
                SpineWindowType.ArknightsBattle => new ArknightsBattle(slotCount),
                _ => throw new NotImplementedException($"Unknown SpineWindow type: {type}"),
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
                if (t == typeof(AzurLaneSD)) return SpineWindowType.AzurLaneSD;
                if (t == typeof(AzurLaneDynamic)) return SpineWindowType.AzurLaneDynamic;
                if (t == typeof(ArknightsDynamic)) return SpineWindowType.ArknightsDynamic;
                if (t == typeof(ArknightsBuild)) return SpineWindowType.ArknightsBuild;
                if (t == typeof(ArknightsBattle)) return SpineWindowType.ArknightsBattle;
                throw new InvalidOperationException($"Unknown SpineWindow type {this}");
            }
        }

        /// <summary>
        /// SpineWindow 基类, 提供 Spine 装载和动画交互
        /// </summary>
        public SpineWindow(uint slotCount)
        {
            spineSlots = new Spine.Spine[slotCount];
            colorTables = new Dictionary<uint, uint>[slotCount];
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~SpineWindow()
        {
            if (window is not null)
                Dispose();
        }

        /// <summary>
        /// Dispose 接口实现, 销毁窗口需要调用以停止窗口线程
        /// </summary>
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

        #region Spine 相关功能实现

        protected Spine.Spine?[] spineSlots;                    // Spine 对象装载数组
        public int SlotCount { get => spineSlots.Length; }      // 窗口可用最大 Spine 装载数
        private Dictionary<uint, uint>?[] colorTables;           // 背景颜色表, 供自动生成背景颜色时使用

        /// <summary>
        /// 资源文件夹, 提供语音等资源的位置, 没有加载任何 Spine 时返回 null
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
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
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
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
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
        public void LoadSpine(string version, string skelPath, string? atlasPath = null, int index = 0)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            Debug.WriteLine($"Loading spine[{index}]({version}) from {skelPath}, {atlasPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, atlasPath); }
            catch { throw; }

            // 尝试用已有的 Spine 对象恢复位置
            var originalPosition = SpinePosition;
            spineNew.X = originalPosition.X;
            spineNew.Y = originalPosition.Y;

            mutex.WaitOne();
            spineSlots[index] = spineNew;
            colorTables[index] = null;
            mutex.ReleaseMutex();

            // 尝试修正自动背景颜色
            SetProperAutoBackgroudColor();

            Trigger_SpineLoaded(index);
            Debug.Write("spine animiation: ");
            foreach (var a in spineSlots[index].AnimationNames) Debug.Write($"{a}; "); Debug.WriteLine("");
        }

        /// <summary>
        /// 卸载指定槽位 Spine
        /// </summary>
        public void UnloadSpine(int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");

            Debug.WriteLine($"Unload spine[{index}]");
            mutex.WaitOne();
            spineSlots[index] = null;
            colorTables[index] = null;
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// 获取指定槽位的 Spine 资源路径
        /// </summary>
        public string? GetSpineSkelPath(int index)
        {
            if (index >= spineSlots.Length)
                throw new ArgumentOutOfRangeException($"Max spine slot count: {spineSlots.Length}, got index {index}");
            mutex.WaitOne();
            var v = spineSlots[index]?.SkelPath;
            mutex.ReleaseMutex();
            return v;
        }

        #endregion

        #region 窗口相关功能实现

        protected SFML.Graphics.RenderWindow? window;                   // SFML 窗口对象
        public IntPtr Handle { get => window.SystemHandle; }            // 窗口句柄, 假定 window 一定不是 null
        private Task? windowLoopTask;                                   // 窗口循环线程
        private CancellationTokenSource cancelTokenSrc = new();         // 取消令牌, 用于结束窗口线程
        private ManualResetEvent windowCreatedEvent = new(false);       // 窗口创建事件, 用于同步等待窗口创建完成
        private SFML.System.Clock clock = new();                        // 计时器, 计算每一帧的时间间隔
        private SFML.System.Vector2i? windowPressedPosition = null;     // 记录点击时的窗口点击位置
        private SFML.System.Vector2f? spinePressedPosition = null;      // 记录点击时 Spine 的世界位置
        private SFML.System.Clock doubleClickClock = new();             // 双击行为计时器
        private bool doubleClickChecking = false;                       // 是否处于双击检测中
        private bool isDragging = false;                                // 是否处于拖动状态
        private const float TimeToSleep = 300f;                         // 休眠判定超时时间
        private float lastLastInputTime = 1f;                           // 最近一次记录的用户上次输入时间间隔

        /// <summary>
        /// 要使用的自动背景颜色, null 不进行自动颜色, 可以通过 BackgroundColor 属性进行设置
        /// </summary>
        public AutoBackgroudColorType AutoBackgroudColor
        {
            get => autoBackgroudColor;
            set { autoBackgroudColor = value; SetProperAutoBackgroudColor(); }
        }
        private AutoBackgroudColorType autoBackgroudColor = AutoBackgroudColorType.Gray;

        /// <summary>
        /// 窗口背景色, 会影响显示半透明边缘颜色效果, 如果是自动颜色则无法进行设置
        /// </summary>
        public SFML.Graphics.Color BackgroudColor
        {
            get { mutex.WaitOne(); var v = backgroundColor; mutex.ReleaseMutex(); return v; }
            set
            {
                // 如果有自动颜色则不接受直接设置颜色
                if (autoBackgroudColor != AutoBackgroudColorType.None)
                    return;

                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                // 此处令 B 总是等于 R, 并且强制保证 A 分量设置为 0
                value.A = 0;
                value.B = value.R;
                mutex.WaitOne();
                backgroundColor = value;
                mutex.ReleaseMutex();
                Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, Opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            }
        }
        private SFML.Graphics.Color backgroundColor = new(128, 128, 128, 0);

        /// <summary>
        /// 用于系统 api 设置透明颜色键
        /// </summary>
        private uint crKey { get => BinaryPrimitives.ReverseEndianness(backgroundColor.ToInteger()); }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public bool Visible
        {
            get { mutex.WaitOne(); var v = visible; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); visible = value; mutex.ReleaseMutex(); window.SetVisible(value); if (value) Trigger_Show(); }
        }
        private bool visible = false;

        /// <summary>
        /// 最大帧率
        /// </summary>
        public uint MaxFps
        {
            get => maxFps;
            set { maxFps = value; window.SetFramerateLimit(value); }
        }
        private uint maxFps = 30;

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
        /// 壁纸模式, 嵌入桌面
        /// </summary>
        public bool WallpaperMode
        {
            get
            {
                // WS_POPUP 要用 GetAncestor 获取父窗口
                var workerW = Win32.GetWorkerW();
                return workerW != IntPtr.Zero && Win32.GetAncestor(window.SystemHandle, Win32.GA_PARENT) == workerW;
            }
            set
            {
                var hWnd = window.SystemHandle;
                var progman = Win32.FindWindow("Progman", null);
                if (progman == IntPtr.Zero)
                    return;
                if (value)
                {
                    // 确保 WorkerW 被创建
                    Win32.SendMessageTimeout(progman, Win32.WM_SPAWN_WORKER, IntPtr.Zero, IntPtr.Zero, Win32.SMTO_NORMAL, 1000, out _);
                    var workerW = Win32.GetWorkerW();
                    if (workerW == IntPtr.Zero)
                        return;
                    Win32.SetParent(hWnd, workerW);

                    // 触发一次内部窗口大小修改逻辑
                    var s = window.Size;
                    window.Size = new(s.X + 1, s.Y + 1);
                    window.Size = s;
                }
                else
                {
                    // 一个奇怪的 BUG, 如果在解除 WorkerW 父窗口时透明度不是 255
                    // 则解除之后透明度不会超过解除时的值
                    var opacity = Opacity;
                    Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
                    Win32.SetParent(hWnd, IntPtr.Zero);

                    // 触发一次内部窗口大小修改逻辑
                    var s = window.Size;
                    window.Size = new(s.X + 1, s.Y + 1);
                    window.Size = s;

                    // 必须重设分层属性, 否则透明度有问题
                    Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
                }
            }
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
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
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
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
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
            set => window.Size = value; // 当 Size 不同时会触发 Resized 事件
        }

        /// <summary>
        /// 窗口大小在注册表的存储值
        /// </summary>
        public SFML.System.Vector2u SizeReg
        {
            get
            {
                SFML.System.Vector2u ret = new(0, 0);
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
                {
                    if (spkey is not null)
                    {
                        uint.TryParse(spkey.GetValue("SizeX", "1000").ToString(), out ret.X);
                        uint.TryParse(spkey.GetValue("SizeY", "1000").ToString(), out ret.Y);
                    }
                }
                return ret;
            }
            set
            {
                using (RegistryKey spkey = Registry.CurrentUser.CreateSubKey($"Software\\{ClassName}"))
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
        /// 线程函数
        /// </summary>
        private static void SpineWindowTask(SpineWindow self)
        {
            self.CreateWindow();
            self.WindowLoop();
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
        /// 窗口主循环
        /// </summary>
        private void WindowLoop()
        {
            while (true)
            {
                if (cancelTokenSrc.Token.IsCancellationRequested)
                {
                    SpinePositionReg = SpinePosition;
                    PositionReg = Position;
                    SizeReg = Size;
                    window.Close();
                    window = null;
                    break;
                }

                window.DispatchEvents();

                mutex.WaitOne(); var v = visible; var c = backgroundColor; mutex.ReleaseMutex();
                if (v)
                {
                    Update();
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
            // 更新内部对象状态
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            mutex.WaitOne();
            foreach (var sp in spineSlots) sp?.Update(delta);
            mutex.ReleaseMutex();
            Trigger_StateUpdated();

            // 检测用户上次输入时间
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
            for (int i = spineSlots.Length - 1; i >= 0; i--) { var sp = spineSlots[i]; if (sp is not null) window.Draw(sp); }
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// 自动背景色模式下设置一个正确的背景色, 如果未使用自动颜色则不做任何操作
        /// </summary>
        private void SetProperAutoBackgroudColor()
        {
            if (autoBackgroudColor == AutoBackgroudColorType.None)
                return;

            // 确保每个加载了 Spine 的槽位有颜色表
            for (int i = 0; i < spineSlots.Length; i++)
            {
                if (spineSlots[i] is not null && colorTables[i] is null)
                    colorTables[i] = spineSlots[i].ColorTable;
            }

            var rnd = new Random();
            var bestColor = SFML.Graphics.Color.Transparent;
            uint bestColorSameCount = uint.MaxValue;
            var currentColor = SFML.Graphics.Color.Transparent;
            for (int i = 0; i < 10; i++)
            {
                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                switch (autoBackgroudColor)
                {
                    case AutoBackgroudColorType.Black:
                        currentColor.R = currentColor.B = (byte)rnd.Next(0, 20);
                        currentColor.G = (byte)rnd.Next(0, 20);
                        break;
                    case AutoBackgroudColorType.White:
                        currentColor.R = currentColor.B = (byte)rnd.Next(235, 255);
                        currentColor.G = (byte)rnd.Next(235, 255);
                        break;
                    case AutoBackgroudColorType.Gray:
                        currentColor.R = currentColor.B = (byte)rnd.Next(118, 138);
                        currentColor.G = (byte)rnd.Next(118, 138);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type: {autoBackgroudColor}");
                }

                var k = currentColor.ToInteger();
                uint count = 0;
                uint tmp = 0;
                // 统计在所有表中该颜色出现的次数
                // 选择重复次数最少的颜色, 如果发现了 0 次, 则立即结束查找
                foreach (var table in colorTables)
                {
                    if (table is not null && table.TryGetValue(k, out tmp))
                        count += tmp;
                }
                if (count < bestColorSameCount)
                {
                    bestColor = currentColor;
                    bestColorSameCount = count;
                }
                if (bestColorSameCount <= 0)
                    break;
            }

            Debug.WriteLine($"AutoBackground Color: {bestColor}, Count: {bestColorSameCount}");

            mutex.WaitOne();
            backgroundColor = bestColor;
            mutex.ReleaseMutex();
            Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, Opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
        }

        #endregion

        #region 窗口事件实现

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
            // 设置 Size 属性的时候会触发该事件
            var desktopMode = SFML.Window.VideoMode.DesktopMode;
            if (e.Height == desktopMode.Height && e.Width == desktopMode.Width)
            {
                window.Size = new(e.Width, e.Height - 1);
            }
            else
            {
                var view = window.GetView();
                view.Size = new(window.Size.X, -window.Size.Y);
                window.SetView(view);
                SizeReg = window.Size;
            }
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
        protected virtual void Trigger_FallAsleep() { }
        protected virtual void Trigger_WakeUp() { }
        protected virtual void Trigger_Show() { }

        /********************************* 静态窗口事件 *********************************/

        private static void Resized(SpineWindow self, SFML.Window.SizeEventArgs e) { self.Resized(e); }
        private static void MouseButtonPressed(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonPressed(e); }
        private static void MouseMoved(SpineWindow self, SFML.Window.MouseMoveEventArgs e) { self.MouseMoved(e); }
        private static void MouseButtonReleased(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonReleased(e); }
        private static void MouseWheelScrolled(SpineWindow self, SFML.Window.MouseWheelScrollEventArgs e) { self.MouseWheelScrolled(e); }

        #endregion
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

        public const int WM_SPAWN_WORKER = 0x052C; // 一个未公开的神秘消息

        public const uint SMTO_NORMAL = 0x0000;
        public const uint SMTO_BLOCK = 0x0001;
        public const uint SMTO_ABORTIFHUNG = 0x0002;
        public const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;

        public const uint GA_PARENT = 1;
        public const uint GW_OWNER = 4;

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
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

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

        public static IntPtr GetWorkerW()
        {
            var progman = FindWindow("Progman", null);
            if (progman == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr hWnd = IntPtr.Zero;
            do
            {
                hWnd = FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                if (hWnd != IntPtr.Zero && GetWindow(hWnd, GW_OWNER) == progman)
                    break;
            } while (hWnd != IntPtr.Zero);
            return hWnd;
        }
    }
}

