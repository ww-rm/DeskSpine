using System.Diagnostics;

namespace DeskSpine
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            comboBox_BackgroudColor.SelectedItem = "灰色";
            comboBox_SpineVersion.SelectedItem = "3.8.x";
            comboBox_WindowType.SelectedItem = "碧蓝航线_后宅小人";
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
            var config = Program.GetCurrentConfig();

        }

        private void ConfigForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }


        private void trackBar_SpineScale_ValueChanged(object sender, EventArgs e) { label_SpineScale.Text = $"{trackBar_SpineScale.Value}"; }
        private void trackBar_Opacity_ValueChanged(object sender, EventArgs e) { label_Opacity.Text = $"{trackBar_Opacity.Value}"; }
        private void trackBar_MaxFps_ValueChanged(object sender, EventArgs e) { label_MaxFps.Text = $"{trackBar_MaxFps.Value}"; }

        private void button_Ok_Click(object sender, EventArgs e)
        {

        }

        private void button_OpenDataFolder_Click(object sender, EventArgs e)
        {

        }

        private void button_Apply_Click(object sender, EventArgs e)
        {

        }

        private void CommandExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }



        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                //var a = new Config();
                //a.Load("a.json");
                //a.Save("b.json");
            }
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

    }

}
