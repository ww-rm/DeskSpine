using System.Buffers.Binary;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace SpineWindow
{
    public abstract partial class SpineWindow
    {
        /// <summary>
        /// Spine 动画窗口
        /// </summary>
        /// <param name="skelPath">skel 文件路径</param>
        /// <param name="atlasPath">atlas 文件路径</param>
        public SpineWindow(string skelPath, string? atlasPath = null)
        {
            spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(() => SpineWindowTask(this), cancelTokenSrc.Token);
            windowCreatedEvent.WaitOne();
        }

        /// <summary>
        /// 析构函数, 停止窗口
        /// </summary>
        ~SpineWindow()
        {
            cancelTokenSrc.Cancel();
            windowLoopTask.Wait();
        }

        /// <summary>
        /// Spine 版本
        /// </summary>
        public abstract string SpineVersion { get; }

        /// <summary>
        /// spine 对象
        /// </summary>
        protected Spine.Spine spine;

        /// <summary>
        /// 加载 Spine 资源
        /// </summary>
        /// <param name="skelPath">skel 文件路径</param>
        /// <param name="atlasPath">atlas 文件路径</param>
        public void LoadSpine(string skelPath, string? atlasPath = null)
        {
            mutex.WaitOne();
            try
            {
                spine = Spine.Spine.New(SpineVersion, skelPath, atlasPath);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 窗口对象
        /// </summary>
        protected SFML.Graphics.RenderWindow window;

        /// <summary>
        /// 互斥锁, 子类访问内部数据时必须通过锁同步
        /// </summary>
        protected Mutex mutex = new();

        /// <summary>
        /// 取消令牌
        /// </summary>
        private CancellationTokenSource cancelTokenSrc = new();

        /// <summary>
        /// 窗口创建通知事件
        /// </summary>
        private ManualResetEvent windowCreatedEvent = new(false);

        /// <summary>
        /// 计时器
        /// </summary>
        private SFML.System.Clock clock = new();

        /// <summary>
        /// 窗口主循环线程
        /// </summary>
        private Task windowLoopTask;

        /// <summary>
        /// 窗口是否可见
        /// </summary>
        public bool Visible
        {
            get { mutex.WaitOne(); var v = _Visible; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); _Visible = value; mutex.ReleaseMutex(); window.SetVisible(value); }
        }
        private bool _Visible = true;

        /// <summary>
        /// 不透明度, 取值范围 0-1
        /// </summary>
        public float Opacity
        {
            get { mutex.WaitOne(); var v = _Opacity; mutex.ReleaseMutex(); return v; }
            set
            {
                mutex.WaitOne();
                _Opacity = Math.Clamp(value, 0, 1);
                mutex.ReleaseMutex();
                Win32.SetLayeredWindowAttributes(
                    window.SystemHandle,
                    BinaryPrimitives.ReverseEndianness(_TransparencyKey.ToInteger()),
                    (byte)(255 * _Opacity),
                    Win32.LWA_COLORKEY | Win32.LWA_ALPHA
                );
            }
        }
        private float _Opacity = 1f;

        /// <summary>
        /// 用作抠透明背景的 RGB 颜色
        /// </summary>
        public SFML.Graphics.Color TransparencyKey
        {
            get { mutex.WaitOne(); var v = _TransparencyKey; mutex.ReleaseMutex(); return v; }
            set
            {
                mutex.WaitOne();
                _TransparencyKey = value;
                mutex.ReleaseMutex();
                Win32.SetLayeredWindowAttributes(
                    window.SystemHandle,
                    BinaryPrimitives.ReverseEndianness(_TransparencyKey.ToInteger()),
                    (byte)(255 * _Opacity),
                    Win32.LWA_COLORKEY | Win32.LWA_ALPHA
                );
            }
        }
        private SFML.Graphics.Color _TransparencyKey = new SFML.Graphics.Color(128, 128, 128);

        /// <summary>
        /// 最大帧率
        /// </summary>
        public uint MaxFps
        {
            get { mutex.WaitOne(); var v = _MaxFps; mutex.ReleaseMutex(); return v; }
            set { mutex.WaitOne(); _MaxFps = value; mutex.ReleaseMutex(); window.SetFramerateLimit(value); }
        }
        private uint _MaxFps = 30;

        private SFML.System.Vector2i? pressedPosition = null;

        /// <summary>
        /// 窗口任务, 窗口创建和窗口循环必须在同一个线程完成
        /// </summary>
        private static void SpineWindowTask(SpineWindow self)
        {
            self.CreateWindow();
            self.WindowLoop();
        }

        /// <summary>
        /// 创建窗口
        /// </summary>
        private void CreateWindow()
        {
            mutex.WaitOne();
            try
            {
                // 创建窗口
                window = new SFML.Graphics.RenderWindow(
                    new SFML.Window.VideoMode(512, 512),
                    "spine",
                    SFML.Window.Styles.Default
                 );

                // 设置分层属性
                var handle = window.SystemHandle;
                var exStyle = Win32.GetWindowLong(handle, Win32.GWL_EXSTYLE);
                Win32.SetWindowLong(handle, Win32.GWL_EXSTYLE, exStyle | Win32.WS_EX_LAYERED);
                Win32.SetLayeredWindowAttributes(
                    handle,
                    BinaryPrimitives.ReverseEndianness(_TransparencyKey.ToInteger()),
                    (byte)(255 * _Opacity),
                    Win32.LWA_ALPHA
                );

                // 设置窗口属性
                window.SetVisible(_Visible);
                window.SetFramerateLimit(_MaxFps);
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

        /// <summary>
        /// 窗口循环
        /// </summary>
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
                    if (_Visible)
                    {
                        window.Clear(_TransparencyKey);
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

        /// <summary>
        /// 更新状态
        /// </summary>
        private void Update()
        {
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            spine.Update(delta);
        }

        /// <summary>
        /// 渲染画面
        /// </summary>
        private void Render()
        {
            window.Draw(spine);
        }

        /// <summary>
        /// 修正窗口视窗
        /// </summary>
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

