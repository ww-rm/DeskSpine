using Microsoft.Win32;
using SpineWindow;

namespace DeskSpine
{
    public static class Program
    {
        public const string ProgramName = "DeskSpine";
        public static string LocalAppdataDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ProgramDirectory { get; } = Path.GetDirectoryName(Application.ExecutablePath);
        public static string ProgramDataDirectory { get; } = Path.Combine(LocalAppdataDirectory, ProgramName);
        public static string ProgramConfigPath { get; } = Path.Combine(ProgramDataDirectory, "config.json");

        public static ConfigForm ConfigForm { get; private set; }
        public static SpineWindow.SpineWindow WindowSpine { get; private set; }

        private static Mutex programMutex;

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

        [STAThread]
        public static void Main()
        {
            programMutex = new Mutex(true, ProgramName, out bool createNew);

#if !DEBUG
            if (!createNew)
            {
                MessageBox.Show("程序已在运行, 请勿重复启动", ProgramName);
                return;
            }
#endif

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (!Directory.Exists(ProgramDataDirectory))
                Directory.CreateDirectory(ProgramDataDirectory);
            InitFromLocalConfig();
            ConfigForm = new ConfigForm();
            Application.Run();
        }

        /// <summary>
        /// 从本地配置文件进行初始化
        /// </summary>
        private static void InitFromLocalConfig()
        {
            var localConfig = new Config();
            localConfig.Load(ProgramConfigPath);

            // 系统配置
            AutoRun = localConfig.SystemConfig.AutuRun;

            // 基础配置
            WindowSpine = SpineWindow.SpineWindow.New(localConfig.SpineConfig.WindowType, SpineConfig.SlotCount);
            WindowSpine.WallpaperMode = localConfig.BasicConfig.WallpaperMode;
            WindowSpine.MouseClickThrough = localConfig.BasicConfig.MouseClickThrough;
            WindowSpine.Opacity = localConfig.BasicConfig.Opacity;
            WindowSpine.MaxFps = localConfig.BasicConfig.MaxFps;
            WindowSpine.BackgroudColor = localConfig.BasicConfig.BackgroudColor;

            // 不需要管位置和大小信息, 会通过注册表加载

            // 加载 Spine 资源
            var spVersion = localConfig.SpineConfig.SpineVersion;
            for (int i = 0; i < WindowSpine.SlotCount; i++)
            {
                var skelPath = localConfig.SpineConfig.GetSkelPath(i);
                if (!string.IsNullOrEmpty(skelPath))
                {
                    try { WindowSpine.LoadSpine(spVersion, skelPath, index: i); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                }
            }

            // 加载完资源之后才设置 Spine 相关参数
            WindowSpine.SpineFlip = localConfig.BasicConfig.SpineFlip;
            WindowSpine.SpineScale = localConfig.BasicConfig.SpineScale;
            WindowSpine.SpineUsePMA = localConfig.BasicConfig.SpineUsePMA;

            // 设置窗口可见状态
            WindowSpine.Visible = localConfig.SystemConfig.Visible;
        }

        /// <summary>
        /// 根据程序实时运行状态获取配置
        /// </summary>
        /// <returns></returns>
        public static Config CurrentConfig
        {
            get
            {
                var config = new Config();
                config.SystemConfig.AutuRun = AutoRun;
                config.SystemConfig.Visible = WindowSpine.Visible;
                config.BasicConfig.WallpaperMode = WindowSpine.WallpaperMode;
                config.BasicConfig.MouseClickThrough = WindowSpine.MouseClickThrough;
                config.BasicConfig.SpineScale = WindowSpine.SpineScale;
                config.BasicConfig.SpineFlip = WindowSpine.SpineFlip;
                config.BasicConfig.Opacity = WindowSpine.Opacity;
                config.BasicConfig.MaxFps = WindowSpine.MaxFps;
                config.BasicConfig.BackgroudColor = WindowSpine.BackgroudColor;
                config.SpineConfig.WindowType = WindowSpine.Type;
                config.SpineConfig.SpineVersion = WindowSpine.SpineVersion;
                for (int i = 0; i < WindowSpine.SlotCount; i++)
                {
                    config.SpineConfig.SetSkelPath(i, WindowSpine.GetSpineSkelPath(i));
                }

                // 一些临时量
                var position = WindowSpine.Position;
                config.BasicConfig.PositionX = position.X;
                config.BasicConfig.PositionY = position.Y;
                var size = WindowSpine.Size;
                config.BasicConfig.SizeX = size.X;
                config.BasicConfig.SizeY = size.Y;
                var spPosition = WindowSpine.SpinePosition;
                config.BasicConfig.SpinePositionX = spPosition.X;
                config.BasicConfig.SpinePositionY = spPosition.Y;
                config.BasicConfig.ClearColor = WindowSpine.ClearColor;

                return config;
            }
        }

        public static Config LocalConfig
        {
            get { var v = new Config(); v.Load(ProgramConfigPath); return v; }
            set => value.Save(ProgramConfigPath);
        }

        public static void ApplyConfig(Config config)
        {
            // 优先检测是否需要更换窗口类型, 重新创建窗口实例之后
            if (CurrentConfig.SpineConfig.WindowType != config.SpineConfig.WindowType)
            {
                WindowSpine.Dispose();
                WindowSpine = SpineWindow.SpineWindow.New(config.SpineConfig.WindowType, SpineConfig.SlotCount);
            }

            var currentConfig = CurrentConfig;

            // 系统设置
            if (currentConfig.SystemConfig.AutuRun != config.SystemConfig.AutuRun)
                AutoRun = config.SystemConfig.AutuRun;
            if (currentConfig.SystemConfig.Visible != config.SystemConfig.Visible)
                WindowSpine.Visible = config.SystemConfig.Visible;

            // 基础设置
            if (currentConfig.BasicConfig.WallpaperMode != config.BasicConfig.WallpaperMode)
                WindowSpine.WallpaperMode = config.BasicConfig.WallpaperMode;
            if (currentConfig.BasicConfig.MouseClickThrough != config.BasicConfig.MouseClickThrough)
                WindowSpine.MouseClickThrough = config.BasicConfig.MouseClickThrough;
            if (currentConfig.BasicConfig.PositionX != config.BasicConfig.PositionX ||
                currentConfig.BasicConfig.PositionY != config.BasicConfig.PositionY)
                WindowSpine.Position = new(config.BasicConfig.PositionX, config.BasicConfig.PositionY);
            if (currentConfig.BasicConfig.SizeX != config.BasicConfig.SizeX ||
                currentConfig.BasicConfig.SizeY != config.BasicConfig.SizeY)
                WindowSpine.Size = new(config.BasicConfig.SizeX, config.BasicConfig.SizeY);
            if (currentConfig.BasicConfig.Opacity != config.BasicConfig.Opacity)
                WindowSpine.Opacity = config.BasicConfig.Opacity;
            if (currentConfig.BasicConfig.MaxFps != config.BasicConfig.MaxFps)
                WindowSpine.MaxFps = config.BasicConfig.MaxFps;
            if (currentConfig.BasicConfig.BackgroudColor != config.BasicConfig.BackgroudColor)
                WindowSpine.BackgroudColor = config.BasicConfig.BackgroudColor;

            // Spine 设置
            if (currentConfig.SpineConfig.SpineVersion != config.SpineConfig.SpineVersion)
            {
                for (int i = 0; i < WindowSpine.SlotCount; i++)
                {
                    WindowSpine.UnloadSpine(i);
                    var skelPath = config.SpineConfig.GetSkelPath(i);
                    if (!string.IsNullOrEmpty(skelPath))
                    {
                        try { WindowSpine.LoadSpine(config.SpineConfig.SpineVersion, skelPath, index: i); }
                        catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                    }
                }
            }
            else
            {
                for (int i = 0; i < WindowSpine.SlotCount; i++)
                {
                    if (currentConfig.SpineConfig.GetSkelPath(i) != config.SpineConfig.GetSkelPath(i))
                    {
                        WindowSpine.UnloadSpine(i);
                        var skelPath = config.SpineConfig.GetSkelPath(i);
                        if (!string.IsNullOrEmpty(skelPath))
                        {
                            try { WindowSpine.LoadSpine(config.SpineConfig.SpineVersion, skelPath, index: i); }
                            catch (Exception ex) { MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName); }
                        }
                    }
                }
            }

            // scale 的设置有代价, 并且会重置动画, 所以有改动时再设置
            if (Math.Abs(currentConfig.BasicConfig.SpineScale - config.BasicConfig.SpineScale) > 1e-3)
                WindowSpine.SpineScale = config.BasicConfig.SpineScale;
            WindowSpine.SpinePosition = new(config.BasicConfig.SpinePositionX, config.BasicConfig.SpinePositionY);
            WindowSpine.SpineFlip = config.BasicConfig.SpineFlip;
            WindowSpine.SpineUsePMA = config.BasicConfig.SpineUsePMA;

            // 保存本地
            LocalConfig = config;
        }
    }
}
