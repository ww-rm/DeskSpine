using Microsoft.Win32;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpineWindow
{
    public enum BackgroudColor
    {
        Black = 0,
        White = 1,
        Gray = 2
    }

    public abstract class SpineWindow: IDisposable
    {
        public SpineWindow(uint slotCont = 3)
        {
            spineSlots = new Spine.Spine[slotCont];
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

        protected Spine.Spine?[] spineSlots;

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

        public SFML.System.Vector2f SpinePositionReg
        {
            get
            {
                SFML.System.Vector2f ret = new(0, 0);
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
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
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("SpinePositionX", value.X.ToString());
                        spkey.SetValue("SpinePositionY", value.Y.ToString());
                    }
                }
            }
        }

        public void LoadSpine(string version, string skelPath, string? atlasPath = null, uint index = 0)
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

            UpdateProperBackgroudColor();
            Trigger_SpineLoaded(index);
            Debug.Write("spine animiation: ");
            foreach (var a in spineSlots[index].AnimationNames) Debug.Write($"{a}; "); Debug.WriteLine("");
        }

        public void UnloadSpine(uint index)
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
            // TODO: 优化查找时间
            var colors = new Dictionary<uint, uint>();
            List<string> paths = [];

            mutex.WaitOne();
            foreach (var sp in spineSlots)
            {
                if (sp is null) continue;
                paths.AddRange(sp.PngPaths);
            }
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
                if (colors.TryGetValue(k, out count))
                {
                    if (count < bestColorSameCount)
                    {
                        bestColor = tmpColor;
                        bestColorSameCount = count;
                    }
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
            Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY);
        }

        protected Mutex mutex = new();
        private CancellationTokenSource cancelTokenSrc = new();
        private ManualResetEvent windowCreatedEvent = new(false);
        private SFML.System.Clock clock = new();
        private SFML.Graphics.Color clearColor = new(128, 128, 128);
        private uint crKey { get => BinaryPrimitives.ReverseEndianness(clearColor.ToInteger()); }
        protected SFML.Graphics.RenderWindow? window;
        private Task? windowLoopTask;

        private BackgroudColor backgroudColor = BackgroudColor.Gray;
        private bool visible = false;
        private uint maxFps = 30;

        private SFML.System.Vector2i? windowPressedPosition = null;
        private SFML.System.Vector2f? spinePressedPosition = null;
        private SFML.System.Clock doubleClickClock = new();
        private bool doubleClickChecking = false;
        private bool isDragging = false;

        private static void SpineWindowTask(SpineWindow self)
        {
            self.CreateWindow();
            self.WindowLoop();
        }

        public BackgroudColor BackgroudColor 
        {
            get => backgroudColor;
            set { backgroudColor = value; UpdateProperBackgroudColor(); }
        }

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
            set => Win32.SetLayeredWindowAttributes(window.SystemHandle, 0, value, Win32.LWA_ALPHA);
        }

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

        public bool Visible
        {
            get { mutex.WaitOne(); var v = visible; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); visible = value; mutex.ReleaseMutex(); window.SetVisible(value); if (value) Trigger_Show(); }
        }

        public uint MaxFps
        {
            get => maxFps;
            set { maxFps = value; window.SetFramerateLimit(value); }
        }

        public SFML.System.Vector2i Position 
        { 
            get => window.Position;
            set { window.Position = value; PositionReg = value; }
        }

        public SFML.System.Vector2i PositionReg
        {
            get
            {
                SFML.System.Vector2i ret = new(0, 0);
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
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
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("PositionX", value.X.ToString());
                        spkey.SetValue("PositionY", value.Y.ToString());
                    }
                }
            }
        }

        public SFML.System.Vector2u Size
        {
            get => window.Size;
            set => window.Size = value; // 会触发 Resized 事件
        }

        public SFML.System.Vector2u SizeReg
        {
            get
            {
                SFML.System.Vector2u ret = new(0, 0);
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
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
                using (RegistryKey software = Registry.CurrentUser.CreateSubKey("Software"), spkey = software?.CreateSubKey("SpineWindow"))
                {
                    if (spkey is not null)
                    {
                        spkey.SetValue("SizeX", value.X.ToString());
                        spkey.SetValue("SizeY", value.Y.ToString());
                    }
                }
            }
        }

        public void ResetPositionAndSize()
        {
            SpinePosition = new(0, 0);
            Position = new(0, 0);
            Size = new(1000, 1000);
        }

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

        private void WindowLoop()
        {
            while (true)
            {
                if (cancelTokenSrc.Token.IsCancellationRequested)
                {
                    window.Close();
                    window = null;
                    cancelTokenSrc.Token.ThrowIfCancellationRequested();
                }

                window.DispatchEvents();
                Update();

                mutex.WaitOne();
                var v = visible;
                var c = clearColor;
                mutex.ReleaseMutex();
                if (v)
                {
                    window.Clear(c);
                    Render();
                    window.Display();
                }
            }
        }

        private void Update()
        {
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            mutex.WaitOne();
            foreach (var sp in spineSlots) sp?.Update(delta);
            mutex.ReleaseMutex();
            Trigger_StateUpdated();
        }

        private void Render()
        {
            mutex.WaitOne();
            for (int i = spineSlots.Length - 1; i >= 0; i--) { var sp = spineSlots[i];  if (sp is not null) window.Draw(sp); }
            mutex.ReleaseMutex();
        }

        private void RegisterEvents()
        {
            window.Resized += (object? s, SFML.Window.SizeEventArgs e) => Resized(this, e);
            window.MouseButtonPressed += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonPressed(this, e);
            window.MouseMoved += (object? s, SFML.Window.MouseMoveEventArgs e) => MouseMoved(this, e);
            window.MouseButtonReleased += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonReleased(this, e);
            window.MouseWheelScrolled += (object? s, SFML.Window.MouseWheelScrollEventArgs e) => MouseWheelScrolled(this, e);
        }

        private void Resized(SFML.Window.SizeEventArgs e)
        {
            // 设置 Size 属性的时候会触发该事件
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
            var delta = new SFML.System.Vector2i(0, 0);
            if (windowPressedPosition is not null)
                delta = (SFML.System.Vector2i)(new SFML.System.Vector2i(e.X, e.Y) - windowPressedPosition);

            // 获取按键状态
            var leftDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left);
            var rightDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Right);

            // 判断是否开始拖动
            // 任意一个键是按下的且移动距离大于阈值
            if (!isDragging && (leftDown || rightDown) && (Math.Abs(delta.X) > 4 || Math.Abs(delta.Y) > 4))
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
                    Position = Position + delta;
                }
                else if (rightDown && spinePressedPosition is not null)
                {
                    var sDelta = new SFML.System.Vector2f(delta.X, -delta.Y);
                    SpinePosition = (SFML.System.Vector2f)spinePressedPosition + sDelta;
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

        protected virtual void Trigger_SpineLoaded(uint index) { }
        protected virtual void Trigger_StateUpdated() { }
        protected virtual void Trigger_MouseButtonClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseButtonDoubleClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseDragBegin(SFML.Window.MouseMoveEventArgs e) { }
        protected virtual void Trigger_MouseDragEnd(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseWheelScroll(SFML.Window.MouseWheelScrollEventArgs e) { }
        protected virtual void Trigger_WorkBegin() { }
        protected virtual void Trigger_WorkEnd() { }
        protected virtual void Trigger_SleepBegin() {  }
        protected virtual void Trigger_SleepEnd() { }
        protected virtual void Trigger_Show() { }

        private static void Resized(SpineWindow self, SFML.Window.SizeEventArgs e) { self.Resized(e); }
        private static void MouseButtonPressed(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonPressed(e); }
        private static void MouseMoved(SpineWindow self, SFML.Window.MouseMoveEventArgs e) { self.MouseMoved(e); }
        private static void MouseButtonReleased(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonReleased(e); }
        private static void MouseWheelScrolled(SpineWindow self, SFML.Window.MouseWheelScrollEventArgs e) { self.MouseWheelScrolled(e); }
    }

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
    }
}

