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
        public SpineWindow()
        {
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();
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

        ~SpineWindow()
        {
            if (window is not null)
                Dispose();
        }

        protected Spine.Spine? spine;
        protected Spine.Spine? spineEx1;
        protected Spine.Spine? spineEx2;

        public string? ResFolder 
        { 
            get
            {
                string? v = null;
                mutex.WaitOne();
                if (spine is not null) v = Path.GetDirectoryName(Path.GetFullPath(spine.SkelPath));
                else if (spineEx1 is not null) v = Path.GetDirectoryName(Path.GetFullPath(spineEx1.SkelPath));
                else if (spineEx2 is not null) v = Path.GetDirectoryName(Path.GetFullPath(spineEx2.SkelPath));
                mutex.ReleaseMutex();
                return v;
            }
        }

        public abstract bool FaceToRight { get; set; }

        public float SpineScale
        {
            get 
            {
                var v = 1f;
                mutex.WaitOne();
                if (spine is not null) v = spine.Scale;
                else if (spineEx1 is not null) v = spineEx1.Scale;
                else if (spineEx2 is not null) v = spineEx2.Scale;
                mutex.ReleaseMutex(); 
                return v; 
            }
            set
            {
                mutex.WaitOne();
                if (spine is not null) { spine.Scale = value; }
                if (spineEx1 is not null) { spineEx1.Scale = value; }
                if (spineEx2 is not null) { spineEx2.Scale = value; }
                mutex.ReleaseMutex();
            }
        }

        public SFML.System.Vector2f SpinePosition
        {
            get
            {
                var v = new SFML.System.Vector2f(0, 0);
                mutex.WaitOne();
                if (spine is not null) v = new SFML.System.Vector2f(spine.X, spine.Y);
                else if (spineEx1 is not null) v = new SFML.System.Vector2f(spineEx1.X, spineEx1.Y);
                else if (spineEx2 is not null) v = new SFML.System.Vector2f(spineEx2.X, spineEx2.Y);
                mutex.ReleaseMutex();
                return v;
            }
            set
            {
                mutex.WaitOne();
                if (spine is not null) { spine.X = value.X; spine.Y = value.Y; }
                if (spineEx1 is not null) { spineEx1.X = value.X; spineEx1.Y = value.Y; }
                if (spineEx2 is not null) { spineEx2.X = value.X; spineEx2.Y = value.Y; }
                mutex.ReleaseMutex();
            }
        }

        public void LoadSpine(string version, string skelPath, string? atlasPath = null)
        {
            Debug.WriteLine($"Loading spine({version}) from {skelPath}, {atlasPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, atlasPath); }
            catch { throw; }

            mutex.WaitOne();
            spine = spineNew; // 调用方负责恢复新 spine 对象的属性
            mutex.ReleaseMutex();
            UpdateProperBackgroudColor();
            Trigger_SpineLoaded();
            Debug.Write("spine animiation: ");
            foreach (var a in spine.AnimationNames) Debug.Write($"{a}; ");
        }

        public void LoadSpineEx1(string version, string skelPath, string? atlasPath = null)
        {
            Debug.WriteLine($"Loading spineEx1({version}) from {skelPath}, {atlasPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, atlasPath); }
            catch { throw; }

            mutex.WaitOne();
            spineEx1 = spineNew; // 调用方负责恢复新 spine 对象的属性
            mutex.ReleaseMutex();
            UpdateProperBackgroudColor();
            Trigger_SpineEx1Loaded();
            Debug.Write("spineEx1 animations: ");
            foreach (var a in spineEx1.AnimationNames) Debug.Write($"{a}; ");
        }

        public void LoadSpineEx2(string version, string skelPath, string? atlasPath = null)
        {
            Debug.WriteLine($"Loading spineEx2({version}) from {skelPath}, {atlasPath}");
            Spine.Spine spineNew;
            try { spineNew = Spine.Spine.New(version, skelPath, atlasPath); }
            catch { throw; }

            mutex.WaitOne();
            spineEx2 = spineNew; // 调用方负责恢复新 spine 对象的属性
            mutex.ReleaseMutex();
            UpdateProperBackgroudColor();
            Trigger_SpineEx2Loaded();
            Debug.Write("spineEx2 animations: ");
            foreach (var a in spineEx2.AnimationNames) Debug.Write($"{a}; ");
        }

        private void UpdateProperBackgroudColor()
        {
            var colors = new Dictionary<uint, uint>();
            mutex.WaitOne();
            string? p1 = spine?.PngPath;
            string? p2 = spineEx1?.PngPath;
            string? p3 = spineEx2?.PngPath;
            mutex.ReleaseMutex();

            if (p1 is not null)
            {
                var png = new SFML.Graphics.Image(p1);
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
            if (p2 is not null)
            {
                var png = new SFML.Graphics.Image(p2);
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
            if (p3 is not null)
            {
                var png = new SFML.Graphics.Image(p3);
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
            var crKey = BinaryPrimitives.ReverseEndianness(bestColor.ToInteger());
            Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY);
        }

        protected Mutex mutex = new();
        private CancellationTokenSource cancelTokenSrc = new();
        private ManualResetEvent windowCreatedEvent = new(false);
        private SFML.System.Clock clock = new();
        private SFML.Graphics.Color clearColor = new(128, 128, 128);
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

        public void Reset()
        {
            SpinePosition = new SFML.System.Vector2f(0, 0);
            SpineScale = 1f;
            window.Position = new(0, 0);
            window.Size = new(1000, 1000);
            FixView();
        }

        private void CreateWindow()
        {
            // 创建窗口
            mutex.WaitOne();
            window = new(new(1000, 1000), "spine", SFML.Window.Styles.None);
            mutex.ReleaseMutex();

            // 设置窗口特殊属性
            var hWnd = window.SystemHandle;
            var style = Win32.GetWindowLong(hWnd, Win32.GWL_STYLE);
            Win32.SetWindowLong(hWnd, Win32.GWL_STYLE, style | Win32.WS_POPUP);
            var exStyle = Win32.GetWindowLong(hWnd, Win32.GWL_EXSTYLE);
            Win32.SetWindowLong(hWnd, Win32.GWL_EXSTYLE, exStyle | Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_TOPMOST);
            var crKey = BinaryPrimitives.ReverseEndianness(clearColor.ToInteger());
            Win32.SetLayeredWindowAttributes(hWnd, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            Win32.SetWindowPos(hWnd, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);

            // 设置窗口属性
            window.SetVisible(visible);
            window.SetFramerateLimit(maxFps);
            FixView();

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
            spine?.Update(delta);
            spineEx1?.Update(delta);
            spineEx2?.Update(delta);
            mutex.ReleaseMutex();
        }

        private void Render()
        {
            mutex.WaitOne();
            if (spineEx2 is not null) window.Draw(spineEx2);
            if (spineEx1 is not null) window.Draw(spineEx1);
            if (spine is not null) window.Draw(spine);
            mutex.ReleaseMutex();
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
        protected virtual void Trigger_SpineEx1Loaded() { }
        protected virtual void Trigger_SpineEx2Loaded() { }
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

