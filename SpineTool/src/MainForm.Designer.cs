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
            tableLayoutPanel_ExporterOptions = new TableLayoutPanel();
            button_Export = new Button();
            label2 = new Label();
            comboBox_SpineVersion = new ComboBox();
            button_SelectSkels = new Button();
            label6 = new Label();
            checkBox_UsePMA = new CheckBox();
            label4 = new Label();
            numericUpDown_SizeX = new NumericUpDown();
            label5 = new Label();
            numericUpDown_SizeY = new NumericUpDown();
            label7 = new Label();
            numericUpDown_Fps = new NumericUpDown();
            tableLayoutPanel_View = new TableLayoutPanel();
            tableLayoutPanel_ViewSet = new TableLayoutPanel();
            label_PreviewSize = new Label();
            button_ResetTimeline = new Button();
            label3 = new Label();
            numericUpDown_PreviewScale = new NumericUpDown();
            panel_PreviewContainer = new Panel();
            panel_Preview = new Panel();
            tabPage_FixEdge = new TabPage();
            tabPage_FixPMA = new TabPage();
            tableLayoutPanel_SpineTool = new TableLayoutPanel();
            tableLayoutPanel_Progress = new TableLayoutPanel();
            progressBar_SpineTool = new ProgressBar();
            label1 = new Label();
            tabControl_Tools.SuspendLayout();
            tabPage_Exporter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer_Exporter).BeginInit();
            splitContainer_Exporter.Panel1.SuspendLayout();
            splitContainer_Exporter.Panel2.SuspendLayout();
            splitContainer_Exporter.SuspendLayout();
            tableLayoutPanel_ExporterOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_SizeX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_SizeY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_Fps).BeginInit();
            tableLayoutPanel_View.SuspendLayout();
            tableLayoutPanel_ViewSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_PreviewScale).BeginInit();
            panel_PreviewContainer.SuspendLayout();
            tableLayoutPanel_SpineTool.SuspendLayout();
            tableLayoutPanel_Progress.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl_Tools
            // 
            tabControl_Tools.Controls.Add(tabPage_Exporter);
            tabControl_Tools.Controls.Add(tabPage_FixEdge);
            tabControl_Tools.Controls.Add(tabPage_FixPMA);
            tabControl_Tools.Dock = DockStyle.Fill;
            tabControl_Tools.Location = new Point(1, 1);
            tabControl_Tools.Margin = new Padding(0);
            tabControl_Tools.Name = "tabControl_Tools";
            tabControl_Tools.SelectedIndex = 0;
            tabControl_Tools.Size = new Size(1546, 880);
            tabControl_Tools.TabIndex = 0;
            // 
            // tabPage_Exporter
            // 
            tabPage_Exporter.BackColor = SystemColors.Control;
            tabPage_Exporter.Controls.Add(splitContainer_Exporter);
            tabPage_Exporter.Location = new Point(4, 36);
            tabPage_Exporter.Margin = new Padding(0);
            tabPage_Exporter.Name = "tabPage_Exporter";
            tabPage_Exporter.Size = new Size(1538, 840);
            tabPage_Exporter.TabIndex = 0;
            tabPage_Exporter.Text = "动画导出工具";
            tabPage_Exporter.Enter += tabPage_Exporter_Enter;
            tabPage_Exporter.Leave += tabPage_Exporter_Leave;
            // 
            // splitContainer_Exporter
            // 
            splitContainer_Exporter.BorderStyle = BorderStyle.FixedSingle;
            splitContainer_Exporter.Dock = DockStyle.Fill;
            splitContainer_Exporter.Location = new Point(0, 0);
            splitContainer_Exporter.Margin = new Padding(0);
            splitContainer_Exporter.Name = "splitContainer_Exporter";
            // 
            // splitContainer_Exporter.Panel1
            // 
            splitContainer_Exporter.Panel1.AutoScroll = true;
            splitContainer_Exporter.Panel1.Controls.Add(tableLayoutPanel_ExporterOptions);
            // 
            // splitContainer_Exporter.Panel2
            // 
            splitContainer_Exporter.Panel2.Controls.Add(tableLayoutPanel_View);
            splitContainer_Exporter.Size = new Size(1538, 840);
            splitContainer_Exporter.SplitterDistance = 522;
            splitContainer_Exporter.TabIndex = 0;
            // 
            // tableLayoutPanel_ExporterOptions
            // 
            tableLayoutPanel_ExporterOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel_ExporterOptions.ColumnCount = 2;
            tableLayoutPanel_ExporterOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.2290077F));
            tableLayoutPanel_ExporterOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.7709923F));
            tableLayoutPanel_ExporterOptions.Controls.Add(button_Export, 0, 8);
            tableLayoutPanel_ExporterOptions.Controls.Add(label2, 0, 1);
            tableLayoutPanel_ExporterOptions.Controls.Add(comboBox_SpineVersion, 1, 1);
            tableLayoutPanel_ExporterOptions.Controls.Add(button_SelectSkels, 0, 0);
            tableLayoutPanel_ExporterOptions.Controls.Add(label6, 0, 2);
            tableLayoutPanel_ExporterOptions.Controls.Add(checkBox_UsePMA, 1, 2);
            tableLayoutPanel_ExporterOptions.Controls.Add(label4, 0, 3);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_SizeX, 1, 3);
            tableLayoutPanel_ExporterOptions.Controls.Add(label5, 0, 4);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_SizeY, 1, 4);
            tableLayoutPanel_ExporterOptions.Controls.Add(label7, 0, 5);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_Fps, 1, 5);
            tableLayoutPanel_ExporterOptions.Location = new Point(0, 0);
            tableLayoutPanel_ExporterOptions.Name = "tableLayoutPanel_ExporterOptions";
            tableLayoutPanel_ExporterOptions.RowCount = 9;
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099606F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1099644F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1151323F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 11.1151323F));
            tableLayoutPanel_ExporterOptions.Size = new Size(520, 616);
            tableLayoutPanel_ExporterOptions.TabIndex = 0;
            // 
            // button_Export
            // 
            button_Export.AutoSize = true;
            button_Export.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel_ExporterOptions.SetColumnSpan(button_Export, 2);
            button_Export.Cursor = Cursors.Hand;
            button_Export.Dock = DockStyle.Fill;
            button_Export.Location = new Point(3, 547);
            button_Export.Name = "button_Export";
            button_Export.Size = new Size(514, 66);
            button_Export.TabIndex = 5;
            button_Export.Text = "点击导出...";
            button_Export.UseVisualStyleBackColor = true;
            button_Export.Click += button_Export_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(32, 88);
            label2.Margin = new Padding(32, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(131, 27);
            label2.TabIndex = 2;
            label2.Text = "Spine 版本：";
            // 
            // comboBox_SpineVersion
            // 
            comboBox_SpineVersion.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SpineVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SpineVersion.FormattingEnabled = true;
            comboBox_SpineVersion.Location = new Point(238, 84);
            comboBox_SpineVersion.Name = "comboBox_SpineVersion";
            comboBox_SpineVersion.Size = new Size(279, 35);
            comboBox_SpineVersion.TabIndex = 34;
            comboBox_SpineVersion.SelectedValueChanged += comboBox_SpineVersion_SelectedValueChanged;
            // 
            // button_SelectSkels
            // 
            button_SelectSkels.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button_SelectSkels.AutoSize = true;
            button_SelectSkels.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel_ExporterOptions.SetColumnSpan(button_SelectSkels, 2);
            button_SelectSkels.Cursor = Cursors.Hand;
            button_SelectSkels.Location = new Point(32, 15);
            button_SelectSkels.Margin = new Padding(32, 3, 3, 3);
            button_SelectSkels.Name = "button_SelectSkels";
            button_SelectSkels.Size = new Size(485, 37);
            button_SelectSkels.TabIndex = 37;
            button_SelectSkels.Text = "选择骨骼文件与动画...";
            button_SelectSkels.UseVisualStyleBackColor = true;
            button_SelectSkels.Click += button_SelectSkels_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(32, 156);
            label6.Margin = new Padding(32, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(118, 27);
            label6.TabIndex = 40;
            label6.Text = "使用PMA：";
            // 
            // checkBox_UsePMA
            // 
            checkBox_UsePMA.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            checkBox_UsePMA.AutoSize = true;
            checkBox_UsePMA.Location = new Point(238, 159);
            checkBox_UsePMA.Name = "checkBox_UsePMA";
            checkBox_UsePMA.Size = new Size(279, 21);
            checkBox_UsePMA.TabIndex = 41;
            checkBox_UsePMA.UseVisualStyleBackColor = true;
            checkBox_UsePMA.CheckedChanged += checkBox_UsePMA_CheckedChanged;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(32, 224);
            label4.Margin = new Padding(32, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(132, 27);
            label4.TabIndex = 42;
            label4.Text = "宽（像素）：";
            // 
            // numericUpDown_SizeX
            // 
            numericUpDown_SizeX.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_SizeX.Location = new Point(238, 221);
            numericUpDown_SizeX.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            numericUpDown_SizeX.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown_SizeX.Name = "numericUpDown_SizeX";
            numericUpDown_SizeX.Size = new Size(279, 33);
            numericUpDown_SizeX.TabIndex = 44;
            numericUpDown_SizeX.TextAlign = HorizontalAlignment.Right;
            numericUpDown_SizeX.Value = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDown_SizeX.ValueChanged += numericUpDown_SizeX_ValueChanged;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(32, 292);
            label5.Margin = new Padding(32, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(132, 27);
            label5.TabIndex = 43;
            label5.Text = "高（像素）：";
            // 
            // numericUpDown_SizeY
            // 
            numericUpDown_SizeY.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_SizeY.Location = new Point(238, 289);
            numericUpDown_SizeY.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
            numericUpDown_SizeY.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown_SizeY.Name = "numericUpDown_SizeY";
            numericUpDown_SizeY.Size = new Size(279, 33);
            numericUpDown_SizeY.TabIndex = 45;
            numericUpDown_SizeY.TextAlign = HorizontalAlignment.Right;
            numericUpDown_SizeY.Value = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDown_SizeY.ValueChanged += numericUpDown_SizeY_ValueChanged;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(32, 360);
            label7.Margin = new Padding(32, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(112, 27);
            label7.TabIndex = 38;
            label7.Text = "导出帧率：";
            // 
            // numericUpDown_Fps
            // 
            numericUpDown_Fps.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_Fps.Location = new Point(238, 357);
            numericUpDown_Fps.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            numericUpDown_Fps.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_Fps.Name = "numericUpDown_Fps";
            numericUpDown_Fps.Size = new Size(279, 33);
            numericUpDown_Fps.TabIndex = 39;
            numericUpDown_Fps.TextAlign = HorizontalAlignment.Right;
            numericUpDown_Fps.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // tableLayoutPanel_View
            // 
            tableLayoutPanel_View.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
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
            tableLayoutPanel_View.Size = new Size(1010, 838);
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
            tableLayoutPanel_ViewSet.Controls.Add(label_PreviewSize, 0, 0);
            tableLayoutPanel_ViewSet.Controls.Add(button_ResetTimeline, 3, 0);
            tableLayoutPanel_ViewSet.Controls.Add(label3, 1, 0);
            tableLayoutPanel_ViewSet.Controls.Add(numericUpDown_PreviewScale, 2, 0);
            tableLayoutPanel_ViewSet.Location = new Point(1, 794);
            tableLayoutPanel_ViewSet.Margin = new Padding(0);
            tableLayoutPanel_ViewSet.Name = "tableLayoutPanel_ViewSet";
            tableLayoutPanel_ViewSet.RowCount = 1;
            tableLayoutPanel_ViewSet.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel_ViewSet.Size = new Size(1008, 43);
            tableLayoutPanel_ViewSet.TabIndex = 0;
            // 
            // label_PreviewSize
            // 
            label_PreviewSize.Anchor = AnchorStyles.None;
            label_PreviewSize.AutoSize = true;
            label_PreviewSize.Location = new Point(3, 8);
            label_PreviewSize.Name = "label_PreviewSize";
            label_PreviewSize.Size = new Size(233, 27);
            label_PreviewSize.TabIndex = 6;
            label_PreviewSize.Text = "视窗大小：[1920, 1080]";
            // 
            // button_ResetTimeline
            // 
            button_ResetTimeline.Anchor = AnchorStyles.None;
            button_ResetTimeline.AutoSize = true;
            button_ResetTimeline.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_ResetTimeline.Cursor = Cursors.Hand;
            button_ResetTimeline.Location = new Point(659, 3);
            button_ResetTimeline.Name = "button_ResetTimeline";
            button_ResetTimeline.Size = new Size(182, 37);
            button_ResetTimeline.TabIndex = 5;
            button_ResetTimeline.Text = "重置预览画面动画";
            button_ResetTimeline.UseVisualStyleBackColor = true;
            button_ResetTimeline.Click += button_ResetTimeline_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Location = new Point(242, 8);
            label3.Name = "label3";
            label3.Size = new Size(132, 27);
            label3.TabIndex = 2;
            label3.Text = "缩放百分比：";
            // 
            // numericUpDown_PreviewScale
            // 
            numericUpDown_PreviewScale.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_PreviewScale.DecimalPlaces = 2;
            numericUpDown_PreviewScale.Location = new Point(380, 5);
            numericUpDown_PreviewScale.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericUpDown_PreviewScale.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_PreviewScale.Name = "numericUpDown_PreviewScale";
            numericUpDown_PreviewScale.Size = new Size(109, 33);
            numericUpDown_PreviewScale.TabIndex = 3;
            numericUpDown_PreviewScale.TextAlign = HorizontalAlignment.Right;
            numericUpDown_PreviewScale.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericUpDown_PreviewScale.ValueChanged += numericUpDown_PreviewScale_ValueChanged;
            // 
            // panel_PreviewContainer
            // 
            panel_PreviewContainer.Controls.Add(panel_Preview);
            panel_PreviewContainer.Dock = DockStyle.Fill;
            panel_PreviewContainer.Location = new Point(4, 4);
            panel_PreviewContainer.Name = "panel_PreviewContainer";
            panel_PreviewContainer.Size = new Size(1002, 786);
            panel_PreviewContainer.TabIndex = 1;
            panel_PreviewContainer.SizeChanged += panel_PreviewContainer_SizeChanged;
            // 
            // panel_Preview
            // 
            panel_Preview.Anchor = AnchorStyles.None;
            panel_Preview.Location = new Point(253, 152);
            panel_Preview.Name = "panel_Preview";
            panel_Preview.Size = new Size(512, 512);
            panel_Preview.TabIndex = 0;
            panel_Preview.MouseDown += panel_Preview_MouseDown;
            panel_Preview.MouseMove += panel_Preview_MouseMove;
            panel_Preview.MouseUp += panel_Preview_MouseUp;
            panel_Preview.MouseWheel += Panel_Preview_MouseWheel;
            // 
            // tabPage_FixEdge
            // 
            tabPage_FixEdge.BackColor = SystemColors.Control;
            tabPage_FixEdge.Location = new Point(4, 33);
            tabPage_FixEdge.Margin = new Padding(0);
            tabPage_FixEdge.Name = "tabPage_FixEdge";
            tabPage_FixEdge.Size = new Size(1538, 843);
            tabPage_FixEdge.TabIndex = 1;
            tabPage_FixEdge.Text = "边缘修复工具";
            // 
            // tabPage_FixPMA
            // 
            tabPage_FixPMA.BackColor = SystemColors.Control;
            tabPage_FixPMA.Location = new Point(4, 33);
            tabPage_FixPMA.Margin = new Padding(0);
            tabPage_FixPMA.Name = "tabPage_FixPMA";
            tabPage_FixPMA.Size = new Size(1538, 843);
            tabPage_FixPMA.TabIndex = 2;
            tabPage_FixPMA.Text = "PMA修复工具";
            // 
            // tableLayoutPanel_SpineTool
            // 
            tableLayoutPanel_SpineTool.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
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
            tableLayoutPanel_SpineTool.Size = new Size(1548, 923);
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
            tableLayoutPanel_Progress.Controls.Add(progressBar_SpineTool, 1, 0);
            tableLayoutPanel_Progress.Controls.Add(label1, 0, 0);
            tableLayoutPanel_Progress.Location = new Point(1, 882);
            tableLayoutPanel_Progress.Margin = new Padding(0);
            tableLayoutPanel_Progress.Name = "tableLayoutPanel_Progress";
            tableLayoutPanel_Progress.RowCount = 1;
            tableLayoutPanel_Progress.RowStyles.Add(new RowStyle());
            tableLayoutPanel_Progress.Size = new Size(1546, 40);
            tableLayoutPanel_Progress.TabIndex = 1;
            // 
            // progressBar_SpineTool
            // 
            progressBar_SpineTool.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar_SpineTool.Location = new Point(121, 3);
            progressBar_SpineTool.Name = "progressBar_SpineTool";
            progressBar_SpineTool.Size = new Size(1422, 34);
            progressBar_SpineTool.Style = ProgressBarStyle.Continuous;
            progressBar_SpineTool.TabIndex = 0;
            progressBar_SpineTool.Value = 50;
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 27F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1548, 923);
            Controls.Add(tableLayoutPanel_SpineTool);
            Font = new Font("Microsoft YaHei UI", 10F);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SpineTool";
            Load += MainForm_Load;
            tabControl_Tools.ResumeLayout(false);
            tabPage_Exporter.ResumeLayout(false);
            splitContainer_Exporter.Panel1.ResumeLayout(false);
            splitContainer_Exporter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer_Exporter).EndInit();
            splitContainer_Exporter.ResumeLayout(false);
            tableLayoutPanel_ExporterOptions.ResumeLayout(false);
            tableLayoutPanel_ExporterOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_SizeX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_SizeY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_Fps).EndInit();
            tableLayoutPanel_View.ResumeLayout(false);
            tableLayoutPanel_View.PerformLayout();
            tableLayoutPanel_ViewSet.ResumeLayout(false);
            tableLayoutPanel_ViewSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_PreviewScale).EndInit();
            panel_PreviewContainer.ResumeLayout(false);
            tableLayoutPanel_SpineTool.ResumeLayout(false);
            tableLayoutPanel_SpineTool.PerformLayout();
            tableLayoutPanel_Progress.ResumeLayout(false);
            tableLayoutPanel_Progress.PerformLayout();
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
        private ProgressBar progressBar_SpineTool;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel_View;
        private TableLayoutPanel tableLayoutPanel_ViewSet;
        private Label label3;
        private NumericUpDown numericUpDown_PreviewScale;
        private Panel panel_PreviewContainer;
        private TableLayoutPanel tableLayoutPanel_ExporterOptions;
        private Label label2;
        private ComboBox comboBox_SpineVersion;
        private Button button_SelectSkels;
        private Label label7;
        private Button button_Export;
        private NumericUpDown numericUpDown_Fps;
        private Button button_ResetTimeline;
        private Label label6;
        private CheckBox checkBox_UsePMA;
        private NumericUpDown numericUpDown_SizeY;
        private NumericUpDown numericUpDown_SizeX;
        private Label label4;
        private Label label5;
        private Label label_PreviewSize;
        private Panel panel_Preview;
    }
}
