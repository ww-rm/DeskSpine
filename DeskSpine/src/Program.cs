using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace DeskSpine
{
    /// <summary>
    /// ����������ö��
    /// </summary>
    public enum EdgeDirection
    {
        Left = 0,       // ABE_LEFT
        Top = 1,        // ABE_TOP
        Right = 2,      // ABE_RIGHT
        Bottom = 3      // ABE_BOTTOM
    }

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
        public static string ProgramResourceDirectory { get; } = Path.Combine(ProgramDirectory, "res");
        public static string ProgramConfigPath { get; } = Path.Combine(ProgramDataDirectory, "config.json");

        public static PerfMonitor.PerfMonitorForm PerfMonitorForm { get; private set; } // ���ܸ���
        public static SpineWindow.SpineRenderWindow spineWindow { get; private set; }   // Spine ����
        public static ConfigForm ConfigForm { get; private set; }                       // ���ô��� (������)
        private static Mutex programMutex;                                              // ����һ������

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

                // ���浽 json ���ֵ

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

                // ������ע�������ֵ

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
                var cur = CurrentConfig;

                // ���ȼ���Ƿ���Ҫ������������, ���´�������ʵ��֮����������������
                if (cur.SpineConfig.WindowType != value.SpineConfig.WindowType)
                {
                    spineWindow.Close();
                    spineWindow = SpineWindow.SpineRenderWindow.New(value.SpineConfig.WindowType, SpineConfig.SlotCount);
                }

                // ������ڻ��߰汾�����˱仯, ��Ҫ�� Spine �������
                if (cur.SpineConfig.WindowType != value.SpineConfig.WindowType ||
                    cur.SpineConfig.SpineVersion != value.SpineConfig.SpineVersion)
                {
                    for (int i = 0; i < SpineConfig.SlotCount; i++)
                        cur.SpineConfig.SetSkelPath(i, null);
                }

                // ����Ƿ���Ҫ������Դ
                for (int i = 0; i < spineWindow.SlotCount; i++)
                {
                    if (cur.SpineConfig.GetSkelPath(i) != value.SpineConfig.GetSkelPath(i))
                    {
                        spineWindow.UnloadSpine(i);
                        var skelPath = value.SpineConfig.GetSkelPath(i);
                        if (!string.IsNullOrEmpty(skelPath))
                        {
                            try { spineWindow.LoadSpine(value.SpineConfig.SpineVersion, skelPath, index: i); }
                            catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", "Spine ��Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                    }
                }

                // ����ȡһ�����ڵ�����ʱ����, ��Ϊ�����ؽ��� Spine ���¼��ػᵼ������ʱ���÷����仯
                cur = CurrentConfig;

                // ϵͳ����
                if (cur.SystemConfig.AutuRun != value.SystemConfig.AutuRun)
                    AutoRun = value.SystemConfig.AutuRun;
                if (cur.SystemConfig.BalloonIconPath != value.SystemConfig.BalloonIconPath)
                {
                    try { ConfigForm.BalloonIconPath = value.SystemConfig.BalloonIconPath; ConfigForm.ShowBalloonTip("ͼ���޸ĳɹ�", "������Ч����~"); }
                    catch (Exception ex) { MessageBox.Show($"{value.SystemConfig.BalloonIconPath} ����ʧ��\n\n{ex}", "����ͼ����Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
                if (cur.SystemConfig.TimeAlarm != value.SystemConfig.TimeAlarm)
                    ConfigForm.TimeAlarm = value.SystemConfig.TimeAlarm;

                // ��������
                if (cur.BasicConfig.WallpaperMode != value.BasicConfig.WallpaperMode)
                    spineWindow.WallpaperMode = value.BasicConfig.WallpaperMode;
                if (cur.BasicConfig.MouseClickThrough != value.BasicConfig.MouseClickThrough)
                    spineWindow.MouseClickThrough = value.BasicConfig.MouseClickThrough;
                if (cur.BasicConfig.PositionX != value.BasicConfig.PositionX || cur.BasicConfig.PositionY != value.BasicConfig.PositionY)
                    spineWindow.Position = new(value.BasicConfig.PositionX, value.BasicConfig.PositionY);
                if (cur.BasicConfig.SizeX != value.BasicConfig.SizeX || cur.BasicConfig.SizeY != value.BasicConfig.SizeY)
                    spineWindow.Size = new(value.BasicConfig.SizeX, value.BasicConfig.SizeY);
                if (cur.BasicConfig.Opacity != value.BasicConfig.Opacity)
                    spineWindow.Opacity = value.BasicConfig.Opacity;
                if (cur.BasicConfig.MaxFps != value.BasicConfig.MaxFps)
                    spineWindow.MaxFps = value.BasicConfig.MaxFps;
                if (cur.BasicConfig.AutoBackgroudColor != value.BasicConfig.AutoBackgroudColor)
                    spineWindow.AutoBackgroudColor = value.BasicConfig.AutoBackgroudColor;
                if (cur.BasicConfig.BackgroundColor != value.BasicConfig.BackgroundColor)
                    spineWindow.BackgroudColor = value.BasicConfig.BackgroundColor;     // Ҫ������ AutoBackgroudColor ������ BackgroudColor

                // scale �������д���, ���һ����ö���, �����иĶ�ʱ������
                if (Math.Abs(cur.BasicConfig.SpineScale - value.BasicConfig.SpineScale) > 1e-3)
                    spineWindow.SpineScale = value.BasicConfig.SpineScale;
                spineWindow.SpinePosition = new(value.BasicConfig.SpinePositionX, value.BasicConfig.SpinePositionY);
                spineWindow.SpineFlip = value.BasicConfig.SpineFlip;
                spineWindow.SpineUsePMA = value.BasicConfig.SpineUsePMA;

                // ������ô��ڿɼ���
                if (cur.SystemConfig.Visible != value.SystemConfig.Visible)
                    spineWindow.Visible = value.SystemConfig.Visible;

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
        private static void InitFromConfig(Config config)
        {
            // ����Ҫ��ע�����洢����Ϣ, �����ڲ��Զ���Ч

            // ϵͳ����
            try { ConfigForm.BalloonIconPath = config.SystemConfig.BalloonIconPath; }
            catch (Exception ex) { MessageBox.Show($"{config.SystemConfig.BalloonIconPath} ����ʧ��\n\n{ex}", "����ͼ����Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            ConfigForm.TimeAlarm = config.SystemConfig.TimeAlarm;

            // ��������
            spineWindow = SpineWindow.SpineRenderWindow.New(config.SpineConfig.WindowType, SpineConfig.SlotCount);
            spineWindow.WallpaperMode = config.BasicConfig.WallpaperMode;
            spineWindow.MouseClickThrough = config.BasicConfig.MouseClickThrough;
            spineWindow.Opacity = config.BasicConfig.Opacity;
            spineWindow.MaxFps = config.BasicConfig.MaxFps;
            spineWindow.AutoBackgroudColor = config.BasicConfig.AutoBackgroudColor;
            spineWindow.BackgroudColor = config.BasicConfig.BackgroundColor;   // Ҫ������ AutoBackgroudColor ������ BackgroudColor

            // ���� Spine ��Դ
            var spVersion = config.SpineConfig.SpineVersion;
            for (int i = 0; i < spineWindow.SlotCount; i++)
            {
                var skelPath = config.SpineConfig.GetSkelPath(i);
                if (!string.IsNullOrEmpty(skelPath))
                {
                    try { spineWindow.LoadSpine(spVersion, skelPath, index: i); }
                    catch (Exception ex) { MessageBox.Show($"{skelPath} ����ʧ��\n\n{ex}", "Spine ��Դ����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }

            // ��������Դ֮������� Spine ��ز���
            spineWindow.SpineScale = config.BasicConfig.SpineScale;
            spineWindow.SpineFlip = config.BasicConfig.SpineFlip;
            spineWindow.SpineUsePMA = config.BasicConfig.SpineUsePMA;

            // ���ô��ڿɼ�״̬
            spineWindow.Visible = config.SystemConfig.Visible;
        }

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
            PerfMonitorForm = new() { UseLightTheme = SystemUseLightTheme };
            ConfigForm = new ConfigForm();
            InitFromConfig(LocalConfig);

            Application.Run();
        }

        /// <summary>
        /// ��ȡϵͳ������ɫ
        /// </summary>
        public static bool SystemUseLightTheme
        {
            get
            {
                using (RegistryKey personalizeKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                    return int.Parse(personalizeKey.GetValue("SystemUsesLightTheme", "0").ToString()) != 0;
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        public static EdgeDirection TaskbarDirection
        {
            get
            {
                // ABM_GETTASKBARPOS = 0x5
                APPBARDATA abData = new APPBARDATA();
                abData.cbSize = Marshal.SizeOf(abData);
                if (SHAppBarMessage(5, ref abData) != 0)
                {
                    return (EdgeDirection)abData.uEdge;
                }
                return EdgeDirection.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);
    }
}
