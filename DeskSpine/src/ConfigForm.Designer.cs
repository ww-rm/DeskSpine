namespace DeskSpine
{
    partial class ConfigForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            notifyIcon = new NotifyIcon(components);
            contextMenuStrip = new ContextMenuStrip(components);
            commandShowSpine = new ToolStripMenuItem();
            commandMousePass = new ToolStripMenuItem();
            commandResetSpine = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            commandConfig = new ToolStripMenuItem();
            commandAbout = new ToolStripMenuItem();
            commandExit = new ToolStripMenuItem();
            系统设置 = new GroupBox();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
            notifyIcon.Text = "DeskSpine";
            notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.ImageScalingSize = new Size(24, 24);
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { commandShowSpine, commandMousePass, commandResetSpine, toolStripSeparator1, commandConfig, commandAbout, commandExit });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(153, 190);
            // 
            // commandShowSpine
            // 
            commandShowSpine.Name = "commandShowSpine";
            commandShowSpine.Size = new Size(152, 30);
            commandShowSpine.Text = "显示精灵";
            // 
            // commandMousePass
            // 
            commandMousePass.Name = "commandMousePass";
            commandMousePass.Size = new Size(152, 30);
            commandMousePass.Text = "鼠标穿透";
            // 
            // commandResetSpine
            // 
            commandResetSpine.Name = "commandResetSpine";
            commandResetSpine.Size = new Size(152, 30);
            commandResetSpine.Text = "精灵复位";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(149, 6);
            // 
            // commandConfig
            // 
            commandConfig.Name = "commandConfig";
            commandConfig.Size = new Size(152, 30);
            commandConfig.Text = "设置";
            // 
            // commandAbout
            // 
            commandAbout.Name = "commandAbout";
            commandAbout.Size = new Size(152, 30);
            commandAbout.Text = "关于";
            // 
            // commandExit
            // 
            commandExit.Name = "commandExit";
            commandExit.Size = new Size(152, 30);
            commandExit.Text = "退出";
            // 
            // 系统设置
            // 
            系统设置.Location = new Point(12, 12);
            系统设置.Name = "系统设置";
            系统设置.Size = new Size(300, 306);
            系统设置.TabIndex = 1;
            系统设置.TabStop = false;
            系统设置.Text = "groupBox1";
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(824, 641);
            Controls.Add(系统设置);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ConfigForm";
            Text = "设置";
            TopMost = true;
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem commandExit;
        private ToolStripMenuItem commandAbout;
        private ToolStripMenuItem commandShowSpine;
        private ToolStripMenuItem commandMousePass;
        private ToolStripMenuItem commandResetSpine;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem commandConfig;
        private GroupBox 系统设置;
    }
}
