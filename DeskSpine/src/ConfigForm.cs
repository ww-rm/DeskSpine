using System.Diagnostics;

namespace DeskSpine
{
    public partial class ConfigForm : Form
    {
        /// <summary>
        /// 版本下拉框映射表
        /// </summary>
        protected static Dictionary<string, Spine.SpineVersion> comboBox_SpineVersion_KV = new()
        {
            { "3.6.x", Spine.SpineVersion.V36 },
            { "3.8.x", Spine.SpineVersion.V38 },
        };

        /// <summary>
        /// 交互方案下拉框映射表
        /// </summary>
        protected static Dictionary<string, TinyEngine.AnimatorType> comboBox_WindowType_KV = new()
        {
            { "碧蓝航线_后宅小人", TinyEngine.AnimatorType.AzurLaneSD },
            { "碧蓝航线_动态立绘", TinyEngine.AnimatorType.AzurLaneDynamic },
            { "明日方舟_动态立绘", TinyEngine.AnimatorType.ArknightsDynamic },
            { "明日方舟_基建小人", TinyEngine.AnimatorType.ArknightsBuild },
            { "明日方舟_战斗小人", TinyEngine.AnimatorType.ArknightsBattle },
        };

        private ShellNotifyIcon shellNotifyIcon;
        private System.Timers.Timer hourlyChimeTimer = new(100) { AutoReset = true };
        private bool hasChimed = false;

        public ConfigForm()
        {
            InitializeComponent();
            _ = Handle; // 强制创建窗口
            shellNotifyIcon = new(notifyIcon);
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            notifyIcon.Icon = (Icon?)(SystemValue.SystemUseLightTheme ? resources.GetObject("$this.Icon") : resources.GetObject("notifyIcon.Icon"));
            hourlyChimeTimer.Elapsed += HourlyChimeTimer_Elapsed;
        }

