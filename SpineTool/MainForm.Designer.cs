namespace SpineTool
{
    partial class MainForm
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
            tabControl_Tools = new TabControl();
            tabPage_Exporter = new TabPage();
            splitContainer_Exporter = new SplitContainer();
            tabPage_FixEdge = new TabPage();
            tabPage_FixPMA = new TabPage();
            tableLayoutPanel_SpineTool = new TableLayoutPanel();
            tableLayoutPanel_Progress = new TableLayoutPanel();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            tableLayoutPanel_View = new TableLayoutPanel();
            tableLayoutPanel_ViewSet = new TableLayoutPanel();
            label3 = new Label();
            numericUpDown_PreviewScale = new NumericUpDown();
            button_PreviewSize = new Button();
            panel_PreviewContainer = new Panel();
            pictureBox1 = new PictureBox();
            tabControl_Tools.SuspendLayout();
            tabPage_Exporter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer_Exporter).BeginInit();
            splitContainer_Exporter.Panel2.SuspendLayout();
            splitContainer_Exporter.SuspendLayout();
            tableLayoutPanel_SpineTool.SuspendLayout();
            tableLayoutPanel_Progress.SuspendLayout();
            tableLayoutPanel_View.SuspendLayout();
            tableLayoutPanel_ViewSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_PreviewScale).BeginInit();
            panel_PreviewContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tabControl_Tools
            // 
            tabControl_Tools.Controls.Add(tabPage_Exporter);
            tabControl_Tools.Controls.Add(tabPage_FixEdge);
            tabControl_Tools.Controls.Add(tabPage_FixPMA);
            tabControl_Tools.Dock = DockStyle.Fill;
            tabControl_Tools.Location = new Point(0, 0);
            tabControl_Tools.Margin = new Padding(0);
            tabControl_Tools.Name = "tabControl_Tools";
            tabControl_Tools.SelectedIndex = 0;
            tabControl_Tools.Size = new Size(1485, 896);
            tabControl_Tools.TabIndex = 0;
            // 
            // tabPage_Exporter
            // 
            tabPage_Exporter.BackColor = SystemColors.Control;
            tabPage_Exporter.Controls.Add(splitContainer_Exporter);
            tabPage_Exporter.Location = new Point(4, 36);
            tabPage_Exporter.Margin = new Padding(0);
            tabPage_Exporter.Name = "tabPage_Exporter";
            tabPage_Exporter.Size = new Size(1477, 856);
            tabPage_Exporter.TabIndex = 0;
            tabPage_Exporter.Text = "动画导出工具";
            // 
            // splitContainer_Exporter
            // 
            splitContainer_Exporter.Dock = DockStyle.Fill;
            splitContainer_Exporter.Location = new Point(0, 0);
            splitContainer_Exporter.Margin = new Padding(0);
            splitContainer_Exporter.Name = "splitContainer_Exporter";
            // 
            // splitContainer_Exporter.Panel1
            // 
            splitContainer_Exporter.Panel1.AutoScroll = true;
            // 
            // splitContainer_Exporter.Panel2
            // 
            splitContainer_Exporter.Panel2.Controls.Add(tableLayoutPanel_View);
            splitContainer_Exporter.Size = new Size(1477, 856);
            splitContainer_Exporter.SplitterDistance = 504;
            splitContainer_Exporter.TabIndex = 0;
            // 
            // tabPage_FixEdge
            // 
            tabPage_FixEdge.BackColor = SystemColors.Control;
            tabPage_FixEdge.Location = new Point(4, 33);
            tabPage_FixEdge.Margin = new Padding(0);
            tabPage_FixEdge.Name = "tabPage_FixEdge";
            tabPage_FixEdge.Size = new Size(1477, 859);
            tabPage_FixEdge.TabIndex = 1;
            tabPage_FixEdge.Text = "边缘修复工具";
            // 
            // tabPage_FixPMA
            // 
            tabPage_FixPMA.BackColor = SystemColors.Control;
            tabPage_FixPMA.Location = new Point(4, 33);
            tabPage_FixPMA.Margin = new Padding(0);
            tabPage_FixPMA.Name = "tabPage_FixPMA";
            tabPage_FixPMA.Size = new Size(1477, 859);
            tabPage_FixPMA.TabIndex = 2;
            tabPage_FixPMA.Text = "PMA修复工具";
            // 
            // tableLayoutPanel_SpineTool
            // 
            tableLayoutPanel_SpineTool.ColumnCount = 1;
            tableLayoutPanel_SpineTool.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_SpineTool.Controls.Add(tabControl_Tools, 0, 0);
            tableLayoutPanel_SpineTool.Controls.Add(tableLayoutPanel_Progress, 0, 1);
            tableLayoutPanel_SpineTool.Dock = DockStyle.Fill;
            tableLayoutPanel_SpineTool.Location = new Point(0, 0);
            tableLayoutPanel_SpineTool.Margin = new Padding(0);
            tableLayoutPanel_SpineTool.Name = "tableLayoutPanel_SpineTool";
            tableLayoutPanel_SpineTool.RowCount = 2;
            tableLayoutPanel_SpineTool.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_SpineTool.RowStyles.Add(new RowStyle());
            tableLayoutPanel_SpineTool.Size = new Size(1485, 936);
            tableLayoutPanel_SpineTool.TabIndex = 1;
            // 
            // tableLayoutPanel_Progress
            // 
            tableLayoutPanel_Progress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel_Progress.AutoSize = true;
            tableLayoutPanel_Progress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel_Progress.ColumnCount = 2;
            tableLayoutPanel_Progress.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_Progress.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_Progress.Controls.Add(progressBar1, 1, 0);
            tableLayoutPanel_Progress.Controls.Add(label1, 0, 0);
            tableLayoutPanel_Progress.Location = new Point(0, 896);
            tableLayoutPanel_Progress.Margin = new Padding(0);
            tableLayoutPanel_Progress.Name = "tableLayoutPanel_Progress";
            tableLayoutPanel_Progress.RowCount = 1;
            tableLayoutPanel_Progress.RowStyles.Add(new RowStyle());
            tableLayoutPanel_Progress.Size = new Size(1485, 40);
            tableLayoutPanel_Progress.TabIndex = 1;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(121, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1361, 34);
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Location = new Point(3, 6);
            label1.Name = "label1";
            label1.Size = new Size(112, 27);
            label1.TabIndex = 1;
            label1.Text = "处理进度：";
            // 
            // tableLayoutPanel_View
            // 
            tableLayoutPanel_View.ColumnCount = 1;
            tableLayoutPanel_View.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_View.Controls.Add(tableLayoutPanel_ViewSet, 0, 1);
            tableLayoutPanel_View.Controls.Add(panel_PreviewContainer, 0, 0);
            tableLayoutPanel_View.Dock = DockStyle.Fill;
            tableLayoutPanel_View.Location = new Point(0, 0);
            tableLayoutPanel_View.Margin = new Padding(0);
            tableLayoutPanel_View.Name = "tableLayoutPanel_View";
            tableLayoutPanel_View.RowCount = 2;
            tableLayoutPanel_View.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_View.RowStyles.Add(new RowStyle());
            tableLayoutPanel_View.Size = new Size(969, 856);
            tableLayoutPanel_View.TabIndex = 0;
            // 
            // tableLayoutPanel_ViewSet
            // 
            tableLayoutPanel_ViewSet.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel_ViewSet.AutoSize = true;
            tableLayoutPanel_ViewSet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel_ViewSet.ColumnCount = 4;
            tableLayoutPanel_ViewSet.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_ViewSet.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_ViewSet.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_ViewSet.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_ViewSet.Controls.Add(label3, 1, 0);
            tableLayoutPanel_ViewSet.Controls.Add(numericUpDown_PreviewScale, 2, 0);
            tableLayoutPanel_ViewSet.Controls.Add(button_PreviewSize, 0, 0);
            tableLayoutPanel_ViewSet.Location = new Point(0, 813);
            tableLayoutPanel_ViewSet.Margin = new Padding(0);
            tableLayoutPanel_ViewSet.Name = "tableLayoutPanel_ViewSet";
            tableLayoutPanel_ViewSet.RowCount = 1;
            tableLayoutPanel_ViewSet.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_ViewSet.Size = new Size(969, 43);
            tableLayoutPanel_ViewSet.TabIndex = 0;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Location = new Point(332, 8);
            label3.Name = "label3";
            label3.Size = new Size(72, 27);
            label3.TabIndex = 2;
            label3.Text = "缩放：";
            // 
            // numericUpDown_PreviewScale
            // 
            numericUpDown_PreviewScale.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_PreviewScale.Location = new Point(410, 5);
            numericUpDown_PreviewScale.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown_PreviewScale.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_PreviewScale.Name = "numericUpDown_PreviewScale";
            numericUpDown_PreviewScale.Size = new Size(109, 33);
            numericUpDown_PreviewScale.TabIndex = 3;
            numericUpDown_PreviewScale.TextAlign = HorizontalAlignment.Right;
            numericUpDown_PreviewScale.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // button_PreviewSize
            // 
            button_PreviewSize.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button_PreviewSize.AutoSize = true;
            button_PreviewSize.Cursor = Cursors.Hand;
            button_PreviewSize.Location = new Point(3, 3);
            button_PreviewSize.Name = "button_PreviewSize";
            button_PreviewSize.Size = new Size(323, 37);
            button_PreviewSize.TabIndex = 4;
            button_PreviewSize.Text = "画布大小（像素）：[1920, 1080]";
            button_PreviewSize.UseVisualStyleBackColor = true;
            // 
            // panel_PreviewContainer
            // 
            panel_PreviewContainer.Controls.Add(pictureBox1);
            panel_PreviewContainer.Dock = DockStyle.Fill;
            panel_PreviewContainer.Location = new Point(3, 3);
            panel_PreviewContainer.Name = "panel_PreviewContainer";
            panel_PreviewContainer.Size = new Size(963, 807);
            panel_PreviewContainer.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(411, 425);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(150, 75);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 27F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1485, 936);
            Controls.Add(tableLayoutPanel_SpineTool);
            Font = new Font("Microsoft YaHei UI", 10F);
            Name = "MainForm";
            Text = "SpineTool";
            tabControl_Tools.ResumeLayout(false);
            tabPage_Exporter.ResumeLayout(false);
            splitContainer_Exporter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer_Exporter).EndInit();
            splitContainer_Exporter.ResumeLayout(false);
            tableLayoutPanel_SpineTool.ResumeLayout(false);
            tableLayoutPanel_SpineTool.PerformLayout();
            tableLayoutPanel_Progress.ResumeLayout(false);
            tableLayoutPanel_Progress.PerformLayout();
            tableLayoutPanel_View.ResumeLayout(false);
            tableLayoutPanel_View.PerformLayout();
            tableLayoutPanel_ViewSet.ResumeLayout(false);
            tableLayoutPanel_ViewSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_PreviewScale).EndInit();
            panel_PreviewContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl_Tools;
        private TabPage tabPage_Exporter;
        private TabPage tabPage_FixEdge;
        private TabPage tabPage_FixPMA;
        private SplitContainer splitContainer_Exporter;
        private TableLayoutPanel tableLayoutPanel_SpineTool;
        private TableLayoutPanel tableLayoutPanel_Progress;
        private ProgressBar progressBar1;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel_View;
        private TableLayoutPanel tableLayoutPanel_ViewSet;
        private Label label3;
        private NumericUpDown numericUpDown_PreviewScale;
        private Button button_PreviewSize;
        private Panel panel_PreviewContainer;
        private PictureBox pictureBox1;
    }
}
