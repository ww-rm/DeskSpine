using System.Buffers.Binary;
using System.Diagnostics;
using System.Reflection.Metadata;
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
        public abstract string SpineVersion { get; }
        protected Spine.Spine spine;

        protected Mutex mutex = new();
        private CancellationTokenSource cancelTokenSrc = new();
        private ManualResetEvent windowCreatedEvent = new(false);
        private SFML.System.Clock clock = new();
        protected SFML.Graphics.RenderWindow window;
        private Task windowLoopTask;

        private BackgroudColor backgroudColor = BackgroudColor.Gray;
        private SFML.Graphics.Color transparencyKey = new(128, 128, 128);
        private uint crKey { get => BinaryPrimitives.ReverseEndianness(transparencyKey.ToInteger()); }
        private bool visible = true;
        private byte opacity = 255;
        private uint maxFps = 30;

        private SFML.System.Vector2i? pressedPosition = null;

        protected static SFML.Graphics.Color GetProperBackgroudColor(string pngPath, BackgroudColor backgroudColor)
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
                bgColor = SFML.Graphics.Color.Black;
                switch (backgroudColor)
                {
                    case BackgroudColor.Black:
                        bgColor.R = (byte)rnd.Next(0, 20);
                        bgColor.G = (byte)rnd.Next(0, 20);
                        bgColor.B = (byte)rnd.Next(0, 20);
                        break;
                    case BackgroudColor.White:
                        bgColor.R = (byte)rnd.Next(235, 255);
                        bgColor.G = (byte)rnd.Next(235, 255);
                        bgColor.B = (byte)rnd.Next(235, 255);
                        break;
                    case BackgroudColor.Gray:
                        bgColor.R = (byte)rnd.Next(118, 138);
                        bgColor.G = (byte)rnd.Next(118, 138);
                        bgColor.B = (byte)rnd.Next(118, 138);
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

        public SpineWindow(string skelPath, string? atlasPath = null)
        {
            spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
            transparencyKey = GetProperBackgroudColor(spine.PngPath, backgroudColor);
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();
        }

        ~SpineWindow()
        {
            cancelTokenSrc.Cancel();
            windowLoopTask.Wait();
        }

        public void LoadSpine(string skelPath, string? atlasPath = null)
        {
            mutex.WaitOne();
            try
            {
                spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
                transparencyKey = GetProperBackgroudColor(spine.PngPath, backgroudColor);
                Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public BackgroudColor BackgroudColor 
        {
            get => backgroudColor;
            set
            {
                backgroudColor = value;
                transparencyKey = GetProperBackgroudColor(spine.PngPath, value);
                Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            }
        }

        public byte Opacity
        {
            get => opacity;
            set { opacity = value; Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, value, Win32.LWA_COLORKEY | Win32.LWA_ALPHA); }
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

        private void CreateWindow()
        {
            mutex.WaitOne();
            try
            {
                // 创建窗口
                window = new SFML.Graphics.RenderWindow(new SFML.Window.VideoMode(512, 512), "spine", SFML.Window.Styles.Default);

                // 设置分层属性
                var hWnd = window.SystemHandle;
                var exStyle = Win32.GetWindowLong(hWnd, Win32.GWL_EXSTYLE);
                Win32.SetWindowLong(hWnd, Win32.GWL_EXSTYLE, exStyle | Win32.WS_EX_LAYERED);
                Win32.SetLayeredWindowAttributes(hWnd, crKey, opacity, Win32.LWA_ALPHA);

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
                        window.Clear(transparencyKey);
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

        public void FixView()
        {
            var view = window.GetView();
            view.Center = new SFML.System.Vector2f(0, 200);
            view.Size = new SFML.System.Vector2f(window.Size.X, -window.Size.Y);
            window.SetView(view);
        }

        private void RegisterEvents()
        {
            window.Resized += (object? s, SFML.Window.SizeEventArgs e) => Resized(this, e);
            window.MouseButtonPressed += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonPressed(this, e);
            window.MouseMoved += (object? s, SFML.Window.MouseMoveEventArgs e) => MouseMoved(this, e);
            window.MouseButtonReleased += (object? s, SFML.Window.MouseButtonEventArgs e) => MouseButtonReleased(this, e);
        }

        private void Resized(SFML.Window.SizeEventArgs e)
        {
            FixView();
        }

        private void MouseButtonPressed(SFML.Window.MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case SFML.Window.Mouse.Button.Left:
                    pressedPosition = new SFML.System.Vector2i(e.X, e.Y);
                    break;
                default:
                    break;
            }
        }

        private void MouseMoved(SFML.Window.MouseMoveEventArgs e)
        {
            if (SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left))
            {
                if (pressedPosition is not null)
                {
                    window.Position = (SFML.System.Vector2i)(window.Position + new SFML.System.Vector2i(e.X, e.Y) - pressedPosition);
                }
            }
        }

        private void MouseButtonReleased(SFML.Window.MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case SFML.Window.Mouse.Button.Left:
                    pressedPosition = null;
                    break;
                default:
                    break;
            }
        }

        private static void Resized(SpineWindow self, SFML.Window.SizeEventArgs e) { self.Resized(e); }
        private static void MouseButtonPressed(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonPressed(e); }
        private static void MouseMoved(SpineWindow self, SFML.Window.MouseMoveEventArgs e) { self.MouseMoved(e); }
        private static void MouseButtonReleased(SpineWindow self, SFML.Window.MouseButtonEventArgs e) { self.MouseButtonReleased(e); }
    }

    internal static class Win32
    {
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const uint LWA_COLORKEY = 0x1;
        public const uint LWA_ALPHA = 0x2;

        // Windows API functions and constants
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    }
}

