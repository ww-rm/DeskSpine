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
                MessageBox.Show("������������, �����ظ�����", ProgramName);
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

        /// <summary>
        /// �ӱ��������ļ����г�ʼ��
        /// </summary>
        private static void InitFromLocalConfig()
        {
            var localConfig = new Config();
            localConfig.Load(ProgramConfigPath);

            // ϵͳ����
            AutoRun = localConfig.SystemConfig.AutuRun;

            // ��������
            spineWindow = SpineWindow.SpineWindow.New(localConfig.BasicConfig.WindowType, SpineConfig.SlotCount);
            spineWindow.WallpaperMode = localConfig.BasicConfig.WallpaperMode;
            spineWindow.MouseClickThrough = localConfig.BasicConfig.MouseClickThrough;
            spineWindow.SpineFlip = localConfig.BasicConfig.SpineFlip;
            spineWindow.SpineScale = localConfig.BasicConfig.SpineScale;
            spineWindow.Opacity = localConfig.BasicConfig.Opacity;
            spineWindow.MaxFps = localConfig.BasicConfig.MaxFps;
            spineWindow.BackgroudColor = localConfig.BasicConfig.BackgroudColor;

            // ����Ҫ��λ�úʹ�С��Ϣ, ��ͨ��ע������

            // ���� Spine ��Դ
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
                        MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", ProgramName);
                    }
                }
            }

            // ���ô��ڿɼ�״̬
            spineWindow.Visible = localConfig.SystemConfig.Visible;
        }

        /// <summary>
        /// ���ݳ���ʵʱ����״̬��ȡ����
        /// </summary>
        /// <returns></returns>
        public static Config GetCurrentConfig()
        {
            var config = new Config();
            config.SystemConfig.AutuRun = AutoRun;
            config.SystemConfig.Visible = spineWindow.Visible;
            config.BasicConfig.WallpaperMode = spineWindow.WallpaperMode;
            config.BasicConfig.MouseClickThrough = spineWindow.MouseClickThrough;
            config.BasicConfig.SpineScale = spineWindow.SpineScale;
            config.BasicConfig.SpineFlip = spineWindow.SpineFlip;
            config.BasicConfig.Opacity = spineWindow.Opacity;
            config.BasicConfig.MaxFps = spineWindow.MaxFps;
            config.BasicConfig.BackgroudColor = spineWindow.BackgroudColor;
            config.BasicConfig.WindowType = spineWindow.Type;
            config.BasicConfig.SpineVersion = spineWindow.SpineVersion;
            for (int i = 0; i < spineWindow.SlotCount; i++)
            {
                config.SpineConfig.SetSkelPath(i, spineWindow.GetSpineSkelPath(i));
            }

            // һЩ��ʱ��
            var position = spineWindow.Position;
            config.BasicConfig.PositionX = position.X;
            config.BasicConfig.PositionY = position.Y;
            var size = spineWindow.Size;
            config.BasicConfig.SizeX = size.X;
            config.BasicConfig.SizeY = size.Y;
            var spPosition = spineWindow.Position;
            config.BasicConfig.SpinePositionX = spPosition.X;
            config.BasicConfig.SpinePositionY = spPosition.Y;
            config.BasicConfig.ClearColor = spineWindow.ClearColor;

            return config;
        }
    }
}
