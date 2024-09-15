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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            tableLayoutPanel_About = new TableLayoutPanel();
            SuspendLayout();
            // 
            // tableLayoutPanel_About
            // 
            tableLayoutPanel_About.BackColor = Color.Transparent;
            tableLayoutPanel_About.ColumnCount = 2;
            tableLayoutPanel_About.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel_About.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            tableLayoutPanel_About.Dock = DockStyle.Fill;
            tableLayoutPanel_About.Location = new Point(0, 0);
            tableLayoutPanel_About.Margin = new Padding(0);
            tableLayoutPanel_About.Name = "tableLayoutPanel_About";
            tableLayoutPanel_About.RowCount = 6;
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel_About.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel_About.Size = new Size(798, 522);
            tableLayoutPanel_About.TabIndex = 0;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Zoom;
            ClientSize = new Size(798, 522);
            Controls.Add(tableLayoutPanel_About);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "关于 DeskSpine";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_About;
    }
}
