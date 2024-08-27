using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpineWindow
{
    public enum BackgroudColor
    {
        Black = 0,
        White = 1,
        Gray = 2
    }

    public abstract class SpineWindow
    {
        public string ResFolder { get; private set; }

        public SpineWindow(string skelPath, string? atlasPath = null, string? resFolder = null)
        {
            spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
            ResFolder = resFolder ??= Path.GetDirectoryName(spine.SkelPath);
            Trigger_SpineLoaded();

            Debug.WriteLine($"AtlasPath: {spine.AtlasPath}");
            foreach (var a in spine.AnimationNames) Debug.Write($"{a}; ");
            Debug.WriteLine("");

            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();

        }

        ~SpineWindow()
        {
            cancelTokenSrc.Cancel();
            windowLoopTask.Wait();
        }

        public abstract string SpineVersion { get; }
        public abstract bool FaceToRight { get; set; }
        protected Spine.Spine spine;

        public void LoadSpine(string skelPath, string? atlasPath = null)
        {
            mutex.WaitOne();
            try
            {
                spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
                Debug.WriteLine($"AtlasPath: {spine.AtlasPath}");
                foreach (var a in spine.AnimationNames) Debug.Write($"{a}; ");
                Debug.WriteLine("");
                clearColor = GetProperBackgroudColor(spine.PngPath, backgroudColor);
                var crKey = BinaryPrimitives.ReverseEndianness(clearColor.ToInteger());
                Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
            Trigger_SpineLoaded();
        }

        protected Mutex mutex = new();
        private CancellationTokenSource cancelTokenSrc = new();
        private ManualResetEvent windowCreatedEvent = new(false);
        private SFML.System.Clock clock = new();
        private SFML.Graphics.Color clearColor = new(128, 128, 128);
        protected SFML.Graphics.RenderWindow window;
        private Task windowLoopTask;

        private BackgroudColor backgroudColor = BackgroudColor.Gray;
        private bool visible = false;
        private uint maxFps = 30;

        private SFML.System.Vector2i? windowPressedPosition = null;
        private SFML.System.Vector2f? spinePressedPosition = null;
        private SFML.System.Clock doubleClickClock = new();
        private bool doubleClickChecking = false;
        private bool isDragging = false;

        private static SFML.Graphics.Color GetProperBackgroudColor(string pngPath, BackgroudColor backgroudColor)
        {
            var png = new SFML.Graphics.Image(pngPath);
            var colors = new HashSet<uint>();
            for (uint i = 0; i < png.Size.X; i++)
            {
                for (uint j = 0; j < png.Size.Y; j++)
                {
                    var c = png.GetPixel(i, j);
                    if (c.A <= 0)
                        continue;
                    colors.Add(c.ToInteger());
                }
            }

            var rnd = new Random();
            var bgColor = SFML.Graphics.Color.Black;
            for (int i = 0; i < 10; i++)
            {
                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                bgColor = SFML.Graphics.Color.Black;
                switch (backgroudColor)
                {
                    case BackgroudColor.Black:
                        bgColor.R = bgColor.B = (byte)rnd.Next(0, 20);
                        bgColor.G = (byte)rnd.Next(0, 20);
                        break;
                    case BackgroudColor.White:
                        bgColor.R = bgColor.B = (byte)rnd.Next(235, 255);
                        bgColor.G = (byte)rnd.Next(235, 255);
                        break;
                    case BackgroudColor.Gray:
                        bgColor.R = bgColor.B = (byte)rnd.Next(118, 138);
                        bgColor.G = (byte)rnd.Next(118, 138);
                        break;
                }
                if (!colors.Contains(bgColor.ToInteger()))
                    break;
            }

            Debug.WriteLine($"bgColor: {bgColor}");
            return bgColor;
        }

        private static void SpineWindowTask(SpineWindow self)
        {
            self.CreateWindow();
            self.WindowLoop();
        }

        public BackgroudColor BackgroudColor 
        {
            get => backgroudColor;
            set
            {
                backgroudColor = value;
                clearColor = GetProperBackgroudColor(spine.PngPath, value);
                var crKey = BinaryPrimitives.ReverseEndianness(clearColor.ToInteger());
                Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY);
            }
        }

        public byte Opacity
        {
            get
            {
                uint crKey = 0;
                byte bAlpha = 0;
                uint dwFlags = 0;
                Win32.GetLayeredWindowAttributes(window.SystemHandle, ref crKey, ref bAlpha, ref dwFlags);
                if ((dwFlags & Win32.LWA_ALPHA) != 0)
                    return bAlpha;
                else
                    return 255;
            }
            set
            {
                Win32.SetLayeredWindowAttributes(window.SystemHandle, 0, value, Win32.LWA_ALPHA);
            }
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

        public SFML.System.Vector2i Position 
        { 
            get => window.Position; 
            set => window.Position = value; 
        }

        public bool Visible
        {
            get { mutex.WaitOne(); var v = visible; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); visible = value; mutex.ReleaseMutex(); window.SetVisible(value); }
        }

        public uint MaxFps
        {
            get => maxFps;
            set { maxFps = value; window.SetFramerateLimit(value); }
        }

        public float Scale
        {
            get { mutex.WaitOne(); var v = spine.Scale; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); spine.Scale = value; mutex.ReleaseMutex(); }
        }

        public void Reset()
        {
            mutex.WaitOne();
            spine.X = spine.Y = 0;
            spine.Scale = 1;
            mutex.ReleaseMutex();
            window.Position = new(0, 0);
            window.Size = new(1000, 1000);
            FixView();
        }

        private void CreateWindow()
        {
            mutex.WaitOne();
            try
            {
                // 创建窗口
                window = new(new(1000, 1000), "spine", SFML.Window.Styles.None);

                // 设置窗口特殊属性
                var hWnd = window.SystemHandle;
                var style = Win32.GetWindowLong(hWnd, Win32.GWL_STYLE);
                Win32.SetWindowLong(hWnd, Win32.GWL_STYLE, style | Win32.WS_POPUP);
                var exStyle = Win32.GetWindowLong(hWnd, Win32.GWL_EXSTYLE);
                Win32.SetWindowLong(hWnd, Win32.GWL_EXSTYLE, exStyle | Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_TOPMOST);
                clearColor = GetProperBackgroudColor(spine.PngPath, backgroudColor);
                var crKey = BinaryPrimitives.ReverseEndianness(clearColor.ToInteger());
                Win32.SetLayeredWindowAttributes(hWnd, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
                Win32.SetWindowPos(hWnd, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);

                // 设置窗口属性
                window.SetVisible(visible);
                window.SetFramerateLimit(maxFps);
                FixView();

                // 注册窗口事件
                RegisterEvents();
            }
            finally
            {
                windowCreatedEvent.Set();
                mutex.ReleaseMutex();
            }
        }

        private void WindowLoop()
        {
            while (true)
            {
                mutex.WaitOne();
                try
                {
                    if (cancelTokenSrc.Token.IsCancellationRequested)
                    {
                        window.Close();
                        window = null;
                        cancelTokenSrc.Token.ThrowIfCancellationRequested();
                    }

                    window.DispatchEvents();
                    Update();
                    if (visible)
                    {
                        window.Clear(clearColor);
                        Render();
                        window.Display();
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private void Update()
        {
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            spine.Update(delta);
        }

        private void Render()
        {
            window.Draw(spine);
        }

        private void FixView()
        {
            var view = window.GetView();
            view.Center = new(0, 200);
            view.Size = new(window.Size.X, -window.Size.Y);
            window.SetView(view);
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
            FixView();
        }

        private void MouseButtonPressed(SFML.Window.MouseButtonEventArgs e)
        {
            // 记录点击位置
            windowPressedPosition = new(e.X, e.Y);
            spinePressedPosition = new(spine.X, spine.Y);

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
                    window.Position = window.Position + delta;
                }
                else if (rightDown && spinePressedPosition is not null)
                {
                    spine.X = ((SFML.System.Vector2f)spinePressedPosition).X + delta.X;
                    spine.Y = ((SFML.System.Vector2f)spinePressedPosition).Y - delta.Y;
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

        protected virtual void Trigger_SpineLoaded() { }
        protected virtual void Trigger_MouseButtonClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseButtonDoubleClick(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseDragBegin(SFML.Window.MouseMoveEventArgs e) { }
        protected virtual void Trigger_MouseDragEnd(SFML.Window.MouseButtonEventArgs e) { }
        protected virtual void Trigger_MouseWheelScroll(SFML.Window.MouseWheelScrollEventArgs e) { }
        protected virtual void Trigger_WorkBegin() { }
        protected virtual void Trigger_WorkEnd() { }
        protected virtual void Trigger_SleepBegin() {  }
        protected virtual void Trigger_SleepEnd() { }

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

