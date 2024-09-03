using System.Diagnostics;

namespace DeskSpine
{
    public partial class ConfigForm : Form
    {
        private Config currentConfig = null;

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

        private void ConfigForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                currentConfig = Program.CurrentConfig;

                // 系统设置
                checkBox_AutoRun.Checked = currentConfig.SystemConfig.AutuRun;
                checkBox_Visible.Checked = currentConfig.SystemConfig.Visible;

                // 基础设置
                checkBox_WallpaperMode.Checked = currentConfig.BasicConfig.WallpaperMode;
                checkBox_MouseClickThrough.Checked = currentConfig.BasicConfig.MouseClickThrough;
                numericUpDown_PositionX.Value = currentConfig.BasicConfig.PositionX;
                numericUpDown_PositionY.Value = currentConfig.BasicConfig.PositionY;
                numericUpDown_SizeX.Value = currentConfig.BasicConfig.SizeX;
                numericUpDown_SizeY.Value = currentConfig.BasicConfig.SizeY;
                numericUpDown_SpinePositionX.Value = (decimal)currentConfig.BasicConfig.SpinePositionX;
                numericUpDown_SpinePositionY.Value = (decimal)currentConfig.BasicConfig.SpinePositionY;
                checkBox_SpineFlip.Checked = currentConfig.BasicConfig.SpineFlip;
                trackBar_SpineScale.Value = (int)(currentConfig.BasicConfig.SpineScale * 100);
                trackBar_Opacity.Value = currentConfig.BasicConfig.Opacity;
                trackBar_MaxFps.Value = (int)currentConfig.BasicConfig.MaxFps;
                comboBox_BackgroudColor.SelectedItem = currentConfig.BasicConfig.BackgroudColor switch
                {
                    SpineWindow.BackgroudColor.Black => "黑色",
                    SpineWindow.BackgroudColor.White => "白色",
                    SpineWindow.BackgroudColor.Gray => "灰色",
                    _ => "灰色"
                };
                textBox_ClearColor.Text = $"#{currentConfig.BasicConfig.ClearColor.ToInteger():X8}";

                // Spine 设置
                comboBox_SpineVersion.SelectedItem = currentConfig.BasicConfig.SpineVersion;
                comboBox_WindowType.SelectedItem = currentConfig.BasicConfig.WindowType switch
                {
                    SpineWindow.SpineWindowType.AzurLaneSD => "碧蓝航线_后宅小人",
                    SpineWindow.SpineWindowType.AzurLaneDynamic => "碧蓝航线_动态立绘",
                    SpineWindow.SpineWindowType.ArknightsDynamic => "明日方舟_动态立绘",
                    _ => "碧蓝航线_后宅小人",
                };
                textBox_SkelPath0.Text = currentConfig.SpineConfig.SkelPath0;
                textBox_SkelPath1.Text = currentConfig.SpineConfig.SkelPath1;
                textBox_SkelPath2.Text = currentConfig.SpineConfig.SkelPath2;
                textBox_SkelPath3.Text = currentConfig.SpineConfig.SkelPath3;
                textBox_SkelPath4.Text = currentConfig.SpineConfig.SkelPath4;
                textBox_SkelPath5.Text = currentConfig.SpineConfig.SkelPath5;
                textBox_SkelPath6.Text = currentConfig.SpineConfig.SkelPath6;
                textBox_SkelPath7.Text = currentConfig.SpineConfig.SkelPath7;
                textBox_SkelPath8.Text = currentConfig.SpineConfig.SkelPath8;
                textBox_SkelPath9.Text = currentConfig.SpineConfig.SkelPath9;
            }
            else
            {
                currentConfig = null;
            }
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

        private void button_SelectSkel0_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath0.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath0.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel1_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath1.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath1.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel2_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath2.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath2.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel3_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath3.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath3.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel4_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath4.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath4.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel5_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath5.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath5.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel6_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath6.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath6.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel7_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath7.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath7.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel8_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath8.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath8.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }
        private void button_SelectSkel9_Click(object sender, EventArgs e)
        {
            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox_SkelPath9.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox_SkelPath9.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }

        private void button_ClearSkel0_Click(object sender, EventArgs e)
        {
            textBox_SkelPath0.Text = string.Empty;
        }
        private void button_ClearSkel1_Click(object sender, EventArgs e)
        {
            textBox_SkelPath1.Text = string.Empty;
        }
        private void button_ClearSkel2_Click(object sender, EventArgs e)
        {
            textBox_SkelPath2.Text = string.Empty;
        }
        private void button_ClearSkel3_Click(object sender, EventArgs e)
        {
            textBox_SkelPath3.Text = string.Empty;
        }
        private void button_ClearSkel4_Click(object sender, EventArgs e)
        {
            textBox_SkelPath4.Text = string.Empty;
        }
        private void button_ClearSkel5_Click(object sender, EventArgs e)
        {
            textBox_SkelPath5.Text = string.Empty;
        }
        private void button_ClearSkel6_Click(object sender, EventArgs e)
        {
            textBox_SkelPath6.Text = string.Empty;
        }
        private void button_ClearSkel7_Click(object sender, EventArgs e)
        {
            textBox_SkelPath7.Text = string.Empty;
        }
        private void button_ClearSkel8_Click(object sender, EventArgs e)
        {
            textBox_SkelPath8.Text = string.Empty;
        }
        private void button_ClearSkel9_Click(object sender, EventArgs e)
        {
            textBox_SkelPath9.Text = string.Empty;
        }

        private void button_OpenDataFolder_Click(object sender, EventArgs e)
        {
            try { Process.Start("explorer.exe", Program.ProgramDataDirectory); }
            catch (Exception ex) { MessageBox.Show($"无法打开文件夹: {ex.Message}", Program.ProgramName); }
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            button_Apply_Click(sender, e);
            this.Hide();
        }

        private void button_Apply_Click(object sender, EventArgs e)
        {
            // 应用设置项
            // 逐项比较是否发生变化然后应用更改
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


        }

        private void commandConfig_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
        }

        private void CommandExit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
