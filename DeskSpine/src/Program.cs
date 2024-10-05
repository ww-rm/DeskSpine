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
        public static SpineWindow.SpineRenderWindow spineWindow { get; private set; }       // Spine 窗口
        private static Mutex programMutex;                                                  // 程序单一启动锁

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
            ConfigForm = new ConfigForm();
            InitFromConfig(LocalConfig);

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

                // 保存到 json 里的值

                config.SystemConfig.Visible = spineWindow.Visible;
                config.SystemConfig.BalloonIconPath = ConfigForm.BalloonIconPath;
                config.SystemConfig.TimeAlarm = ConfigForm.TimeAlarm;

                config.BasicConfig.WallpaperMode = spineWindow.WallpaperMode;
                config.BasicConfig.MouseClickThrough = spineWindow.MouseClickThrough;
                config.BasicConfig.SpineScale = spineWindow.SpineScale;
                config.BasicConfig.SpineFlip = spineWindow.SpineFlip;
                config.BasicConfig.Opacity = spineWindow.Opacity;
                config.BasicConfig.MaxFps = spineWindow.MaxFps;
                config.BasicConfig.AutoBackgroudColor = spineWindow.AutoBackgroudColor;
                config.BasicConfig.BackgroundColor = spineWindow.BackgroudColor;
                config.BasicConfig.SpineUsePMA = spineWindow.SpineUsePMA;

                config.SpineConfig.SpineVersion = spineWindow.SpineVersion;
                config.SpineConfig.WindowType = spineWindow.Type;
                for (int i = 0; i < spineWindow.SlotCount; i++)
                {
                    config.SpineConfig.SetSkelPath(i, spineWindow.GetSpineSkelPath(i));
                }

                // 保存在注册表的里的值

                config.SystemConfig.AutuRun = AutoRun;
                var position = spineWindow.Position;
                config.BasicConfig.PositionX = position.X;
                config.BasicConfig.PositionY = position.Y;
                var size = spineWindow.Size;
                config.BasicConfig.SizeX = size.X;
                config.BasicConfig.SizeY = size.Y;
                var spPosition = spineWindow.SpinePosition;
                config.BasicConfig.SpinePositionX = spPosition.X;
                config.BasicConfig.SpinePositionY = spPosition.Y;

                return config;
            }

            set
            {
                // 系统设置
                AutoRun = value.SystemConfig.AutuRun;
                ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath;
                ConfigForm.TimeAlarm = value.SystemConfig.TimeAlarm;

                // 优先检测是否需要更换窗口类型, 重新创建窗口实例之后再设置其他配置
                if (spineWindow.Type != value.SpineConfig.WindowType)
                {
                    spineWindow.Close();
                    spineWindow = SpineWindow.SpineRenderWindow.New(value.SpineConfig.WindowType, SpineConfig.SlotCount);
                }

                // 窗口设置
                spineWindow.WallpaperMode = value.BasicConfig.WallpaperMode;
                spineWindow.MouseClickThrough = value.BasicConfig.MouseClickThrough;
                spineWindow.Position = new(value.BasicConfig.PositionX, value.BasicConfig.PositionY);
                spineWindow.Size = new(value.BasicConfig.SizeX, value.BasicConfig.SizeY);
                spineWindow.Opacity = value.BasicConfig.Opacity;
                spineWindow.MaxFps = value.BasicConfig.MaxFps;
                spineWindow.BackgroudColor = value.BasicConfig.BackgroundColor;         // 要先设置 BackgroudColor 再设置 AutoBackgroudColor
                spineWindow.AutoBackgroudColor = value.BasicConfig.AutoBackgroudColor;

                // 检查是否需要更换资源
                for (int i = 0; i < spineWindow.SlotCount; i++)
                {
                    var skelPath = value.SpineConfig.GetSkelPath(i);
                    if (string.IsNullOrEmpty(skelPath))
                    {
                        spineWindow.UnloadSpine(i);
                    }
                    else
                    {
                        try { spineWindow.LoadSpine(value.SpineConfig.SpineVersion, skelPath, i); }
                        catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                }

                // 精灵设置
                spineWindow.SpineScale = value.BasicConfig.SpineScale;
                spineWindow.SpinePosition = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
                spineWindow.SpineFlip = value.BasicConfig.SpineFlip;
                spineWindow.SpineUsePMA = value.BasicConfig.SpineUsePMA;
                spineWindow.Visible = value.SystemConfig.Visible;

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
        private static void InitFromConfig(Config config)
        {
            // 不需要管注册表里存储的信息, 会在内部自动生效

            // 系统配置
            try { ConfigForm.BalloonIconPath = config.SystemConfig.BalloonIconPath; }
            catch (Exception ex) { MessageBox.Show($"{config.SystemConfig.BalloonIconPath} 加载失败\n\n{ex}", "气泡图标资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            ConfigForm.TimeAlarm = config.SystemConfig.TimeAlarm;

            // 基础配置
            spineWindow = SpineWindow.SpineRenderWindow.New(config.SpineConfig.WindowType, SpineConfig.SlotCount);
            spineWindow.WallpaperMode = config.BasicConfig.WallpaperMode;
            spineWindow.MouseClickThrough = config.BasicConfig.MouseClickThrough;
            spineWindow.Opacity = config.BasicConfig.Opacity;
            spineWindow.MaxFps = config.BasicConfig.MaxFps;
            spineWindow.AutoBackgroudColor = config.BasicConfig.AutoBackgroudColor;
            spineWindow.BackgroudColor = config.BasicConfig.BackgroundColor;   // 要先设置 AutoBackgroudColor 再设置 BackgroudColor

            // 加载 Spine 资源
            var spVersion = config.SpineConfig.SpineVersion;
            for (int i = 0; i < spineWindow.SlotCount; i++)
            {
                var skelPath = config.SpineConfig.GetSkelPath(i);
                if (!string.IsNullOrEmpty(skelPath))
                {
                    try { spineWindow.LoadSpine(spVersion, skelPath, index: i); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", "Spine 资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }

            // 加载完资源之后才设置 Spine 相关参数
            spineWindow.SpineScale = config.BasicConfig.SpineScale;
            spineWindow.SpineFlip = config.BasicConfig.SpineFlip;
            spineWindow.SpineUsePMA = config.BasicConfig.SpineUsePMA;

            // 设置窗口可见状态
            spineWindow.Visible = config.SystemConfig.Visible;
        }
    }
}
