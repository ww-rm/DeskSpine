using Microsoft.Win32;

namespace DeskSpine
{
    public static class Program
    {
#if DEBUG
        public const string ProgramName = "DeskSpine_d";
#else
        public const string ProgramName = "DeskSpine";
#endif

        /// <summary>
        /// 程序可执行文件所在目录
        /// </summary>
        public static readonly string ProgramDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// 程序数据存放目录
        /// </summary>
        public static readonly string ProgramDataDirectory = Path.Combine(SystemValue.LocalAppdataDirectory, ProgramName);

        /// <summary>
        /// 程序资源目录
        /// </summary>
        public static readonly string ProgramResourceDirectory = Path.Combine(ProgramDirectory, "res");

        /// <summary>
        /// 程序配置文件路径
        /// </summary>
        public static readonly string ProgramConfigPath = Path.Combine(ProgramDataDirectory, "config.json");

        public static PerfMonitor.PerfMonitorForm PerfMonitorForm { get; private set; }     // 性能浮窗
        public static ConfigForm ConfigForm { get; private set; }                           // 设置窗口 (主窗口)
        public static TinyEngine.SpineRenderWindow SpineWindow { get; private set; }        // Spine 窗口
        private static Mutex programMutex;                                                  // 程序单一启动锁
        private static System.Timers.Timer configSaveTimer = new(300000) { AutoReset = true };

        /// <summary>
        /// 程序入口
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            programMutex = new Mutex(true, ProgramName, out bool createNew);
            if (!createNew)
            {
                MessageBox.Show("程序已在运行, 请勿重复启动", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(ProgramDataDirectory)) 
                Directory.CreateDirectory(ProgramDataDirectory);

            PerfMonitorForm = new() { UseLightTheme = SystemValue.SystemUseLightTheme };
            SpineWindow = new TinyEngine.SpineRenderWindow(SpineConfig.SlotCount);
            ConfigForm = new ConfigForm();
            InitFromConfig(LocalConfig);

            // 开启一个 Timer 定时保存配置
            configSaveTimer.Elapsed += (object? s, System.Timers.ElapsedEventArgs e) => LocalConfig = CurrentConfig;
            configSaveTimer.Enabled = true;

            Application.Run();
        }

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
                var exePath = $"\"{Application.ExecutablePath}\"";
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

                // 系统设置
                config.SystemConfig.AutuRun = AutoRun;
                config.SystemConfig.Visible = SpineWindow.Visible;
                config.SystemConfig.BalloonIconPath = ConfigForm.BalloonIconPath;
                config.SystemConfig.HourlyChime = ConfigForm.HourlyChime;

                // 基础设置
                var position = SpineWindow.Position;
                var size = SpineWindow.Size;
                var spPosition = SpineWindow.Spine.Position;
                config.BasicConfig.WallpaperMode = SpineWindow.WallpaperMode;
                config.BasicConfig.MouseClickThrough = SpineWindow.MouseClickThrough;
                config.BasicConfig.PositionX = position.X;
                config.BasicConfig.PositionY = position.Y;
                config.BasicConfig.SizeX = size.X;
                config.BasicConfig.SizeY = size.Y;
                config.BasicConfig.SpinePositionX = spPosition.X;
                config.BasicConfig.SpinePositionY = spPosition.Y;
                config.BasicConfig.SpineFlip = SpineWindow.Spine.Flip;
                config.BasicConfig.SpineScale = SpineWindow.Spine.Scale;
                config.BasicConfig.Opacity = SpineWindow.Opacity;
                config.BasicConfig.MaxFps = SpineWindow.MaxFps;
                config.BasicConfig.BackgroundColor = SpineWindow.BackgroudColor;
                config.BasicConfig.SpineUsePMA = SpineWindow.Spine.UsePremultipliedAlpha;

                // Spine 设置
                config.SpineConfig.SpineVersion = SpineWindow.Spine.Version;
                config.SpineConfig.InteractMode = SpineWindow.Spine.AnimatorType;
                for (int i = 0; i < SpineConfig.SlotCount; i++)
                {
                    config.SpineConfig.SetSkelPath(i, SpineWindow.Spine.GetSpineSkelPath(i));
                }

