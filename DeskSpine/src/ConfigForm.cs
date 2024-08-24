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
            this.exitCommand.Click += ExitCommand_Click;

            this.notifyIcon.MouseClick += NotifyIcon_MouseClick;
            this.notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void ExitCommand_Click(object? sender, EventArgs e)
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
                this.Show();
                this.Activate();
            }
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            Program.azurLaneSD.LoadSpine(@"D:\ACGN\AzurLane_Export\spines\aierbin\aierbin.skel");
        }
    }

}
