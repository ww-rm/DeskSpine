using System.Diagnostics;

namespace DeskSpine
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            this.FormClosing += ConfigForm_FormClosing;
            this.commandExit.Click += CommandExit_Click;

            this.notifyIcon.MouseClick += NotifyIcon_MouseClick;
            this.notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void CommandExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ConfigForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                //Program.azurLaneSD.Reset();
                Program.spineWindow.LoadSpine("3.6", @"D:\ACGN\AzurLane_Export\spines\aierbin\aierbin.skel");
            }
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }
    }

}
