namespace DeskSpine
{
    public static class Program
    {
        public static ConfigForm configForm;
        public static SpineWindow.AzurLaneSD azurLaneSD;
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
            Program.configForm = new ConfigForm();

            Program.azurLaneSD = new SpineWindow.AzurLaneSD("D:\\Program Files\\DesktopSprite\\res\\spine\\guanghui_2.skel");
            Program.azurLaneSD.Visible = true;
            Application.Run();
        }
    }
}