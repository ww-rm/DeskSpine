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
        /// �����ִ���ļ�����Ŀ¼
        /// </summary>
        public static readonly string ProgramDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// �������ݴ��Ŀ¼
        /// </summary>
        public static readonly string ProgramDataDirectory = Path.Combine(SystemValue.LocalAppdataDirectory, ProgramName);

        /// <summary>
        /// ������ԴĿ¼
        /// </summary>
        public static readonly string ProgramResourceDirectory = Path.Combine(ProgramDirectory, "res");

        /// <summary>
        /// ���������ļ�·��
        /// </summary>
        public static readonly string ProgramConfigPath = Path.Combine(ProgramDataDirectory, "config.json");

        public static PerfMonitor.PerfMonitorForm PerfMonitorForm { get; private set; }     // ���ܸ���
        public static ConfigForm ConfigForm { get; private set; }                           // ���ô��� (������)
        public static TinyEngine.SpineRenderWindow SpineWindow { get; private set; }        // Spine ����
        private static Mutex programMutex;                                                  // ����һ������
        private static System.Timers.Timer configSaveTimer = new(300000) { AutoReset = true };

        /// <summary>
        /// �������
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
                MessageBox.Show("������������, �����ظ�����", ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(ProgramDataDirectory)) 
                Directory.CreateDirectory(ProgramDataDirectory);

            PerfMonitorForm = new() { UseLightTheme = SystemValue.SystemUseLightTheme };
            SpineWindow = new TinyEngine.SpineRenderWindow(SpineConfig.SlotCount);
            ConfigForm = new ConfigForm();
            InitFromConfig(LocalConfig);

            // ����һ�� Timer ��ʱ��������
            configSaveTimer.Elapsed += (object? s, System.Timers.ElapsedEventArgs e) => LocalConfig = CurrentConfig;
            configSaveTimer.Enabled = true;

            Application.Run();
        }

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
        /// ���ݳ���ʵʱ����״̬��ȡ����
        /// </summary>
        public static Config CurrentConfig
        {
            get
            {
                var config = new Config();

                // ϵͳ����
                config.SystemConfig.AutuRun = AutoRun;
                config.SystemConfig.Visible = SpineWindow.Visible;
                config.SystemConfig.BalloonIconPath = ConfigForm.BalloonIconPath;
                config.SystemConfig.HourlyChime = ConfigForm.HourlyChime;

                // ��������
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

                // Spine ����
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
                // XXX: ��ȷ���Ƿ���Ҫ Lock/Unlock

                // ϵͳ����
                AutoRun = value.SystemConfig.AutuRun;
                ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath;
                ConfigForm.HourlyChime = value.SystemConfig.HourlyChime;

                // ��������
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

                // Spine ����
                SpineWindow.Spine.AnimatorType = value.SpineConfig.InteractMode;
                try
                {
                    SpineWindow.Spine.Version = value.SpineConfig.SpineVersion;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ConfigForm,
                        $"�汾 {value.SpineConfig.SpineVersion} ����ʧ��\n\n{ex}",
                        "Spine ��Դ����ʧ��",
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
                        MessageBox.Show(ConfigForm, $"{skelPath} ����ʧ��\n\n{ex}", "Spine ��Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    }
                }

                // ���������ڿɼ���
                SpineWindow.Visible = value.SystemConfig.Visible;

                // ���汾��
                LocalConfig = CurrentConfig;
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
        private static void InitFromConfig(Config value)
        {
            // ϵͳ����
            ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath; 
            ConfigForm.HourlyChime = value.SystemConfig.HourlyChime;

            // ��������
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

            // Spine ����
            SpineWindow.Spine.AnimatorType = value.SpineConfig.InteractMode;
            try 
            { 
                SpineWindow.Spine.Version = value.SpineConfig.SpineVersion; 
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(
                    ConfigForm, 
                    $"�汾 {value.SpineConfig.SpineVersion} ����ʧ��\n\n{ex}", 
                    "Spine ��Դ����ʧ��", 
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
                    MessageBox.Show(ConfigForm, $"{skelPath} ����ʧ��\n\n{ex}", "Spine ��Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
            }

            // ���������ڿɼ���
            SpineWindow.Visible = value.SystemConfig.Visible;
        }
    }
}
