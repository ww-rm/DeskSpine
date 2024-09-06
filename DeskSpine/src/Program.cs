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

        public static ConfigForm ConfigForm { get; private set; }                   // ���ô��� (������)
        public static SpineWindow.SpineWindow WindowSpine { get; private set; }     // Spine ����
        private static Mutex programMutex;                                          // ����һ������

        /// <summary>
        /// �Ƿ񿪻�����
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
        /// ���ݳ���ʵʱ����״̬��ȡ����
        /// </summary>
        public static Config CurrentConfig
        {
            get
            {
                var config = new Config();

                // ���浽 json ���ֵ

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

                // ������ע�������ֵ

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

                // ���ȼ���Ƿ���Ҫ������������, ���´�������ʵ��֮����������������
                if (cur.SpineConfig.WindowType != value.SpineConfig.WindowType)
                {
                    WindowSpine.Dispose();
                    WindowSpine = SpineWindow.SpineWindow.New(value.SpineConfig.WindowType, SpineConfig.SlotCount);
                }

                // ����Ƿ���Ҫ������Դ
                if (cur.SpineConfig.SpineVersion != value.SpineConfig.SpineVersion)
                {
                    for (int i = 0; i < WindowSpine.SlotCount; i++)
                    {
                        WindowSpine.UnloadSpine(i);
                        var skelPath = value.SpineConfig.GetSkelPath(i);
                        if (!string.IsNullOrEmpty(skelPath))
                        {
                            try { WindowSpine.LoadSpine(value.SpineConfig.SpineVersion, skelPath, index: i); }
                            catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", ProgramName); }
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
                                catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", ProgramName); }
                            }
                        }
                    }
                }

                // ����ȡһ�����ڵ�����ʱ����
                cur = CurrentConfig;

                // ϵͳ����
                if (cur.SystemConfig.AutuRun != value.SystemConfig.AutuRun)
                    AutoRun = value.SystemConfig.AutuRun;

                // ��������
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
                    WindowSpine.BackgroudColor = value.BasicConfig.BackgroundColor;     // Ҫ������ AutoBackgroudColor ������ BackgroudColor

                // scale �������д���, ���һ����ö���, �����иĶ�ʱ������
                if (Math.Abs(cur.BasicConfig.SpineScale - value.BasicConfig.SpineScale) > 1e-3)
                    WindowSpine.SpineScale = value.BasicConfig.SpineScale;
                WindowSpine.SpinePosition = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
                WindowSpine.SpineFlip = value.BasicConfig.SpineFlip;
                WindowSpine.SpineUsePMA = value.BasicConfig.SpineUsePMA;

                // ������ô��ڿɼ���
                if (cur.SystemConfig.Visible != value.SystemConfig.Visible)
                    WindowSpine.Visible = value.SystemConfig.Visible;

                // ���汾��
                LocalConfig = value;
            }
        }

        /// <summary>
        /// ��ȡ�ͱ��汾������
        /// </summary>
        public static Config LocalConfig
        {
            get { var v = new Config(); v.Load(ProgramConfigPath); return v; }
            set => value.Save(ProgramConfigPath);
        }

        /// <summary>
        /// ��ָ�������ý��г�ʼ��
        /// </summary>
        private static void InitFromConfig(Config config)
        {
            // ����Ҫ��ע�����洢����Ϣ, �����ڲ��Զ���Ч

            // ϵͳ����

            // ��������
            WindowSpine = SpineWindow.SpineWindow.New(config.SpineConfig.WindowType, SpineConfig.SlotCount);
            WindowSpine.WallpaperMode = config.BasicConfig.WallpaperMode;
            WindowSpine.MouseClickThrough = config.BasicConfig.MouseClickThrough;
            WindowSpine.Opacity = config.BasicConfig.Opacity;
            WindowSpine.MaxFps = config.BasicConfig.MaxFps;
            WindowSpine.AutoBackgroudColor = config.BasicConfig.AutoBackgroudColor;
            WindowSpine.BackgroudColor = config.BasicConfig.BackgroundColor;   // Ҫ������ AutoBackgroudColor ������ BackgroudColor

            // ���� Spine ��Դ
            var spVersion = config.SpineConfig.SpineVersion;
            for (int i = 0; i < WindowSpine.SlotCount; i++)
            {
                var skelPath = config.SpineConfig.GetSkelPath(i);
                if (!string.IsNullOrEmpty(skelPath))
                {
                    try { WindowSpine.LoadSpine(spVersion, skelPath, index: i); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", ProgramName); }
                }
            }

            // ��������Դ֮������� Spine ��ز���
            WindowSpine.SpineScale = config.BasicConfig.SpineScale;
            WindowSpine.SpineFlip = config.BasicConfig.SpineFlip;
            WindowSpine.SpineUsePMA = config.BasicConfig.SpineUsePMA;

            // ���ô��ڿɼ�״̬
            WindowSpine.Visible = config.SystemConfig.Visible;
        }

        /// <summary>
        /// �������
        /// </summary>
        [STAThread]
        public static void Main()
        {
            programMutex = new Mutex(true, ProgramName, out bool createNew);
            if (!createNew)
            {
                MessageBox.Show("������������, �����ظ�����", ProgramName);
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