                return config;
            }

            set
            {
                // XXX: 不确定是否需要 Lock/Unlock

                // 系统设置
                AutoRun = value.SystemConfig.AutuRun;
                ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath;
                ConfigForm.HourlyChime = value.SystemConfig.HourlyChime;

                // 基础设置
                SpineWindow.WallpaperMode = value.BasicConfig.WallpaperMode;
                SpineWindow.MouseClickThrough = value.BasicConfig.MouseClickThrough;
                SpineWindow.Position = new(value.BasicConfig.PositionX, value.BasicConfig.PositionY);
                SpineWindow.Size = new(value.BasicConfig.SizeX, value.BasicConfig.SizeY);
                SpineWindow.Spine.Position = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
                SpineWindow.Spine.Flip = value.BasicConfig.SpineFlip;
                SpineWindow.Spine.Scale = value.BasicConfig.SpineScale;
                SpineWindow.Opacity = value.BasicConfig.Opacity;
                SpineWindow.MaxFps = value.BasicConfig.MaxFps;
                SpineWindow.BackgroudColor = value.BasicConfig.BackgroundColor;
                SpineWindow.Spine.UsePremultipliedAlpha = value.BasicConfig.SpineUsePMA;

                // Spine 设置
                SpineWindow.Spine.AnimatorType = value.SpineConfig.InteractMode;
                try
                {
                    SpineWindow.Spine.Version = value.SpineConfig.SpineVersion;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ConfigForm,
                        $"版本 {value.SpineConfig.SpineVersion} 加载失败\n\n{ex}",
                        "Spine 资源加载失败",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

                for (int i = 0; i < SpineConfig.SlotCount; i++)
                {
                    var skelPath = value.SpineConfig.GetSkelPath(i);
                    try 
                    { 
                        SpineWindow.Spine.SetSpine(skelPath, i); 
                    }
                    catch (Exception ex) 
                    { 
                        MessageBox.Show(ConfigForm, $"{skelPath} 加载失败\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }

                // 最后调整窗口可见性
                SpineWindow.Visible = value.SystemConfig.Visible;

                // 保存本地
                LocalConfig = CurrentConfig;
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
        private static void InitFromConfig(Config value)
        {
            // 系统配置
            ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath; 
            ConfigForm.HourlyChime = value.SystemConfig.HourlyChime;

            // 基础配置
            SpineWindow.WallpaperMode = value.BasicConfig.WallpaperMode;
            SpineWindow.MouseClickThrough = value.BasicConfig.MouseClickThrough;
            SpineWindow.Position = new(value.BasicConfig.PositionX, value.BasicConfig.PositionY);
            SpineWindow.Size = new(value.BasicConfig.SizeX, value.BasicConfig.SizeY);
            SpineWindow.Spine.Position = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
            SpineWindow.Spine.Flip = value.BasicConfig.SpineFlip;
            SpineWindow.Spine.Scale = value.BasicConfig.SpineScale;
            SpineWindow.Opacity = value.BasicConfig.Opacity;
            SpineWindow.MaxFps = value.BasicConfig.MaxFps;
            SpineWindow.BackgroudColor = value.BasicConfig.BackgroundColor;
            SpineWindow.Spine.UsePremultipliedAlpha = value.BasicConfig.SpineUsePMA;

            // Spine 设置
            SpineWindow.Spine.AnimatorType = value.SpineConfig.InteractMode;
            try 
            { 
                SpineWindow.Spine.Version = value.SpineConfig.SpineVersion; 
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(
                    ConfigForm, 
                    $"版本 {value.SpineConfig.SpineVersion} 加载失败\n\n{ex}", 
                    "Spine 资源加载失败", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
            }

            for (int i = 0; i < SpineConfig.SlotCount; i++)
            {
                var skelPath = value.SpineConfig.GetSkelPath(i);
                try 
                { 
                    SpineWindow.Spine.SetSpine(skelPath, i); 
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ConfigForm, $"{skelPath} 加载失败\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
            }

            // 最后调整窗口可见性
            SpineWindow.Visible = value.SystemConfig.Visible;
        }
    }
}
