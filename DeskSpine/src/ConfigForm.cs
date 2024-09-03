using System.Diagnostics;

namespace DeskSpine
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        public Config Value
        {
            get
            {
                var v = new Config();
                // 系统设置
                v.SystemConfig.AutuRun = checkBox_AutoRun.Checked;
                v.SystemConfig.Visible = checkBox_Visible.Checked;

                // 基础设置
                v.BasicConfig.WallpaperMode = checkBox_WallpaperMode.Checked;
                v.BasicConfig.MouseClickThrough = checkBox_MouseClickThrough.Checked;
                v.BasicConfig.PositionX = (int)numericUpDown_PositionX.Value;
                v.BasicConfig.PositionY = (int)numericUpDown_PositionY.Value;
                v.BasicConfig.SizeX = (uint)numericUpDown_SizeX.Value;
                v.BasicConfig.SizeY = (uint)numericUpDown_SizeY.Value;
                v.BasicConfig.SpinePositionX = (float)numericUpDown_SpinePositionX.Value;
                v.BasicConfig.SpinePositionY = (float)numericUpDown_SpinePositionY.Value;
                v.BasicConfig.SpineFlip = checkBox_SpineFlip.Checked;
                v.BasicConfig.SpineScale = trackBar_SpineScale.Value / 100.0f;
                v.BasicConfig.Opacity = (byte)trackBar_Opacity.Value;
                v.BasicConfig.MaxFps = (uint)trackBar_MaxFps.Value;
                v.BasicConfig.SpineUsePMA = checkBox_SpineUsePMA.Checked;

                // 获取背景颜色
                v.BasicConfig.BackgroudColor = comboBox_BackgroudColor.SelectedItem switch
                {
                    "黑色" => SpineWindow.BackgroudColor.Black,
                    "白色" => SpineWindow.BackgroudColor.White,
                    "灰色" => SpineWindow.BackgroudColor.Gray,
                    _ => SpineWindow.BackgroudColor.Gray
                };

                // 获取清除颜色
                v.BasicConfig.ClearColor = new(0, 0, 0, 0);

                // Spine 设置
                v.SpineConfig.SpineVersion = (string)comboBox_SpineVersion.SelectedItem;
                v.SpineConfig.WindowType = comboBox_WindowType.SelectedItem switch
                {
                    "碧蓝航线_后宅小人" => SpineWindow.SpineWindowType.AzurLaneSD,
                    "碧蓝航线_动态立绘" => SpineWindow.SpineWindowType.AzurLaneDynamic,
                    "明日方舟_动态立绘" => SpineWindow.SpineWindowType.ArknightsDynamic,
                    _ => SpineWindow.SpineWindowType.AzurLaneSD
                };

                // 设置 Spine 路径
                v.SpineConfig.SkelPath0 = string.IsNullOrEmpty(textBox_SkelPath0.Text) ? null : textBox_SkelPath0.Text;
                v.SpineConfig.SkelPath1 = string.IsNullOrEmpty(textBox_SkelPath1.Text) ? null : textBox_SkelPath1.Text;
                v.SpineConfig.SkelPath2 = string.IsNullOrEmpty(textBox_SkelPath2.Text) ? null : textBox_SkelPath2.Text;
                v.SpineConfig.SkelPath3 = string.IsNullOrEmpty(textBox_SkelPath3.Text) ? null : textBox_SkelPath3.Text;
                v.SpineConfig.SkelPath4 = string.IsNullOrEmpty(textBox_SkelPath4.Text) ? null : textBox_SkelPath4.Text;
                v.SpineConfig.SkelPath5 = string.IsNullOrEmpty(textBox_SkelPath5.Text) ? null : textBox_SkelPath5.Text;
                v.SpineConfig.SkelPath6 = string.IsNullOrEmpty(textBox_SkelPath6.Text) ? null : textBox_SkelPath6.Text;
                v.SpineConfig.SkelPath7 = string.IsNullOrEmpty(textBox_SkelPath7.Text) ? null : textBox_SkelPath7.Text;
                v.SpineConfig.SkelPath8 = string.IsNullOrEmpty(textBox_SkelPath8.Text) ? null : textBox_SkelPath8.Text;
                v.SpineConfig.SkelPath9 = string.IsNullOrEmpty(textBox_SkelPath9.Text) ? null : textBox_SkelPath9.Text;

                return v;
            }

            set
            {
                // 系统设置
                checkBox_AutoRun.Checked = value.SystemConfig.AutuRun;
                checkBox_Visible.Checked = value.SystemConfig.Visible;

                // 基础设置
                checkBox_WallpaperMode.Checked = value.BasicConfig.WallpaperMode;
                checkBox_MouseClickThrough.Checked = value.BasicConfig.MouseClickThrough;
                numericUpDown_PositionX.Value = value.BasicConfig.PositionX;
                numericUpDown_PositionY.Value = value.BasicConfig.PositionY;
                numericUpDown_SizeX.Value = value.BasicConfig.SizeX;
                numericUpDown_SizeY.Value = value.BasicConfig.SizeY;
                numericUpDown_SpinePositionX.Value = (decimal)value.BasicConfig.SpinePositionX;
                numericUpDown_SpinePositionY.Value = (decimal)value.BasicConfig.SpinePositionY;
                checkBox_SpineFlip.Checked = value.BasicConfig.SpineFlip;
                trackBar_SpineScale.Value = (int)(value.BasicConfig.SpineScale * 100);
                trackBar_Opacity.Value = value.BasicConfig.Opacity;
                trackBar_MaxFps.Value = (int)value.BasicConfig.MaxFps;
                checkBox_SpineUsePMA.Checked = value.BasicConfig.SpineUsePMA;
                comboBox_BackgroudColor.SelectedItem = value.BasicConfig.BackgroudColor switch
                {
                    SpineWindow.BackgroudColor.Black => "黑色",
                    SpineWindow.BackgroudColor.White => "白色",
                    SpineWindow.BackgroudColor.Gray => "灰色",
                    _ => "灰色"
                };
                textBox_ClearColor.Text = $"#{value.BasicConfig.ClearColor.ToInteger():X8}";

                // Spine 设置
                comboBox_SpineVersion.SelectedItem = value.SpineConfig.SpineVersion;
                comboBox_WindowType.SelectedItem = value.SpineConfig.WindowType switch
                {
                    SpineWindow.SpineWindowType.AzurLaneSD => "碧蓝航线_后宅小人",
                    SpineWindow.SpineWindowType.AzurLaneDynamic => "碧蓝航线_动态立绘",
                    SpineWindow.SpineWindowType.ArknightsDynamic => "明日方舟_动态立绘",
                    _ => "碧蓝航线_后宅小人",
                };
                textBox_SkelPath0.Text = value.SpineConfig.SkelPath0;
                textBox_SkelPath1.Text = value.SpineConfig.SkelPath1;
                textBox_SkelPath2.Text = value.SpineConfig.SkelPath2;
                textBox_SkelPath3.Text = value.SpineConfig.SkelPath3;
                textBox_SkelPath4.Text = value.SpineConfig.SkelPath4;
                textBox_SkelPath5.Text = value.SpineConfig.SkelPath5;
                textBox_SkelPath6.Text = value.SpineConfig.SkelPath6;
                textBox_SkelPath7.Text = value.SpineConfig.SkelPath7;
                textBox_SkelPath8.Text = value.SpineConfig.SkelPath8;
                textBox_SkelPath9.Text = value.SpineConfig.SkelPath9;
            }
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
                Value = Program.CurrentConfig;
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
            Program.ApplyConfig(Value);
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

        private void commandResetSpine_Click(object sender, EventArgs e)
        {
            Program.WindowSpine.ResetPositionAndSize();
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
