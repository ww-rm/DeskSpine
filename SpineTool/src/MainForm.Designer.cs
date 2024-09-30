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
            components = new System.ComponentModel.Container();
            tabControl_Tools = new TabControl();
            tabPage_Exporter = new TabPage();
            splitContainer_Exporter = new SplitContainer();
            tableLayoutPanel_ExporterOptions = new TableLayoutPanel();
            button_Export = new Button();
            label2 = new Label();
            comboBox_SpineVersion = new ComboBox();
            label6 = new Label();
            checkBox_UsePMA = new CheckBox();
            label4 = new Label();
            numericUpDown_SizeX = new NumericUpDown();
            label5 = new Label();
            numericUpDown_SizeY = new NumericUpDown();
            label7 = new Label();
            numericUpDown_Fps = new NumericUpDown();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            comboBox_SelectAnime9 = new ComboBox();
            button_SelectSkel9 = new Button();
            textBox_SkelPath9 = new TextBox();
            label17 = new Label();
            comboBox_SelectAnime8 = new ComboBox();
            button_SelectSkel8 = new Button();
            textBox_SkelPath8 = new TextBox();
            label16 = new Label();
            comboBox_SelectAnime7 = new ComboBox();
            button_SelectSkel7 = new Button();
            textBox_SkelPath7 = new TextBox();
            label15 = new Label();
            comboBox_SelectAnime6 = new ComboBox();
            button_SelectSkel6 = new Button();
            textBox_SkelPath6 = new TextBox();
            label14 = new Label();
            comboBox_SelectAnime5 = new ComboBox();
            button_SelectSkel5 = new Button();
            textBox_SkelPath5 = new TextBox();
            label13 = new Label();
            comboBox_SelectAnime4 = new ComboBox();
            button_SelectSkel4 = new Button();
            textBox_SkelPath4 = new TextBox();
            label12 = new Label();
            comboBox_SelectAnime3 = new ComboBox();
            button_SelectSkel3 = new Button();
            textBox_SkelPath3 = new TextBox();
            label11 = new Label();
            label10 = new Label();
            comboBox_SelectAnime2 = new ComboBox();
            button_SelectSkel2 = new Button();
            textBox_SkelPath2 = new TextBox();
            label9 = new Label();
            comboBox_SelectAnime1 = new ComboBox();
            button_SelectSkel1 = new Button();
            textBox_SkelPath1 = new TextBox();
            label8 = new Label();
            comboBox_SelectAnime0 = new ComboBox();
            button_SelectSkel0 = new Button();
            textBox_SkelPath0 = new TextBox();
            label28 = new Label();
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
            button_CancelTask = new Button();
            progressBar_SpineTool = new ProgressBar();
            label_ProgressBar = new Label();
            openFileDialog_SelectSkel = new OpenFileDialog();
            folderBrowserDialog_Export = new FolderBrowserDialog();
            toolTip1 = new ToolTip(components);
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
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
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
            tabControl_Tools.Size = new Size(1546, 877);
            tabControl_Tools.TabIndex = 0;
            // 
            // tabPage_Exporter
            // 
            tabPage_Exporter.BackColor = SystemColors.Control;
            tabPage_Exporter.Controls.Add(splitContainer_Exporter);
            tabPage_Exporter.Location = new Point(4, 36);
            tabPage_Exporter.Margin = new Padding(0);
            tabPage_Exporter.Name = "tabPage_Exporter";
            tabPage_Exporter.Size = new Size(1538, 837);
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
            splitContainer_Exporter.Size = new Size(1538, 837);
            splitContainer_Exporter.SplitterDistance = 522;
            splitContainer_Exporter.TabIndex = 0;
            // 
            // tableLayoutPanel_ExporterOptions
            // 
            tableLayoutPanel_ExporterOptions.ColumnCount = 2;
            tableLayoutPanel_ExporterOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.2290077F));
            tableLayoutPanel_ExporterOptions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54.7709923F));
            tableLayoutPanel_ExporterOptions.Controls.Add(button_Export, 0, 8);
            tableLayoutPanel_ExporterOptions.Controls.Add(label2, 0, 1);
            tableLayoutPanel_ExporterOptions.Controls.Add(comboBox_SpineVersion, 1, 1);
            tableLayoutPanel_ExporterOptions.Controls.Add(label6, 0, 2);
            tableLayoutPanel_ExporterOptions.Controls.Add(checkBox_UsePMA, 1, 2);
            tableLayoutPanel_ExporterOptions.Controls.Add(label4, 0, 3);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_SizeX, 1, 3);
            tableLayoutPanel_ExporterOptions.Controls.Add(label5, 0, 4);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_SizeY, 1, 4);
            tableLayoutPanel_ExporterOptions.Controls.Add(label7, 0, 5);
            tableLayoutPanel_ExporterOptions.Controls.Add(numericUpDown_Fps, 1, 5);
            tableLayoutPanel_ExporterOptions.Controls.Add(panel1, 0, 0);
            tableLayoutPanel_ExporterOptions.Dock = DockStyle.Fill;
            tableLayoutPanel_ExporterOptions.Location = new Point(0, 0);
            tableLayoutPanel_ExporterOptions.Name = "tableLayoutPanel_ExporterOptions";
            tableLayoutPanel_ExporterOptions.RowCount = 9;
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 33.109684F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.360316F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.364205F));
            tableLayoutPanel_ExporterOptions.RowStyles.Add(new RowStyle(SizeType.Percent, 8.364205F));
            tableLayoutPanel_ExporterOptions.Size = new Size(520, 835);
            tableLayoutPanel_ExporterOptions.TabIndex = 0;
            // 
            // button_Export
            // 
            button_Export.AutoSize = true;
            button_Export.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel_ExporterOptions.SetColumnSpan(button_Export, 2);
            button_Export.Cursor = Cursors.Hand;
            button_Export.Dock = DockStyle.Fill;
            button_Export.Location = new Point(32, 762);
            button_Export.Margin = new Padding(32, 3, 4, 3);
            button_Export.Name = "button_Export";
            button_Export.Size = new Size(484, 70);
            button_Export.TabIndex = 5;
            button_Export.Text = "点击导出...";
            button_Export.UseVisualStyleBackColor = true;
            button_Export.Click += button_Export_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(32, 297);
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
            comboBox_SpineVersion.Location = new Point(238, 294);
            comboBox_SpineVersion.Name = "comboBox_SpineVersion";
            comboBox_SpineVersion.Size = new Size(279, 35);
            comboBox_SpineVersion.TabIndex = 34;
            comboBox_SpineVersion.SelectedValueChanged += comboBox_SpineVersion_SelectedValueChanged;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(32, 366);
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
            checkBox_UsePMA.Location = new Point(238, 369);
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
            label4.Location = new Point(32, 435);
            label4.Margin = new Padding(32, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(132, 27);
            label4.TabIndex = 42;
            label4.Text = "宽（像素）：";
            // 
            // numericUpDown_SizeX
            // 
            numericUpDown_SizeX.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_SizeX.Location = new Point(238, 432);
            numericUpDown_SizeX.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
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
            label5.Location = new Point(32, 504);
            label5.Margin = new Padding(32, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(132, 27);
            label5.TabIndex = 43;
            label5.Text = "高（像素）：";
            // 
            // numericUpDown_SizeY
            // 
            numericUpDown_SizeY.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_SizeY.Location = new Point(238, 501);
            numericUpDown_SizeY.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
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
            label7.Location = new Point(32, 573);
            label7.Margin = new Padding(32, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(112, 27);
            label7.TabIndex = 38;
            label7.Text = "导出帧率：";
            // 
            // numericUpDown_Fps
            // 
            numericUpDown_Fps.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_Fps.Location = new Point(238, 570);
            numericUpDown_Fps.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
            numericUpDown_Fps.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown_Fps.Name = "numericUpDown_Fps";
            numericUpDown_Fps.Size = new Size(279, 33);
            numericUpDown_Fps.TabIndex = 39;
            numericUpDown_Fps.TextAlign = HorizontalAlignment.Right;
            numericUpDown_Fps.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            tableLayoutPanel_ExporterOptions.SetColumnSpan(panel1, 2);
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new Size(520, 276);
            panel1.TabIndex = 46;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75.05981F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.9401913F));
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime9, 3, 10);
            tableLayoutPanel1.Controls.Add(button_SelectSkel9, 2, 10);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath9, 1, 10);
            tableLayoutPanel1.Controls.Add(label17, 0, 10);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime8, 3, 9);
            tableLayoutPanel1.Controls.Add(button_SelectSkel8, 2, 9);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath8, 1, 9);
            tableLayoutPanel1.Controls.Add(label16, 0, 9);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime7, 3, 8);
            tableLayoutPanel1.Controls.Add(button_SelectSkel7, 2, 8);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath7, 1, 8);
            tableLayoutPanel1.Controls.Add(label15, 0, 8);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime6, 3, 7);
            tableLayoutPanel1.Controls.Add(button_SelectSkel6, 2, 7);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath6, 1, 7);
            tableLayoutPanel1.Controls.Add(label14, 0, 7);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime5, 3, 6);
            tableLayoutPanel1.Controls.Add(button_SelectSkel5, 2, 6);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath5, 1, 6);
            tableLayoutPanel1.Controls.Add(label13, 0, 6);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime4, 3, 5);
            tableLayoutPanel1.Controls.Add(button_SelectSkel4, 2, 5);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath4, 1, 5);
            tableLayoutPanel1.Controls.Add(label12, 0, 5);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime3, 3, 4);
            tableLayoutPanel1.Controls.Add(button_SelectSkel3, 2, 4);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath3, 1, 4);
            tableLayoutPanel1.Controls.Add(label11, 0, 4);
            tableLayoutPanel1.Controls.Add(label10, 0, 0);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime2, 3, 3);
            tableLayoutPanel1.Controls.Add(button_SelectSkel2, 2, 3);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath2, 1, 3);
            tableLayoutPanel1.Controls.Add(label9, 0, 3);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime1, 3, 2);
            tableLayoutPanel1.Controls.Add(button_SelectSkel1, 2, 2);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath1, 1, 2);
            tableLayoutPanel1.Controls.Add(label8, 0, 2);
            tableLayoutPanel1.Controls.Add(comboBox_SelectAnime0, 3, 1);
            tableLayoutPanel1.Controls.Add(button_SelectSkel0, 2, 1);
            tableLayoutPanel1.Controls.Add(textBox_SkelPath0, 1, 1);
            tableLayoutPanel1.Controls.Add(label28, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 11;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090908F));
            tableLayoutPanel1.Size = new Size(494, 495);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // comboBox_SelectAnime9
            // 
            comboBox_SelectAnime9.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime9.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime9.Enabled = false;
            comboBox_SelectAnime9.FormattingEnabled = true;
            comboBox_SelectAnime9.Location = new Point(392, 451);
            comboBox_SelectAnime9.Name = "comboBox_SelectAnime9";
            comboBox_SelectAnime9.Size = new Size(99, 35);
            comboBox_SelectAnime9.TabIndex = 72;
            comboBox_SelectAnime9.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel9
            // 
            button_SelectSkel9.Anchor = AnchorStyles.None;
            button_SelectSkel9.AutoSize = true;
            button_SelectSkel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel9.Location = new Point(348, 449);
            button_SelectSkel9.Margin = new Padding(4);
            button_SelectSkel9.Name = "button_SelectSkel9";
            button_SelectSkel9.Size = new Size(37, 37);
            button_SelectSkel9.TabIndex = 71;
            button_SelectSkel9.Text = "...";
            button_SelectSkel9.UseVisualStyleBackColor = true;
            button_SelectSkel9.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath9
            // 
            textBox_SkelPath9.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath9.BackColor = SystemColors.Window;
            textBox_SkelPath9.Location = new Point(36, 451);
            textBox_SkelPath9.Margin = new Padding(4);
            textBox_SkelPath9.Name = "textBox_SkelPath9";
            textBox_SkelPath9.Size = new Size(304, 33);
            textBox_SkelPath9.TabIndex = 70;
            textBox_SkelPath9.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath9.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label17
            // 
            label17.Anchor = AnchorStyles.None;
            label17.AutoSize = true;
            label17.Location = new Point(4, 454);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(24, 27);
            label17.TabIndex = 69;
            label17.Text = "9";
            // 
            // comboBox_SelectAnime8
            // 
            comboBox_SelectAnime8.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime8.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime8.Enabled = false;
            comboBox_SelectAnime8.FormattingEnabled = true;
            comboBox_SelectAnime8.Location = new Point(392, 402);
            comboBox_SelectAnime8.Name = "comboBox_SelectAnime8";
            comboBox_SelectAnime8.Size = new Size(99, 35);
            comboBox_SelectAnime8.TabIndex = 68;
            comboBox_SelectAnime8.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel8
            // 
            button_SelectSkel8.Anchor = AnchorStyles.None;
            button_SelectSkel8.AutoSize = true;
            button_SelectSkel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel8.Location = new Point(348, 400);
            button_SelectSkel8.Margin = new Padding(4);
            button_SelectSkel8.Name = "button_SelectSkel8";
            button_SelectSkel8.Size = new Size(37, 36);
            button_SelectSkel8.TabIndex = 67;
            button_SelectSkel8.Text = "...";
            button_SelectSkel8.UseVisualStyleBackColor = true;
            button_SelectSkel8.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath8
            // 
            textBox_SkelPath8.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath8.BackColor = SystemColors.Window;
            textBox_SkelPath8.Location = new Point(36, 401);
            textBox_SkelPath8.Margin = new Padding(4);
            textBox_SkelPath8.Name = "textBox_SkelPath8";
            textBox_SkelPath8.Size = new Size(304, 33);
            textBox_SkelPath8.TabIndex = 66;
            textBox_SkelPath8.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath8.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.None;
            label16.AutoSize = true;
            label16.Location = new Point(4, 404);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(24, 27);
            label16.TabIndex = 65;
            label16.Text = "8";
            // 
            // comboBox_SelectAnime7
            // 
            comboBox_SelectAnime7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime7.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime7.Enabled = false;
            comboBox_SelectAnime7.FormattingEnabled = true;
            comboBox_SelectAnime7.Location = new Point(392, 358);
            comboBox_SelectAnime7.Name = "comboBox_SelectAnime7";
            comboBox_SelectAnime7.Size = new Size(99, 35);
            comboBox_SelectAnime7.TabIndex = 64;
            comboBox_SelectAnime7.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel7
            // 
            button_SelectSkel7.Anchor = AnchorStyles.None;
            button_SelectSkel7.AutoSize = true;
            button_SelectSkel7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel7.Location = new Point(348, 356);
            button_SelectSkel7.Margin = new Padding(4);
            button_SelectSkel7.Name = "button_SelectSkel7";
            button_SelectSkel7.Size = new Size(37, 36);
            button_SelectSkel7.TabIndex = 63;
            button_SelectSkel7.Text = "...";
            button_SelectSkel7.UseVisualStyleBackColor = true;
            button_SelectSkel7.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath7
            // 
            textBox_SkelPath7.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath7.BackColor = SystemColors.Window;
            textBox_SkelPath7.Location = new Point(36, 357);
            textBox_SkelPath7.Margin = new Padding(4);
            textBox_SkelPath7.Name = "textBox_SkelPath7";
            textBox_SkelPath7.Size = new Size(304, 33);
            textBox_SkelPath7.TabIndex = 62;
            textBox_SkelPath7.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath7.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.None;
            label15.AutoSize = true;
            label15.Location = new Point(4, 360);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(24, 27);
            label15.TabIndex = 61;
            label15.Text = "7";
            // 
            // comboBox_SelectAnime6
            // 
            comboBox_SelectAnime6.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime6.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime6.Enabled = false;
            comboBox_SelectAnime6.FormattingEnabled = true;
            comboBox_SelectAnime6.Location = new Point(392, 314);
            comboBox_SelectAnime6.Name = "comboBox_SelectAnime6";
            comboBox_SelectAnime6.Size = new Size(99, 35);
            comboBox_SelectAnime6.TabIndex = 60;
            comboBox_SelectAnime6.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel6
            // 
            button_SelectSkel6.Anchor = AnchorStyles.None;
            button_SelectSkel6.AutoSize = true;
            button_SelectSkel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel6.Location = new Point(348, 312);
            button_SelectSkel6.Margin = new Padding(4);
            button_SelectSkel6.Name = "button_SelectSkel6";
            button_SelectSkel6.Size = new Size(37, 36);
            button_SelectSkel6.TabIndex = 59;
            button_SelectSkel6.Text = "...";
            button_SelectSkel6.UseVisualStyleBackColor = true;
            button_SelectSkel6.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath6
            // 
            textBox_SkelPath6.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath6.BackColor = SystemColors.Window;
            textBox_SkelPath6.Location = new Point(36, 313);
            textBox_SkelPath6.Margin = new Padding(4);
            textBox_SkelPath6.Name = "textBox_SkelPath6";
            textBox_SkelPath6.Size = new Size(304, 33);
            textBox_SkelPath6.TabIndex = 58;
            textBox_SkelPath6.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath6.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.None;
            label14.AutoSize = true;
            label14.Location = new Point(4, 316);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(24, 27);
            label14.TabIndex = 57;
            label14.Text = "6";
            // 
            // comboBox_SelectAnime5
            // 
            comboBox_SelectAnime5.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime5.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime5.Enabled = false;
            comboBox_SelectAnime5.FormattingEnabled = true;
            comboBox_SelectAnime5.Location = new Point(392, 270);
            comboBox_SelectAnime5.Name = "comboBox_SelectAnime5";
            comboBox_SelectAnime5.Size = new Size(99, 35);
            comboBox_SelectAnime5.TabIndex = 56;
            comboBox_SelectAnime5.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel5
            // 
            button_SelectSkel5.Anchor = AnchorStyles.None;
            button_SelectSkel5.AutoSize = true;
            button_SelectSkel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel5.Location = new Point(348, 268);
            button_SelectSkel5.Margin = new Padding(4);
            button_SelectSkel5.Name = "button_SelectSkel5";
            button_SelectSkel5.Size = new Size(37, 36);
            button_SelectSkel5.TabIndex = 55;
            button_SelectSkel5.Text = "...";
            button_SelectSkel5.UseVisualStyleBackColor = true;
            button_SelectSkel5.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath5
            // 
            textBox_SkelPath5.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath5.BackColor = SystemColors.Window;
            textBox_SkelPath5.Location = new Point(36, 269);
            textBox_SkelPath5.Margin = new Padding(4);
            textBox_SkelPath5.Name = "textBox_SkelPath5";
            textBox_SkelPath5.Size = new Size(304, 33);
            textBox_SkelPath5.TabIndex = 54;
            textBox_SkelPath5.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath5.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.None;
            label13.AutoSize = true;
            label13.Location = new Point(4, 272);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(24, 27);
            label13.TabIndex = 53;
            label13.Text = "5";
            // 
            // comboBox_SelectAnime4
            // 
            comboBox_SelectAnime4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime4.Enabled = false;
            comboBox_SelectAnime4.FormattingEnabled = true;
            comboBox_SelectAnime4.Location = new Point(392, 226);
            comboBox_SelectAnime4.Name = "comboBox_SelectAnime4";
            comboBox_SelectAnime4.Size = new Size(99, 35);
            comboBox_SelectAnime4.TabIndex = 52;
            comboBox_SelectAnime4.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel4
            // 
            button_SelectSkel4.Anchor = AnchorStyles.None;
            button_SelectSkel4.AutoSize = true;
            button_SelectSkel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel4.Location = new Point(348, 224);
            button_SelectSkel4.Margin = new Padding(4);
            button_SelectSkel4.Name = "button_SelectSkel4";
            button_SelectSkel4.Size = new Size(37, 36);
            button_SelectSkel4.TabIndex = 51;
            button_SelectSkel4.Text = "...";
            button_SelectSkel4.UseVisualStyleBackColor = true;
            button_SelectSkel4.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath4
            // 
            textBox_SkelPath4.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath4.BackColor = SystemColors.Window;
            textBox_SkelPath4.Location = new Point(36, 225);
            textBox_SkelPath4.Margin = new Padding(4);
            textBox_SkelPath4.Name = "textBox_SkelPath4";
            textBox_SkelPath4.Size = new Size(304, 33);
            textBox_SkelPath4.TabIndex = 50;
            textBox_SkelPath4.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath4.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.None;
            label12.AutoSize = true;
            label12.Location = new Point(4, 228);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(24, 27);
            label12.TabIndex = 49;
            label12.Text = "4";
            // 
            // comboBox_SelectAnime3
            // 
            comboBox_SelectAnime3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime3.Enabled = false;
            comboBox_SelectAnime3.FormattingEnabled = true;
            comboBox_SelectAnime3.Location = new Point(392, 182);
            comboBox_SelectAnime3.Name = "comboBox_SelectAnime3";
            comboBox_SelectAnime3.Size = new Size(99, 35);
            comboBox_SelectAnime3.TabIndex = 48;
            comboBox_SelectAnime3.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel3
            // 
            button_SelectSkel3.Anchor = AnchorStyles.None;
            button_SelectSkel3.AutoSize = true;
            button_SelectSkel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel3.Location = new Point(348, 180);
            button_SelectSkel3.Margin = new Padding(4);
            button_SelectSkel3.Name = "button_SelectSkel3";
            button_SelectSkel3.Size = new Size(37, 36);
            button_SelectSkel3.TabIndex = 47;
            button_SelectSkel3.Text = "...";
            button_SelectSkel3.UseVisualStyleBackColor = true;
            button_SelectSkel3.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath3
            // 
            textBox_SkelPath3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath3.BackColor = SystemColors.Window;
            textBox_SkelPath3.Location = new Point(36, 181);
            textBox_SkelPath3.Margin = new Padding(4);
            textBox_SkelPath3.Name = "textBox_SkelPath3";
            textBox_SkelPath3.Size = new Size(304, 33);
            textBox_SkelPath3.TabIndex = 46;
            textBox_SkelPath3.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath3.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.None;
            label11.AutoSize = true;
            label11.Location = new Point(4, 184);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(24, 27);
            label11.TabIndex = 45;
            label11.Text = "3";
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label10, 4);
            label10.Location = new Point(3, 8);
            label10.Margin = new Padding(3);
            label10.Name = "label10";
            label10.Size = new Size(488, 27);
            label10.TabIndex = 44;
            label10.Text = "加载骨骼动画";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // comboBox_SelectAnime2
            // 
            comboBox_SelectAnime2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime2.Enabled = false;
            comboBox_SelectAnime2.FormattingEnabled = true;
            comboBox_SelectAnime2.Location = new Point(392, 138);
            comboBox_SelectAnime2.Name = "comboBox_SelectAnime2";
            comboBox_SelectAnime2.Size = new Size(99, 35);
            comboBox_SelectAnime2.TabIndex = 43;
            comboBox_SelectAnime2.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel2
            // 
            button_SelectSkel2.Anchor = AnchorStyles.None;
            button_SelectSkel2.AutoSize = true;
            button_SelectSkel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel2.Location = new Point(348, 136);
            button_SelectSkel2.Margin = new Padding(4);
            button_SelectSkel2.Name = "button_SelectSkel2";
            button_SelectSkel2.Size = new Size(37, 36);
            button_SelectSkel2.TabIndex = 42;
            button_SelectSkel2.Text = "...";
            button_SelectSkel2.UseVisualStyleBackColor = true;
            button_SelectSkel2.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath2
            // 
            textBox_SkelPath2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath2.BackColor = SystemColors.Window;
            textBox_SkelPath2.Location = new Point(36, 137);
            textBox_SkelPath2.Margin = new Padding(4);
            textBox_SkelPath2.Name = "textBox_SkelPath2";
            textBox_SkelPath2.Size = new Size(304, 33);
            textBox_SkelPath2.TabIndex = 41;
            textBox_SkelPath2.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath2.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.None;
            label9.AutoSize = true;
            label9.Location = new Point(4, 140);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(24, 27);
            label9.TabIndex = 40;
            label9.Text = "2";
            // 
            // comboBox_SelectAnime1
            // 
            comboBox_SelectAnime1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime1.Enabled = false;
            comboBox_SelectAnime1.FormattingEnabled = true;
            comboBox_SelectAnime1.Location = new Point(392, 94);
            comboBox_SelectAnime1.Name = "comboBox_SelectAnime1";
            comboBox_SelectAnime1.Size = new Size(99, 35);
            comboBox_SelectAnime1.TabIndex = 39;
            comboBox_SelectAnime1.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel1
            // 
            button_SelectSkel1.Anchor = AnchorStyles.None;
            button_SelectSkel1.AutoSize = true;
            button_SelectSkel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel1.Location = new Point(348, 92);
            button_SelectSkel1.Margin = new Padding(4);
            button_SelectSkel1.Name = "button_SelectSkel1";
            button_SelectSkel1.Size = new Size(37, 36);
            button_SelectSkel1.TabIndex = 38;
            button_SelectSkel1.Text = "...";
            button_SelectSkel1.UseVisualStyleBackColor = true;
            button_SelectSkel1.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath1
            // 
            textBox_SkelPath1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath1.BackColor = SystemColors.Window;
            textBox_SkelPath1.Location = new Point(36, 93);
            textBox_SkelPath1.Margin = new Padding(4);
            textBox_SkelPath1.Name = "textBox_SkelPath1";
            textBox_SkelPath1.Size = new Size(304, 33);
            textBox_SkelPath1.TabIndex = 37;
            textBox_SkelPath1.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath1.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.None;
            label8.AutoSize = true;
            label8.Location = new Point(4, 96);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(24, 27);
            label8.TabIndex = 36;
            label8.Text = "1";
            // 
            // comboBox_SelectAnime0
            // 
            comboBox_SelectAnime0.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox_SelectAnime0.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_SelectAnime0.Enabled = false;
            comboBox_SelectAnime0.FormattingEnabled = true;
            comboBox_SelectAnime0.Location = new Point(392, 50);
            comboBox_SelectAnime0.Name = "comboBox_SelectAnime0";
            comboBox_SelectAnime0.Size = new Size(99, 35);
            comboBox_SelectAnime0.TabIndex = 35;
            comboBox_SelectAnime0.SelectedValueChanged += ComboBox_SelectAnime_SelectedValueChanged;
            // 
            // button_SelectSkel0
            // 
            button_SelectSkel0.Anchor = AnchorStyles.None;
            button_SelectSkel0.AutoSize = true;
            button_SelectSkel0.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_SelectSkel0.Location = new Point(348, 48);
            button_SelectSkel0.Margin = new Padding(4);
            button_SelectSkel0.Name = "button_SelectSkel0";
            button_SelectSkel0.Size = new Size(37, 36);
            button_SelectSkel0.TabIndex = 31;
            button_SelectSkel0.Text = "...";
            button_SelectSkel0.UseVisualStyleBackColor = true;
            button_SelectSkel0.Click += button_SelectSkel_Click;
            // 
            // textBox_SkelPath0
            // 
            textBox_SkelPath0.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox_SkelPath0.BackColor = SystemColors.Window;
            textBox_SkelPath0.Location = new Point(36, 49);
            textBox_SkelPath0.Margin = new Padding(4);
            textBox_SkelPath0.Name = "textBox_SkelPath0";
            textBox_SkelPath0.Size = new Size(304, 33);
            textBox_SkelPath0.TabIndex = 30;
            textBox_SkelPath0.TextChanged += textBox_SkelPath_TextChanged;
            textBox_SkelPath0.MouseHover += textBox_SkelPath_MouseHover;
            // 
            // label28
            // 
            label28.Anchor = AnchorStyles.None;
            label28.AutoSize = true;
            label28.Location = new Point(4, 52);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(24, 27);
            label28.TabIndex = 29;
            label28.Text = "0";
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
            tableLayoutPanel_View.Size = new Size(1010, 835);
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
            tableLayoutPanel_ViewSet.Location = new Point(1, 791);
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
            button_ResetTimeline.Location = new Point(645, 3);
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
            label3.Size = new Size(104, 27);
            label3.TabIndex = 2;
            label3.Text = "缩放(%)：";
            // 
            // numericUpDown_PreviewScale
            // 
            numericUpDown_PreviewScale.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_PreviewScale.DecimalPlaces = 2;
            numericUpDown_PreviewScale.Location = new Point(352, 5);
            numericUpDown_PreviewScale.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
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
            panel_PreviewContainer.Size = new Size(1002, 783);
            panel_PreviewContainer.TabIndex = 1;
            panel_PreviewContainer.SizeChanged += panel_PreviewContainer_SizeChanged;
            // 
            // panel_Preview
            // 
            panel_Preview.Anchor = AnchorStyles.None;
            panel_Preview.BackColor = Color.White;
            panel_Preview.Location = new Point(253, 150);
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
            tableLayoutPanel_Progress.ColumnCount = 3;
            tableLayoutPanel_Progress.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_Progress.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel_Progress.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel_Progress.Controls.Add(button_CancelTask, 2, 0);
            tableLayoutPanel_Progress.Controls.Add(progressBar_SpineTool, 1, 0);
            tableLayoutPanel_Progress.Controls.Add(label_ProgressBar, 0, 0);
            tableLayoutPanel_Progress.Location = new Point(1, 879);
            tableLayoutPanel_Progress.Margin = new Padding(0);
            tableLayoutPanel_Progress.Name = "tableLayoutPanel_Progress";
            tableLayoutPanel_Progress.RowCount = 1;
            tableLayoutPanel_Progress.RowStyles.Add(new RowStyle());
            tableLayoutPanel_Progress.Size = new Size(1546, 43);
            tableLayoutPanel_Progress.TabIndex = 1;
            // 
            // button_CancelTask
            // 
            button_CancelTask.Anchor = AnchorStyles.None;
            button_CancelTask.AutoSize = true;
            button_CancelTask.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button_CancelTask.Cursor = Cursors.Hand;
            button_CancelTask.Enabled = false;
            button_CancelTask.Location = new Point(1441, 3);
            button_CancelTask.Name = "button_CancelTask";
            button_CancelTask.Size = new Size(102, 37);
            button_CancelTask.TabIndex = 6;
            button_CancelTask.Text = "取消任务";
            button_CancelTask.UseVisualStyleBackColor = true;
            // 
            // progressBar_SpineTool
            // 
            progressBar_SpineTool.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar_SpineTool.Location = new Point(121, 4);
            progressBar_SpineTool.Maximum = 10000;
            progressBar_SpineTool.Name = "progressBar_SpineTool";
            progressBar_SpineTool.Size = new Size(1314, 34);
            progressBar_SpineTool.Style = ProgressBarStyle.Continuous;
            progressBar_SpineTool.TabIndex = 0;
            // 
            // label_ProgressBar
            // 
            label_ProgressBar.Anchor = AnchorStyles.None;
            label_ProgressBar.AutoSize = true;
            label_ProgressBar.Location = new Point(3, 8);
            label_ProgressBar.Name = "label_ProgressBar";
            label_ProgressBar.Size = new Size(112, 27);
            label_ProgressBar.TabIndex = 1;
            label_ProgressBar.Text = "处理进度：";
            // 
            // openFileDialog_SelectSkel
            // 
            openFileDialog_SelectSkel.AddExtension = false;
            openFileDialog_SelectSkel.AddToRecent = false;
            openFileDialog_SelectSkel.Filter = "Skel 文件 (*.skel; *.json)|*.skel;*.json";
            openFileDialog_SelectSkel.RestoreDirectory = true;
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
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
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
        private Label label_ProgressBar;
        private TableLayoutPanel tableLayoutPanel_View;
        private TableLayoutPanel tableLayoutPanel_ViewSet;
        private Label label3;
        private NumericUpDown numericUpDown_PreviewScale;
        private Panel panel_PreviewContainer;
        private TableLayoutPanel tableLayoutPanel_ExporterOptions;
        private Label label2;
        private ComboBox comboBox_SpineVersion;
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
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label28;
        private TextBox textBox_SkelPath0;
        private Button button_SelectSkel0;
        private Label label10;
        private ComboBox comboBox_SelectAnime2;
        private Button button_SelectSkel2;
        private TextBox textBox_SkelPath2;
        private Label label9;
        private ComboBox comboBox_SelectAnime1;
        private Button button_SelectSkel1;
        private TextBox textBox_SkelPath1;
        private Label label8;
        private ComboBox comboBox_SelectAnime0;
        private Button button_SelectSkel4;
        private TextBox textBox_SkelPath4;
        private Label label12;
        private ComboBox comboBox_SelectAnime3;
        private Button button_SelectSkel3;
        private TextBox textBox_SkelPath3;
        private Label label11;
        private ComboBox comboBox_SelectAnime4;
        private ComboBox comboBox_SelectAnime9;
        private Button button_SelectSkel9;
        private TextBox textBox_SkelPath9;
        private Label label17;
        private ComboBox comboBox_SelectAnime8;
        private Button button_SelectSkel8;
        private TextBox textBox_SkelPath8;
        private Label label16;
        private ComboBox comboBox_SelectAnime7;
        private Button button_SelectSkel7;
        private TextBox textBox_SkelPath7;
        private Label label15;
        private ComboBox comboBox_SelectAnime6;
        private Button button_SelectSkel6;
        private TextBox textBox_SkelPath6;
        private Label label14;
        private ComboBox comboBox_SelectAnime5;
        private Button button_SelectSkel5;
        private TextBox textBox_SkelPath5;
        private Label label13;
        private OpenFileDialog openFileDialog_SelectSkel;
        private FolderBrowserDialog folderBrowserDialog_Export;
        private ToolTip toolTip1;
        private Button button_CancelTask;
    }
}
