using Microsoft.Win32;
using SpineWindow;

namespace DeskSpine
{
    public static class Program
    {
#if DEBUG
        public const string ProgramName = "DeskSpine_d";
#else
        public const string ProgramName = "DeskSpine";
#endif
        public static string LocalAppdataDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ProgramDirectory { get; } = Path.GetDirectoryName(Application.ExecutablePath);
        public static string ProgramDataDirectory { get; } = Path.Combine(LocalAppdataDirectory, ProgramName);
        public static string ProgramConfigPath { get; } = Path.Combine(ProgramDataDirectory, "config.json");

        public static ConfigForm ConfigForm { get; private set; }                   // 设置窗口 (主窗口)
        public static SpineWindow.SpineWindow WindowSpine { get; private set; }     // Spine 窗口
        private static Mutex programMutex;                                          // 程序单一启动锁

        /// <summary>
        /// 是否开机自启
        /// </summary>
        public static bool AutoRun
        {
            get
            {
                var exePath = $"\"{Application.ExecutablePath}\"";
                var ret = false;
                using (RegistryKey runKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                    ret = runKey.GetValue(ProgramName, "").ToString() == exePath;
                return ret;
            }
            set
            {
                var exePath = Application.ExecutablePath;
                using (RegistryKey runKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                {
                    if (value)
                        runKey.SetValue(ProgramName, exePath);
                    else
                        runKey.DeleteValue(ProgramName, false);
                }
            }
        }

        /// <summary>
        /// 根据程序实时运行状态获取配置
        /// </summary>
        public static Config CurrentConfig
        {
            get
            {
                var config = new Config();

                // 保存到 json 里的值

                config.SystemConfig.Visible = WindowSpine.Visible;

                config.BasicConfig.WallpaperMode = WindowSpine.WallpaperMode;
                config.BasicConfig.MouseClickThrough = WindowSpine.MouseClickThrough;
                config.BasicConfig.SpineScale = WindowSpine.SpineScale;
                config.BasicConfig.SpineFlip = WindowSpine.SpineFlip;
                config.BasicConfig.Opacity = WindowSpine.Opacity;
                config.BasicConfig.MaxFps = WindowSpine.MaxFps;
                config.BasicConfig.AutoBackgroudColor = WindowSpine.AutoBackgroudColor;
                config.BasicConfig.BackgroundColor = WindowSpine.BackgroudColor;
                config.BasicConfig.SpineUsePMA = WindowSpine.SpineUsePMA;

                config.SpineConfig.SpineVersion = WindowSpine.SpineVersion;
                config.SpineConfig.WindowType = WindowSpine.Type;
                for (int i = 0; i < WindowSpine.SlotCount; i++)
                {
                    config.SpineConfig.SetSkelPath(i, WindowSpine.GetSpineSkelPath(i));
                }

                // 保存在注册表的里的值

                config.SystemConfig.AutuRun = AutoRun;
                var position = WindowSpine.Position;
                config.BasicConfig.PositionX = position.X;
                config.BasicConfig.PositionY = position.Y;
                var size = WindowSpine.Size;
                config.BasicConfig.SizeX = size.X;
                config.BasicConfig.SizeY = size.Y;
                var spPosition = WindowSpine.SpinePosition;
                config.BasicConfig.SpinePositionX = spPosition.X;
                config.BasicConfig.SpinePositionY = spPosition.Y;

                return config;
            }

            set
            {
                var cur = CurrentConfig;

                // 优先检测是否需要更换窗口类型, 重新创建窗口实例之后再设置其他配置
                if (cur.SpineConfig.WindowType != value.SpineConfig.WindowType)
                {
                    WindowSpine.Dispose();
                    WindowSpine = SpineWindow.SpineWindow.New(value.SpineConfig.WindowType, SpineConfig.SlotCount);
                }

                // 检查是否需要更换资源
                if (cur.SpineConfig.SpineVersion != value.SpineConfig.SpineVersion)
                {
                    for (int i = 0; i < WindowSpine.SlotCount; i++)
                    {
                        WindowSpine.UnloadSpine(i);
                        var skelPath = value.SpineConfig.GetSkelPath(i);
                        if (!string.IsNullOrEmpty(skelPath))
                        {
                            try { WindowSpine.LoadSpine(value.SpineConfig.SpineVersion, skelPath, index: i); }
                            catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < WindowSpine.SlotCount; i++)
                    {
                        if (cur.SpineConfig.GetSkelPath(i) != value.SpineConfig.GetSkelPath(i))
                        {
                            WindowSpine.UnloadSpine(i);
                            var skelPath = value.SpineConfig.GetSkelPath(i);
                            if (!string.IsNullOrEmpty(skelPath))
                            {
                                try { WindowSpine.LoadSpine(value.SpineConfig.SpineVersion, skelPath, index: i); }
                                catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                            }
                        }
                    }
                }

                // 重新取一次现在的运行时配置
                cur = CurrentConfig;

                // 系统设置
                if (cur.SystemConfig.AutuRun != value.SystemConfig.AutuRun)
                    AutoRun = value.SystemConfig.AutuRun;

                // 基础设置
                if (cur.BasicConfig.WallpaperMode != value.BasicConfig.WallpaperMode)
                    WindowSpine.WallpaperMode = value.BasicConfig.WallpaperMode;
                if (cur.BasicConfig.MouseClickThrough != value.BasicConfig.MouseClickThrough)
                    WindowSpine.MouseClickThrough = value.BasicConfig.MouseClickThrough;
                if (cur.BasicConfig.PositionX != value.BasicConfig.PositionX || cur.BasicConfig.PositionY != value.BasicConfig.PositionY)
                    WindowSpine.Position = new(value.BasicConfig.PositionX, value.BasicConfig.PositionY);
                if (cur.BasicConfig.SizeX != value.BasicConfig.SizeX || cur.BasicConfig.SizeY != value.BasicConfig.SizeY)
                    WindowSpine.Size = new(value.BasicConfig.SizeX, value.BasicConfig.SizeY);
                if (cur.BasicConfig.Opacity != value.BasicConfig.Opacity)
                    WindowSpine.Opacity = value.BasicConfig.Opacity;
                if (cur.BasicConfig.MaxFps != value.BasicConfig.MaxFps)
                    WindowSpine.MaxFps = value.BasicConfig.MaxFps;
                if (cur.BasicConfig.AutoBackgroudColor != value.BasicConfig.AutoBackgroudColor)
                    WindowSpine.AutoBackgroudColor = value.BasicConfig.AutoBackgroudColor;
                if (cur.BasicConfig.BackgroundColor != value.BasicConfig.BackgroundColor)
                    WindowSpine.BackgroudColor = value.BasicConfig.BackgroundColor;     // 要先设置 AutoBackgroudColor 再设置 BackgroudColor

                // scale 的设置有代价, 并且会重置动画, 所以有改动时再设置
                if (Math.Abs(cur.BasicConfig.SpineScale - value.BasicConfig.SpineScale) > 1e-3)
                    WindowSpine.SpineScale = value.BasicConfig.SpineScale;
                WindowSpine.SpinePosition = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
                WindowSpine.SpineFlip = value.BasicConfig.SpineFlip;
                WindowSpine.SpineUsePMA = value.BasicConfig.SpineUsePMA;

                // 最后设置窗口可见性
                if (cur.SystemConfig.Visible != value.SystemConfig.Visible)
                    WindowSpine.Visible = value.SystemConfig.Visible;

                // 保存本地
                LocalConfig = value;
            }
        }

        /// <summary>
        /// 获取和保存本地配置
        /// </summary>
        public static Config LocalConfig
        {
            get { var v = new Config(); v.Load(ProgramConfigPath); return v; }
            set => value.Save(ProgramConfigPath);
        }

        /// <summary>
        /// 从指定的配置进行初始化
        /// </summary>
        private static void InitFromConfig(Config config)
        {
            // 不需要管注册表里存储的信息, 会在内部自动生效

            // 系统配置

            // 基础配置
            WindowSpine = SpineWindow.SpineWindow.New(config.SpineConfig.WindowType, SpineConfig.SlotCount);
            WindowSpine.WallpaperMode = config.BasicConfig.WallpaperMode;
            WindowSpine.MouseClickThrough = config.BasicConfig.MouseClickThrough;
            WindowSpine.Opacity = config.BasicConfig.Opacity;
            WindowSpine.MaxFps = config.BasicConfig.MaxFps;
            WindowSpine.AutoBackgroudColor = config.BasicConfig.AutoBackgroudColor;
            WindowSpine.BackgroudColor = config.BasicConfig.BackgroundColor;   // 要先设置 AutoBackgroudColor 再设置 BackgroudColor

            // 加载 Spine 资源
            var spVersion = config.SpineConfig.SpineVersion;
            for (int i = 0; i < WindowSpine.SlotCount; i++)
            {
                var skelPath = config.SpineConfig.GetSkelPath(i);
                if (!string.IsNullOrEmpty(skelPath))
                {
                    try { WindowSpine.LoadSpine(spVersion, skelPath, index: i); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                }
            }

            // 加载完资源之后才设置 Spine 相关参数
            WindowSpine.SpineScale = config.BasicConfig.SpineScale;
            WindowSpine.SpineFlip = config.BasicConfig.SpineFlip;
            WindowSpine.SpineUsePMA = config.BasicConfig.SpineUsePMA;

            // 设置窗口可见状态
            WindowSpine.Visible = config.SystemConfig.Visible;
        }

        /// <summary>
        /// 程序入口
        /// </summary>
        [STAThread]
        public static void Main()
        {
            programMutex = new Mutex(true, ProgramName, out bool createNew);
            if (!createNew)
            {
                MessageBox.Show("程序已在运行, 请勿重复启动", ProgramName);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (!Directory.Exists(ProgramDataDirectory))
                Directory.CreateDirectory(ProgramDataDirectory);
            InitFromConfig(LocalConfig);
            ConfigForm = new ConfigForm();
            Application.Run();
        }
    }
}
