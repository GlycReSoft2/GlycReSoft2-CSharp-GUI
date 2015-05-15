namespace GlycReSoft
{
    partial class MS1ResultsViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.ResultsGridView = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button17 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.richTextBox7 = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button15 = new System.Windows.Forms.Button();
            this.GlycanCompositionFileDisplay = new System.Windows.Forms.RichTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.ROCCurveChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TotalVolumeHistogramChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.VolumeHistogramFileSelector = new System.Windows.Forms.ComboBox();
            this.fBDGResults1 = new System.Windows.Forms.FolderBrowserDialog();
            this.oFDResults = new System.Windows.Forms.OpenFileDialog();
            this.oFDcposTest = new System.Windows.Forms.OpenFileDialog();
            this.oFDPPMSD = new System.Windows.Forms.OpenFileDialog();
            this.sFDCR = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGridView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ROCCurveChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalVolumeHistogramChart)).BeginInit();
            this.SuspendLayout();
            // 
            // ResultsGridView
            // 
            this.ResultsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultsGridView.Location = new System.Drawing.Point(8, 59);
            this.ResultsGridView.Name = "ResultsGridView";
            this.ResultsGridView.Size = new System.Drawing.Size(1200, 445);
            this.ResultsGridView.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(342, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 43);
            this.button1.TabIndex = 1;
            this.button1.Text = "Save All to File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.SaveAllResultsToFileButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1235, 558);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.ResultsGridView);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1227, 532);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "View Results";
            // 
            // comboBox1
            // 
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(24, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(297, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "File list";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage2.Controls.Add(this.button17);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.richTextBox7);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.checkBox2);
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.button15);
            this.tabPage2.Controls.Add(this.GlycanCompositionFileDisplay);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.radioButton2);
            this.tabPage2.Controls.Add(this.radioButton1);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.ROCCurveChart);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Controls.Add(this.comboBox2);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1227, 532);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Evaluate Results";
            // 
            // button17
            // 
            this.button17.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button17.Enabled = false;
            this.button17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button17.Location = new System.Drawing.Point(82, 282);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(215, 23);
            this.button17.TabIndex = 148;
            this.button17.Text = "Load Protein Prospector MS-digest file";
            this.button17.UseVisualStyleBackColor = false;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(3, 211);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(283, 13);
            this.label11.TabIndex = 147;
            this.label11.Text = "(For glycopeptide data ONLY) Load Protein Prospector file.";
            // 
            // richTextBox7
            // 
            this.richTextBox7.Location = new System.Drawing.Point(8, 227);
            this.richTextBox7.Name = "richTextBox7";
            this.richTextBox7.Size = new System.Drawing.Size(289, 49);
            this.richTextBox7.TabIndex = 146;
            this.richTextBox7.Text = "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(8, 323);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 144;
            this.label9.Text = "Additional Curves:";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.Color.Transparent;
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox2.Location = new System.Drawing.Point(8, 389);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(170, 17);
            this.checkBox2.TabIndex = 143;
            this.checkBox2.Text = "Include Unsupervised Learning";
            this.checkBox2.UseVisualStyleBackColor = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox1.Location = new System.Drawing.Point(8, 355);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(139, 17);
            this.checkBox1.TabIndex = 142;
            this.checkBox1.Text = "Include Default Features";
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button15.Enabled = false;
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.Location = new System.Drawing.Point(136, 184);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(161, 23);
            this.button15.TabIndex = 141;
            this.button15.Text = "Add GlycanCompositions File";
            this.button15.UseVisualStyleBackColor = false;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // GlycanCompositionFileDisplay
            // 
            this.GlycanCompositionFileDisplay.Location = new System.Drawing.Point(11, 129);
            this.GlycanCompositionFileDisplay.Name = "GlycanCompositionFileDisplay";
            this.GlycanCompositionFileDisplay.Size = new System.Drawing.Size(286, 49);
            this.GlycanCompositionFileDisplay.TabIndex = 140;
            this.GlycanCompositionFileDisplay.Text = "";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(8, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 13);
            this.label10.TabIndex = 139;
            this.label10.Text = "Generate curve based on:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label8.Location = new System.Drawing.Point(8, 91);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(292, 26);
            this.label8.TabIndex = 138;
            this.label8.Text = "Load GlycanCompositions File for False Data Set generation \r\n(.cpos file)";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.BackColor = System.Drawing.Color.Transparent;
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.Location = new System.Drawing.Point(11, 61);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(195, 17);
            this.radioButton2.TabIndex = 137;
            this.radioButton2.Text = "Randomly generated False Data Set";
            this.radioButton2.UseVisualStyleBackColor = false;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.Checked = true;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.Location = new System.Drawing.Point(11, 38);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(57, 17);
            this.radioButton1.TabIndex = 136;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Scores";
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Location = new System.Drawing.Point(341, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(175, 13);
            this.label7.TabIndex = 135;
            this.label7.Text = "Choose a feature to look at its data:";
            // 
            // ROCCurveChart
            // 
            chartArea3.AxisX.Interval = 0.2D;
            chartArea3.AxisX.IsMarginVisible = false;
            chartArea3.AxisX.Maximum = 1D;
            chartArea3.AxisX.Minimum = 0D;
            chartArea3.AxisX.Title = "False Positive Rate";
            chartArea3.AxisX2.Interval = 0.2D;
            chartArea3.AxisX2.IsMarginVisible = false;
            chartArea3.AxisX2.Maximum = 1D;
            chartArea3.AxisX2.Minimum = 0D;
            chartArea3.AxisX2.Title = "False Positive Rate";
            chartArea3.AxisY.Interval = 0.2D;
            chartArea3.AxisY.IsMarginVisible = false;
            chartArea3.AxisY.Maximum = 1D;
            chartArea3.AxisY.Minimum = 0D;
            chartArea3.AxisY.Title = "True Positive Rate";
            chartArea3.AxisY2.Interval = 0.2D;
            chartArea3.AxisY2.IsMarginVisible = false;
            chartArea3.AxisY2.Maximum = 1D;
            chartArea3.AxisY2.Minimum = 0D;
            chartArea3.AxisY2.Title = "True Positive Rate";
            chartArea3.Name = "ROC Curve";
            this.ROCCurveChart.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.ROCCurveChart.Legends.Add(legend3);
            this.ROCCurveChart.Location = new System.Drawing.Point(644, 32);
            this.ROCCurveChart.Name = "ROCCurveChart";
            this.ROCCurveChart.Size = new System.Drawing.Size(528, 466);
            this.ROCCurveChart.TabIndex = 134;
            this.ROCCurveChart.Text = "chart1";
            title2.Name = "Title1";
            title2.Text = "ROC Curve";
            this.ROCCurveChart.Titles.Add(title2);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(344, 59);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(255, 439);
            this.dataGridView2.TabIndex = 133;
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(344, 32);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(255, 21);
            this.comboBox2.TabIndex = 132;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(157, 434);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 42);
            this.button2.TabIndex = 121;
            this.button2.Text = "Evaluate Results";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button9);
            this.tabPage3.Controls.Add(this.button8);
            this.tabPage3.Controls.Add(this.button4);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Controls.Add(this.richTextBox2);
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1227, 532);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Combine Results";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button9.Enabled = false;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Location = new System.Drawing.Point(8, 332);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(102, 36);
            this.button9.TabIndex = 21;
            this.button9.Text = "Combine Results";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(182, 287);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(102, 23);
            this.button8.TabIndex = 20;
            this.button8.Text = "Remove all files";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(8, 374);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(102, 36);
            this.button4.TabIndex = 19;
            this.button4.Text = "Save to File";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(8, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "File List of Glycan Results";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(8, 287);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(110, 23);
            this.button3.TabIndex = 17;
            this.button3.Text = "Add Result Files";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(8, 23);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(276, 258);
            this.richTextBox2.TabIndex = 16;
            this.richTextBox2.Text = "";
            // 
            // dataGridView3
            // 
            this.dataGridView3.AllowUserToDeleteRows = false;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Location = new System.Drawing.Point(290, 23);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.Size = new System.Drawing.Size(917, 453);
            this.dataGridView3.TabIndex = 15;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.numericUpDown1);
            this.tabPage4.Controls.Add(this.checkBox4);
            this.tabPage4.Controls.Add(this.checkBox3);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.TotalVolumeHistogramChart);
            this.tabPage4.Controls.Add(this.VolumeHistogramFileSelector);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1227, 532);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Percent TotalVolume Histogram";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(966, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "compounds.";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(906, 11);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(54, 20);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.BackColor = System.Drawing.Color.Transparent;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Location = new System.Drawing.Point(1049, 15);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(151, 17);
            this.checkBox4.TabIndex = 5;
            this.checkBox4.Text = "Matched Compounds Only";
            this.checkBox4.UseVisualStyleBackColor = false;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.Color.Transparent;
            this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox3.Location = new System.Drawing.Point(688, 12);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(222, 17);
            this.checkBox3.TabIndex = 4;
            this.checkBox3.Text = "Limit Number of data to the highest scored";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Location = new System.Drawing.Point(16, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Choose a File";
            // 
            // TotalVolumeHistogramChart
            // 
            this.TotalVolumeHistogramChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            chartArea4.AxisX.Interval = 1D;
            chartArea4.AxisX.IsStartedFromZero = false;
            chartArea4.AxisX.LabelAutoFitMinFontSize = 5;
            chartArea4.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)(((((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep90) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea4.AxisX.Title = "GlycanCompositions";
            chartArea4.AxisY.Title = "Percent of Total Volume";
            chartArea4.Name = "ChartArea1";
            this.TotalVolumeHistogramChart.ChartAreas.Add(chartArea4);
            legend4.AutoFitMinFontSize = 5;
            legend4.Enabled = false;
            legend4.IsEquallySpacedItems = true;
            legend4.Name = "Legend1";
            legend4.TextWrapThreshold = 1000000;
            this.TotalVolumeHistogramChart.Legends.Add(legend4);
            this.TotalVolumeHistogramChart.Location = new System.Drawing.Point(8, 38);
            this.TotalVolumeHistogramChart.Name = "TotalVolumeHistogramChart";
            series2.ChartArea = "ChartArea1";
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.IsValueShownAsLabel = true;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            this.TotalVolumeHistogramChart.Series.Add(series2);
            this.TotalVolumeHistogramChart.Size = new System.Drawing.Size(1254, 488);
            this.TotalVolumeHistogramChart.TabIndex = 2;
            this.TotalVolumeHistogramChart.Text = "chart2";
            // 
            // VolumeHistogramFileSelector
            // 
            this.VolumeHistogramFileSelector.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VolumeHistogramFileSelector.FormattingEnabled = true;
            this.VolumeHistogramFileSelector.Location = new System.Drawing.Point(93, 11);
            this.VolumeHistogramFileSelector.Name = "VolumeHistogramFileSelector";
            this.VolumeHistogramFileSelector.Size = new System.Drawing.Size(560, 21);
            this.VolumeHistogramFileSelector.TabIndex = 0;
            this.VolumeHistogramFileSelector.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // fBDGResults1
            // 
            this.fBDGResults1.RootFolder = global::GlycReSoft.Properties.Settings.Default.Desk;
            this.fBDGResults1.SelectedPath = global::GlycReSoft.Properties.Settings.Default.Desk1;
            // 
            // oFDResults
            // 
            this.oFDResults.FileOk += new System.ComponentModel.CancelEventHandler(this.oFDResults_FileOk);
            // 
            // oFDcposTest
            // 
            this.oFDcposTest.FileOk += new System.ComponentModel.CancelEventHandler(this.oFDcposTest_FileOk);
            // 
            // oFDPPMSD
            // 
            this.oFDPPMSD.FileOk += new System.ComponentModel.CancelEventHandler(this.oFDPPMSD_FileOk);
            // 
            // sFDCR
            // 
            this.sFDCR.FileOk += new System.ComponentModel.CancelEventHandler(this.sFDCR_FileOk);
            // 
            // MS1ResultsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 560);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MS1ResultsViewer";
            this.Text = "Results";
            ((System.ComponentModel.ISupportInitialize)(this.ResultsGridView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ROCCurveChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TotalVolumeHistogramChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView ResultsGridView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.FolderBrowserDialog fBDGResults1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataVisualization.Charting.Chart ROCCurveChart;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.OpenFileDialog oFDResults;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.RichTextBox GlycanCompositionFileDisplay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.OpenFileDialog oFDcposTest;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RichTextBox richTextBox7;
        private System.Windows.Forms.OpenFileDialog oFDPPMSD;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.SaveFileDialog sFDCR;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataVisualization.Charting.Chart TotalVolumeHistogramChart;
        private System.Windows.Forms.ComboBox VolumeHistogramFileSelector;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}