namespace DeskSpine
{
    public static class Program
    {
        public static ConfigForm configForm;
        public static SpineWindow.SpineWindow spineWindow;
        public static object debugobj;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
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