        /// <summary>
        /// 气泡图标
        /// </summary>
        public string? BalloonIconPath
        {
            get => balloonIconPath;
            set
            {
                value = string.IsNullOrEmpty(value) ? null : value;
                if (value == balloonIconPath) return;
                if (value is null)
                {
                    balloonIconPath = null;
                    balloonIcon = null;
                }
                else
                {
                    Bitmap newIcon = null;
                    try
                    {
                        newIcon = new Bitmap(value);
                        balloonIcon?.Dispose();
                        balloonIconPath = value;
                        balloonIcon = newIcon;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, $"{value} 加载失败\n\n{ex}", "气泡图标资源加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private string? balloonIconPath;
        private Bitmap? balloonIcon;

        /// <summary>
        /// 获取和填充设置项
        /// </summary>
        public Config Value
        {
            get
            {
                var v = new Config();

                // 系统设置
                v.SystemConfig.AutuRun = checkBox_AutoRun.Checked;
                v.SystemConfig.Visible = checkBox_Visible.Checked;
                v.SystemConfig.BalloonIconPath = string.IsNullOrEmpty(textBox_BalloonIconPath.Text) ? null : textBox_BalloonIconPath.Text;
                v.SystemConfig.HourlyChime = checkBox_HourlyChime.Checked;

                // 基础设置
                v.BasicConfig.WallpaperMode = checkBox_WallpaperMode.Checked;
                v.BasicConfig.MouseClickThrough = checkBox_MouseClickThrough.Checked;
                v.BasicConfig.PositionX = numericUpDown_PositionX.Enabled ? (int)numericUpDown_PositionX.Value : Program.SpineWindow.Position.X;
                v.BasicConfig.PositionY = numericUpDown_PositionY.Enabled ? (int)numericUpDown_PositionY.Value : Program.SpineWindow.Position.Y;
                v.BasicConfig.SizeX = numericUpDown_SizeX.Enabled ? (uint)numericUpDown_SizeX.Value : Program.SpineWindow.Size.X;
                v.BasicConfig.SizeY = numericUpDown_SizeY.Enabled ? (uint)numericUpDown_SizeY.Value : Program.SpineWindow.Size.Y;
                v.BasicConfig.SpinePositionX = numericUpDown_SpinePositionX.Enabled ? (float)numericUpDown_SpinePositionX.Value : Program.SpineWindow.Spine.Position.X;
                v.BasicConfig.SpinePositionY = numericUpDown_SpinePositionY.Enabled ? (float)numericUpDown_SpinePositionY.Value : Program.SpineWindow.Spine.Position.Y;
                v.BasicConfig.SpineFlip = checkBox_SpineFlip.Checked;
                v.BasicConfig.SpineScale = trackBar_SpineScale.Value / 100.0f;
                v.BasicConfig.Opacity = (byte)trackBar_Opacity.Value;
                v.BasicConfig.MaxFps = (uint)trackBar_MaxFps.Value;
                v.BasicConfig.BackgroundColor = new((byte)numericUpDown_BackgroundColorR.Value,
                                                    (byte)numericUpDown_BackgroundColorG.Value,
                                                    (byte)numericUpDown_BackgroundColorB.Value,
                                                    0);
                v.BasicConfig.SpineUsePMA = checkBox_SpineUsePMA.Checked;

                // Spine 设置
                v.SpineConfig.SpineVersion = (Spine.SpineVersion)comboBox_SpineVersion.SelectedValue;
                v.SpineConfig.InteractMode = (TinyEngine.AnimatorType)comboBox_WindowType.SelectedValue;

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
                textBox_BalloonIconPath.Text = value.SystemConfig.BalloonIconPath;
                checkBox_HourlyChime.Checked = value.SystemConfig.HourlyChime;

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
                numericUpDown_BackgroundColorR.Value = value.BasicConfig.BackgroundColor.R;
                numericUpDown_BackgroundColorG.Value = value.BasicConfig.BackgroundColor.G;
                numericUpDown_BackgroundColorB.Value = value.BasicConfig.BackgroundColor.B;
                checkBox_SpineUsePMA.Checked = value.BasicConfig.SpineUsePMA;

                // Spine 设置
                comboBox_SpineVersion.SelectedValue = value.SpineConfig.SpineVersion;
                comboBox_WindowType.SelectedValue = value.SpineConfig.InteractMode;
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

        /// <summary>
        /// 整点报时功能
        /// </summary>
        public bool HourlyChime { get => hourlyChimeTimer.Enabled; set => hourlyChimeTimer.Enabled = value; }

        /// <summary>
        /// 使用设置项弹出气泡消息
        /// </summary>
        public void ShowBalloonTip(string title, string info)
        {
            if (balloonIcon is null) { ShowBalloonTip(title, info, ToolTipIcon.None); }
            else { ShowBalloonTip(title, info, balloonIcon.GetHicon()); }
        }

        /// <summary>
        /// 使用指定图标弹出气泡消息
        /// </summary>
        public void ShowBalloonTip(string title, string info, IntPtr balloonIcon)
        {
            title ??= ""; info ??= "";
            if (title.Length <= 0 && info.Length <= 0) info = "~";
            shellNotifyIcon.ShowBalloonTip(title, info, balloonIcon);
        }

        /// <summary>
        /// 使用系统图标弹出气泡消息
        /// </summary>
        public void ShowBalloonTip(string title, string info, ToolTipIcon balloonIcon)
        {
            title ??= ""; info ??= "";
            if (title.Length <= 0 && info.Length <= 0) info = "~";
            notifyIcon.ShowBalloonTip(5, title, info, balloonIcon);
        }

        #region 窗体事件

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            comboBox_SpineVersion.DataSource = new BindingSource(comboBox_SpineVersion_KV, null);
            comboBox_SpineVersion.DisplayMember = "Key";
            comboBox_SpineVersion.ValueMember = "Value";
            comboBox_SpineVersion.SelectedValue = Spine.SpineVersion.V38;

            comboBox_WindowType.DataSource = new BindingSource(comboBox_WindowType_KV, null);
            comboBox_WindowType.DisplayMember = "Key";
            comboBox_WindowType.ValueMember = "Value";
            comboBox_WindowType.SelectedValue = TinyEngine.AnimatorType.AzurLaneSD;
        }

        private void ConfigForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                Value = Program.CurrentConfig;

                // 恢复默认显示
                Size = new(1600, 900);
                CenterToScreen();
                tabControl_Config.SelectTab(0);

                // 默认锁定位置大小调整
                numericUpDown_PositionX.Enabled = false;
                numericUpDown_PositionY.Enabled = false;
                numericUpDown_SizeX.Enabled = false;
                numericUpDown_SizeY.Enabled = false;
                numericUpDown_SpinePositionX.Enabled = false;
                numericUpDown_SpinePositionY.Enabled = false;
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

        private void button_OpenDataFolder_Click(object sender, EventArgs e)
        {
            try 
            { 
                Process.Start("explorer.exe", Program.ProgramDataDirectory); 
            }
            catch (Exception ex) 
            {
                MessageBox.Show(this, $"无法打开文件夹: {ex.Message}", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            button_Apply_Click(sender, e);
            this.Hide();
        }

        private void button_Apply_Click(object sender, EventArgs e)
        {
            Program.CurrentConfig = Value;
            Value = Program.CurrentConfig; // 刷新页面值
        }

        private void HourlyChimeTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Minute == 0)
            {
                if (!hasChimed)
                {
                    hasChimed = true;
                    ShowBalloonTip($"北京时间 {now.Hour:d2}: {now.Minute:d2}", "Take a break~");
                }
            }
            else
            {
                hasChimed = false;
            }
        }

        #endregion

        #region 通知栏图标事件

        private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var wndSize = Program.PerfMonitorForm.Size;
                var iconRect = shellNotifyIcon.Rectangle;
                var halfDeltaWidth = (wndSize.Width - iconRect.Width) / 2;
                var halfDeltaHeight = (wndSize.Height - iconRect.Height) / 2;
                Point location = SystemValue.TaskbarDirection switch
                {
                    EdgeDirection.Left => new(iconRect.Right, iconRect.Top - halfDeltaHeight),
                    EdgeDirection.Top => new(iconRect.Left - halfDeltaWidth, iconRect.Bottom),
                    EdgeDirection.Right => new(iconRect.Left - wndSize.Width, iconRect.Top - halfDeltaHeight),
                    EdgeDirection.Bottom => new(iconRect.Left - halfDeltaWidth, iconRect.Top - wndSize.Height),
                    _ => throw new NotImplementedException()
                };
                Program.PerfMonitorForm.Popup(location);
            }
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                commandConfig_Click(sender, EventArgs.Empty);
            }
        }

        #endregion

        #region 菜单命令事件

        private void contextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var currentConfig = Program.CurrentConfig;
            commandShowSpine.Checked = currentConfig.SystemConfig.Visible;
            commandWallpaperMode.Checked = currentConfig.BasicConfig.WallpaperMode;
            commandMouseClickThrough.Checked = currentConfig.BasicConfig.MouseClickThrough;
        }

        private void commandShowSpine_Click(object sender, EventArgs e)
        {
            var currentConfig = Program.CurrentConfig;
            if (commandShowSpine.Checked)
            {
                Program.SpineWindow.Visible = false;
                currentConfig.SystemConfig.Visible = false;
            }
            else
            {
                Program.SpineWindow.Visible = true;
                currentConfig.SystemConfig.Visible = true;
            }
            Program.LocalConfig = currentConfig;
        }

        private void commandWallpaperMode_Click(object sender, EventArgs e)
        {
            var currentConfig = Program.CurrentConfig;
            if (commandWallpaperMode.Checked)
            {
                Program.SpineWindow.WallpaperMode = false;
                currentConfig.BasicConfig.WallpaperMode = false;
            }
            else
            {
                Program.SpineWindow.WallpaperMode = true;
                currentConfig.BasicConfig.WallpaperMode = true;
            }
            Program.LocalConfig = currentConfig;
        }

        private void commandMouseClickThrough_Click(object sender, EventArgs e)
        {
            var currentConfig = Program.CurrentConfig;
            if (commandMouseClickThrough.Checked)
            {
                Program.SpineWindow.MouseClickThrough = false;
                currentConfig.BasicConfig.MouseClickThrough = false;
            }
            else
            {
                Program.SpineWindow.MouseClickThrough = true;
                currentConfig.BasicConfig.MouseClickThrough = true;
            }
            Program.LocalConfig = currentConfig;
        }

        private void commandSetFullScreen_Click(object sender, EventArgs e)
        {
            var screenBounds = Screen.FromHandle(Program.SpineWindow.Handle).Bounds;
            var screenPosition = screenBounds.Location;
            var screenSize = screenBounds.Size;
            Program.SpineWindow.Position = new(screenPosition.X, screenPosition.Y);
            Program.SpineWindow.Size = new((uint)screenSize.Width, (uint)screenSize.Height);
            Program.LocalConfig = Program.CurrentConfig;
        }

        private void commandResetSpine_Click(object sender, EventArgs e)
        {
            Program.SpineWindow.ResetPositionAndSize();
            Program.LocalConfig = Program.CurrentConfig;
        }

        private void commandSpineTool_Click(object sender, EventArgs e)
        {
            var spineToolPath = Path.Combine(Program.ProgramDirectory, "SpineTool.exe");
            try
            {
                Process.Start(spineToolPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"SpineTool 启动失败\n\n{ex}", Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void commandConfig_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.Activate();
            this.Focus();
            this.BringToFront();
        }

        private void commandAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
        }

        private void commandExit_Click(object? sender, EventArgs e)
        {
            Program.LocalConfig = Program.CurrentConfig;
            Application.Exit();
        }

        #endregion

        #region 系统设置控件事件

        private void button_SelectBalloonIconPath_Click(object sender, EventArgs e)
        {
            openFileDialog_BalloonIconPath.InitialDirectory = Path.GetDirectoryName(textBox_BalloonIconPath.Text);
            if (openFileDialog_BalloonIconPath.ShowDialog() == DialogResult.OK)
            {
                textBox_BalloonIconPath.Text = Path.GetFullPath(openFileDialog_BalloonIconPath.FileName);
            }
        }

        private void label_BalloonIcon_Click(object sender, EventArgs e)
        {
            ShowBalloonTip("叮——", "这是现在的图标噢");
        }

        #endregion

        #region 基础设置控件事件

        private void label_Position_Click(object sender, EventArgs e)
        {
            numericUpDown_PositionX.Enabled = !numericUpDown_PositionX.Enabled;
            numericUpDown_PositionY.Enabled = !numericUpDown_PositionY.Enabled;
        }
        private void label_Size_Click(object sender, EventArgs e)
        {
            numericUpDown_SizeX.Enabled = !numericUpDown_SizeX.Enabled;
            numericUpDown_SizeY.Enabled = !numericUpDown_SizeY.Enabled;
        }
        private void label_SpinePosition_Click(object sender, EventArgs e)
        {
            numericUpDown_SpinePositionX.Enabled = !numericUpDown_SpinePositionX.Enabled;
            numericUpDown_SpinePositionY.Enabled = !numericUpDown_SpinePositionY.Enabled;
        }

        private void trackBar_SpineScale_ValueChanged(object sender, EventArgs e) { label_SpineScale.Text = $"{trackBar_SpineScale.Value}"; }
        private void trackBar_Opacity_ValueChanged(object sender, EventArgs e) { label_Opacity.Text = $"{trackBar_Opacity.Value}"; }
        private void trackBar_MaxFps_ValueChanged(object sender, EventArgs e) { label_MaxFps.Text = $"{trackBar_MaxFps.Value}"; }

        private void numericUpDown_BackgroundColorR_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_BackgroundColorB.Value != numericUpDown_BackgroundColorR.Value)
                numericUpDown_BackgroundColorB.Value = numericUpDown_BackgroundColorR.Value;
        }
        private void numericUpDown_BackgroundColorG_ValueChanged(object sender, EventArgs e) { }
        private void numericUpDown_BackgroundColorB_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown_BackgroundColorR.Value != numericUpDown_BackgroundColorB.Value)
                numericUpDown_BackgroundColorR.Value = numericUpDown_BackgroundColorB.Value;
        }

        #endregion

        #region Spine 设置控件事件

        private void button_SelectSkel_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Name.Substring(button.Name.Length - 1)); // 从按钮名称中提取索引
            var textBox = Controls.Find($"textBox_SkelPath{index}", true).FirstOrDefault() as TextBox;

            openFileDialog_SelectSkel.InitialDirectory = Path.GetDirectoryName(textBox.Text);
            if (openFileDialog_SelectSkel.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = Path.GetFullPath(openFileDialog_SelectSkel.FileName);
            }
        }

        private void button_ClearSkel_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            int index = int.Parse(button.Name.Substring(button.Name.Length - 1)); // 从按钮名称中提取索引
            var textBox = Controls.Find($"textBox_SkelPath{index}", true).FirstOrDefault() as TextBox;
            textBox.Text = string.Empty;
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                // WM_SETTINGCHANGE
                case 0x001A:
                    var useLightTheme = SystemValue.SystemUseLightTheme;
                    var resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
                    Program.PerfMonitorForm.UseLightTheme = useLightTheme;
                    notifyIcon.Icon = (Icon?)(useLightTheme ? resources.GetObject("$this.Icon") : resources.GetObject("notifyIcon.Icon"));
                    break;
            }
        }

    }
}
