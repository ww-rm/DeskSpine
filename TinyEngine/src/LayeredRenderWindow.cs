using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyEngine
{
    /// <summary>
    /// 可渲染场景对象, 装有可渲染对象的容器
    /// </summary>
    public class RenderScene : Renderable 
    {
        /// <summary>
        /// 场景内对象
        /// </summary>
        public Dictionary<string, Renderable> Renderables { get => components; }
    }

    /// <summary>
    /// 分层可渲染窗口
    /// </summary>
    public abstract class LayeredRenderWindow : IDisposable
    {
        private Mutex mutex = new();                                    // 互斥锁, 用于同步临界数据
        private SFML.Graphics.RenderWindow? window;                     // SFML 窗口对象
        private Task? windowLoopTask;                                   // 窗口循环线程
        private CancellationTokenSource cancelTokenSrc = new();         // 取消令牌, 用于结束窗口线程
        private ManualResetEvent windowCreatedEvent = new(false);       // 窗口创建事件, 用于同步等待窗口创建完成
        private SFML.System.Clock clock = new();                        // 计时器, 计算每一帧的时间间隔

        private const float TimeToSleep = 300f;                         // 休眠判定超时时间
        private float lastLastInputTime = 1f;                           // 最近一次记录的用户上次输入时间间隔
        private SFML.System.Clock doubleClickClock = new();             // 双击行为计时器
        private bool doubleClickChecking = false;                       // 是否处于双击检测中
        private SFML.System.Vector2i? windowPressedPosition = null;     // 记录点击时的窗口点击位置
        private SFML.System.Vector2i? lastWindowMovedPosition = null;   // 上一次移动鼠标的位置
        private bool isDragging = false;                                // 是否处于拖动状态

        protected RenderScene scene = new();                            // 场景对象, 管理所有可渲染对象

        /// <summary>
        /// LayeredRenderWindow 基类, 提供 Layered RenderWindow 的功能封装
        /// </summary>
        public LayeredRenderWindow()
        {
            windowCreatedEvent.Reset();
            windowLoopTask = Task.Run(WindowLoopTask);
            windowCreatedEvent.WaitOne();
        }

        /// <summary>
        /// 关闭窗口并释放所有资源
        /// </summary>
        public void Close() { Dispose(); }
        ~LayeredRenderWindow() { Dispose(false); }
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (window is not null)
            {
                cancelTokenSrc.Cancel();
                windowLoopTask.Wait();

                mutex.Dispose();
                window.Dispose();
                windowLoopTask.Dispose();
                cancelTokenSrc.Dispose();
                windowCreatedEvent.Dispose();
                clock.Dispose();

                window = null;
                windowLoopTask = null;
            }
        }

        /// <summary>
        /// 创建窗口并进入主循环
        /// </summary>
        private void WindowLoopTask()
        {
            CreateWindow();
            while (true)
            {
                if (cancelTokenSrc.Token.IsCancellationRequested)
                {
                    window.Close();
                    break;
                }

                // 操作都在锁内进行
                mutex.WaitOne();

                window.DispatchEvents();
                if (visible)
                {
                    UpdateFrame();
                    RenderFrame();
                }
                else
                {
                    Thread.Sleep(33);   // 防止空转占用较高 cpu
                }

                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 创建并初始化窗口属性
        /// </summary>
        private void CreateWindow()
        {
            // 创建窗口
            window = new(new(1000, 1000), "SFML.RenderWindow.DeskSpine", SFML.Window.Styles.None);

            // 设置窗口属性默认值
            var hWnd = window.SystemHandle;
            var style = Win32.GetWindowLong(hWnd, Win32.GWL_STYLE) | Win32.WS_POPUP;
            var exStyle = Win32.GetWindowLong(hWnd, Win32.GWL_EXSTYLE) | Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_TOPMOST;
            Win32.SetWindowLong(hWnd, Win32.GWL_STYLE, style);
            Win32.SetWindowLong(hWnd, Win32.GWL_EXSTYLE, exStyle);
            Win32.SetLayeredWindowAttributes(hWnd, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
            Win32.SetWindowPos(hWnd, Win32.HWND_TOPMOST, 0, 0, 0, 0, Win32.SWP_NOMOVE | Win32.SWP_NOSIZE);
            window.SetVisible(visible);
            window.SetFramerateLimit(maxFps);
            var view = window.GetView();
            view.Center = new(0, 200);
            view.Size = new(window.Size.X, -window.Size.Y); // SFML 窗口 y 轴默认向下
            window.SetView(view);

            // 注册窗口事件
            RegisterEvents();

            // 发送通知窗口创建完成
            windowCreatedEvent.Set();
        }

        /// <summary>
        /// 逻辑帧更新
        /// </summary>
        private void UpdateFrame()
        {
            // 检测睡眠事件
            var lastInputTime = (float)Win32.GetLastInputElapsedTime().TotalSeconds;
            if (lastInputTime >= TimeToSleep && lastLastInputTime < TimeToSleep)
                scene.SleepStateChange(true);
            else if (lastInputTime < TimeToSleep && lastLastInputTime >= TimeToSleep)
                scene.SleepStateChange(false);
            lastLastInputTime = lastInputTime;

            // 更新内部对象状态
            var delta = clock.ElapsedTime.AsSeconds();
            clock.Restart();
            scene.Update(delta);
        }

        /// <summary>
        /// 渲染帧更新
        /// </summary>
        private void RenderFrame()
        {
            window.Clear(backgroundColor);
            scene.Render(window);
            window.Display();
        }

        /// <summary>
        /// 锁定线程资源
        /// </summary>
        public void Lock() { mutex.WaitOne(); }

        /// <summary>
        /// 解锁线程资源
        /// </summary>
        public void Unlock() { mutex.ReleaseMutex(); }

        /// <summary>
        /// 窗口句柄
        /// </summary>
        public IntPtr Handle { get => window.SystemHandle; }

        /// <summary>
        /// 窗口位置
        /// </summary>
        public SFML.System.Vector2i Position
        {
            get => window.Position;
            set => window.Position = value;
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
        /// 窗口背景色, 要被透明化的底色
        /// </summary>
        public SFML.Graphics.Color BackgroudColor
        {
            get => backgroundColor;
            set
            {
                // BUG: SetLayeredWindowAttributes 的 R 和 B 分量必须相等才能让背景部分的透明和穿透同时生效
                // https://stackoverflow.com/questions/76415771/setlayeredwindowattributes-click-through-only-works-with-specific-colors
                value.B = value.R; value.A = 0;
                if (value == backgroundColor) return;

                backgroundColor = value;
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
            get => visible;
            set { if (value == visible) return; visible = value; window.SetVisible(value); scene.VisibleChange(value); }
        }
        private bool visible = false;

        /// <summary>
        /// 最大帧率
        /// </summary>
        public uint MaxFps
        {
            get => maxFps;
            set { if (value == maxFps) return; maxFps = value; window.SetFramerateLimit(value); }
        }
        private uint maxFps = 30;

        /// <summary>
        /// 窗口整体透明度
        /// </summary>
        public byte Opacity
        {
            get
            {
                uint crKey = 0; byte bAlpha = 0; uint dwFlags = 0;
                Win32.GetLayeredWindowAttributes(window.SystemHandle, ref crKey, ref bAlpha, ref dwFlags);
                return ((dwFlags & Win32.LWA_ALPHA) != 0) ? bAlpha : (byte)255;
            }
            set { if (value == Opacity) return; Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, value, Win32.LWA_COLORKEY | Win32.LWA_ALPHA); }
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
                if (value && (exStyle & Win32.WS_EX_TRANSPARENT) == 0)
                {
                    exStyle |= Win32.WS_EX_TRANSPARENT;
                    Win32.SetWindowLong(window.SystemHandle, Win32.GWL_EXSTYLE, exStyle);
                }
                else if (!value && (exStyle & Win32.WS_EX_TRANSPARENT) != 0)
                {
                    exStyle &= ~Win32.WS_EX_TRANSPARENT;
                    Win32.SetWindowLong(window.SystemHandle, Win32.GWL_EXSTYLE, exStyle);
                }
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
                if (value == WallpaperMode) return;

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

                    var opacity = Opacity;
                    Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, 255, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);
                    Win32.SetParent(hWnd, workerW);
                    Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);

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
                    Win32.SetLayeredWindowAttributes(window.SystemHandle, crKey, opacity, Win32.LWA_COLORKEY | Win32.LWA_ALPHA);

                    // 触发一次内部窗口大小修改逻辑
                    var s = window.Size;
                    window.Size = new(s.X + 1, s.Y + 1);
                    window.Size = s;
                }
            }
        }

        /// <summary>
        /// 注册所有窗口事件
        /// </summary>
        private void RegisterEvents()
        {
            window.Resized += (object? s, SFML.Window.SizeEventArgs e) => Window_Resized(e);
            window.MouseButtonPressed += (object? s, SFML.Window.MouseButtonEventArgs e) => Window_MouseButtonPressed(e);
            window.MouseMoved += (object? s, SFML.Window.MouseMoveEventArgs e) => Window_MouseMoved(e);
            window.MouseButtonReleased += (object? s, SFML.Window.MouseButtonEventArgs e) => Window_MouseButtonReleased(e);
            window.MouseWheelScrolled += (object? s, SFML.Window.MouseWheelScrollEventArgs e) => Window_MouseWheelScrolled(e);
        }

        /********************************* 窗口输入事件 *********************************/

        private void Window_Resized(SFML.Window.SizeEventArgs e)
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
            }
        }

        private void Window_MouseButtonPressed(SFML.Window.MouseButtonEventArgs e)
        {
            // 记录点击位置
            windowPressedPosition = new(e.X, e.Y);
            lastWindowMovedPosition = new(e.X, e.Y);

            // 检查双击超时
            if (doubleClickChecking && doubleClickClock.ElapsedTime.AsMilliseconds() > Win32.GetDoubleClickTime())
                doubleClickChecking = false;

            // 事件处理, 按下时动作不完整不做任何处理
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

        private void Window_MouseMoved(SFML.Window.MouseMoveEventArgs e)
        {
            // 计算距离点击源点移动距离
            var windowDelta = new SFML.System.Vector2i(0, 0);
            var worldDelta = new SFML.System.Vector2f(0, 0);
            var windowDst = new SFML.System.Vector2i(e.X, e.Y);
            if (windowPressedPosition is not null)
            {
                var windowSrc = (SFML.System.Vector2i)windowPressedPosition;
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
                scene.DragBegin(leftDown ? SFML.Window.Mouse.Button.Left : SFML.Window.Mouse.Button.Right); // 左键优先级高于右键
            }

            if (isDragging)
            {
                // 计算世界内拖动距离
                var worldSmallDelta = new SFML.System.Vector2f(0, 0);
                if (lastWindowMovedPosition is not null)
                {
                    var src = window.MapPixelToCoords((SFML.System.Vector2i)lastWindowMovedPosition);
                    var dst = window.MapPixelToCoords(windowDst);
                    worldSmallDelta = dst - src;
                }
                lastWindowMovedPosition = windowDst;

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
                // 否则右键被按下则触发子类拖动事件
                if (leftDown)
                {
                    Position = Position + windowDelta;
                }

                scene.Drag(leftDown ? SFML.Window.Mouse.Button.Left : SFML.Window.Mouse.Button.Right, worldSmallDelta, worldDelta);
            }
        }

        private void Window_MouseButtonReleased(SFML.Window.MouseButtonEventArgs e)
        {
            // 清除位置记录
            windowPressedPosition = null;
            lastWindowMovedPosition = null;

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
                scene.DragEnd(e.Button);
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

                    scene.DoubleClick(e.Button);
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

                    scene.Click(e.Button);
                }
            }
        }

        private void Window_MouseWheelScrolled(SFML.Window.MouseWheelScrollEventArgs e)
        {
            scene.Scroll(e.Wheel, e.Delta);
        }
    }
}
