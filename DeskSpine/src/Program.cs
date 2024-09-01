using Microsoft.Win32;

namespace DeskSpine
{
    public static class Program
    {
        public const string ProgramName = "DeskSpine";
        public static ConfigForm configForm;
        public static SpineWindow.SpineWindow spineWindow;
        public static Mutex programMutex;


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
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            bool createNew;
            programMutex = new Mutex(true, "DeskSpine", out createNew);

#if !DEBUG
            if (!createNew)
            {
                MessageBox.Show("程序已在运行, 请勿重复启动", "DeskSpine");
                return;
            }
#endif

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            configForm = new ConfigForm();

            spineWindow = new SpineWindow.AzurLaneSD();
            spineWindow.LoadSpine("3.6", "D:\\Program Files\\DesktopSprite\\res\\spine\\guanghui_2.skel");
            spineWindow.Visible = true;
            Application.Run();
        }
    }
}