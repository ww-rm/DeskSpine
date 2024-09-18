namespace DeskSpine
{
    partial class AboutForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            tableLayoutPanel_About = new TableLayoutPanel();
            label3 = new Label();
            label1 = new Label();
            label_Version = new Label();
            linkLabel_RepoUrl = new LinkLabel();
            toolTip1 = new ToolTip(components);
            tableLayoutPanel_About.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel_About
            // 
            tableLayoutPanel_About.BackColor = Color.Transparent;
            tableLayoutPanel_About.ColumnCount = 2;
            tableLayoutPanel_About.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.5714283F));
            tableLayoutPanel_About.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 71.42857F));
            tableLayoutPanel_About.Controls.Add(label3, 0, 1);
            tableLayoutPanel_About.Controls.Add(label1, 0, 0);
            tableLayoutPanel_About.Controls.Add(label_Version, 1, 0);
            tableLayoutPanel_About.Controls.Add(linkLabel_RepoUrl, 1, 1);
            tableLayoutPanel_About.Dock = DockStyle.Fill;
            tableLayoutPanel_About.Location = new Point(0, 0);
            tableLayoutPanel_About.Margin = new Padding(0);
            tableLayoutPanel_About.Name = "tableLayoutPanel_About";
            tableLayoutPanel_About.RowCount = 6;
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666718F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666679F));
            tableLayoutPanel_About.Size = new Size(778, 544);
            tableLayoutPanel_About.TabIndex = 0;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(107, 121);
            label3.Name = "label3";
            label3.Size = new Size(112, 27);
            label3.TabIndex = 2;
            label3.Text = "项目地址：";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(107, 31);
            label1.Name = "label1";
            label1.Size = new Size(112, 27);
            label1.TabIndex = 0;
            label1.Text = "程序版本：";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label_Version
            // 
            label_Version.Anchor = AnchorStyles.Left;
            label_Version.AutoSize = true;
            label_Version.Location = new Point(225, 31);
            label_Version.Name = "label_Version";
            label_Version.Size = new Size(70, 27);
            label_Version.TabIndex = 1;
            label_Version.Text = "vX.Y.Z";
            label_Version.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // linkLabel_RepoUrl
            // 
            linkLabel_RepoUrl.Anchor = AnchorStyles.Left;
            linkLabel_RepoUrl.AutoSize = true;
            linkLabel_RepoUrl.Location = new Point(225, 121);
            linkLabel_RepoUrl.Name = "linkLabel_RepoUrl";
            linkLabel_RepoUrl.Size = new Size(377, 27);
            linkLabel_RepoUrl.TabIndex = 3;
            linkLabel_RepoUrl.TabStop = true;
            linkLabel_RepoUrl.Text = "https://github.com/ww-rm/DeskSpine";
            toolTip1.SetToolTip(linkLabel_RepoUrl, "单击复制到剪贴板，按下Ctrl单击直接访问");
            linkLabel_RepoUrl.LinkClicked += linkLabel_RepoUrl_LinkClicked;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(12F, 27F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(778, 544);
            Controls.Add(tableLayoutPanel_About);
            DoubleBuffered = true;
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(7, 7, 7, 7);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "关于 DeskSpine";
            TopMost = true;
            tableLayoutPanel_About.ResumeLayout(false);
            tableLayoutPanel_About.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_About;
        private Label label1;
        private Label label_Version;
        private Label label3;
        private LinkLabel linkLabel_RepoUrl;
        private ToolTip toolTip1;
    }
}
