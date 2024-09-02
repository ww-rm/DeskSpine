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

        private static ConfigForm configForm;
        private static SpineWindow.SpineWindow spineWindow;

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
            configForm = new ConfigForm();
            Application.Run();
        }

        private static void InitFromLocalConfig()
        {
            var localConfig = new Config();
            localConfig.Load(ProgramConfigPath);

            // 系统配置
            AutoRun = localConfig.SystemConfig.AutuRun;

            // 基础配置
            spineWindow = SpineWindow.SpineWindow.New(localConfig.BasicConfig.WindowType, SpineConfig.SlotCount);
            spineWindow.WallpaperMode = localConfig.BasicConfig.WallpaperMode;
            spineWindow.MouseClickThrough = localConfig.BasicConfig.MouseClickThrough;
            spineWindow.SpineFlip = localConfig.BasicConfig.SpineFlip;
            spineWindow.SpineScale = localConfig.BasicConfig.SpineScale;
            spineWindow.Opacity = localConfig.BasicConfig.Opacity;
            spineWindow.MaxFps = localConfig.BasicConfig.MaxFps;
            spineWindow.BackgroudColor = localConfig.BasicConfig.BackgroudColor;

            // 加载 Spine 资源
            var spVersion = localConfig.BasicConfig.SpineVersion;
            for (int i = 0; i < spineWindow.SlotCount; i++)
            {
                var skelPath = localConfig.SpineConfig.GetSkelPath(i);
                if (skelPath is not null)
                {
                    try
                    { 
                        spineWindow.LoadSpine(spVersion, skelPath, index: i); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{skelPath} 加载失败\n\n{ex}", ProgramName);
                    }
                }
            }

            // 设置窗口可见状态
            spineWindow.Visible = localConfig.SystemConfig.Visible;
        }
    }
}
