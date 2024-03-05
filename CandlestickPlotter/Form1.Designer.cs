namespace CandleStickPlotter
{
    partial class Form1
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
            loadFileButton = new Button();
            logScaleCheckBox = new CheckBox();
            tabControl1 = new TabControl();
            graphTab = new TabPage();
            plotTableLayoutPanel = new TableLayoutPanel();
            mainPlotPictureBox = new PictureBox();
            priceLevelPanel = new Panel();
            plotUpperControlsPanel = new Panel();
            editStudiesButton = new Button();
            studiesTab = new TabPage();
            dataGridView1 = new DataGridView();
            loadFileButton2 = new Button();
            strategiesTab = new TabPage();
            computeButton = new Button();
            addConditionButton = new Button();
            startOverButton = new Button();
            conditionsListLabel = new Label();
            resultsDataGridView = new DataGridView();
            OpeningIndexCol = new DataGridViewTextBoxColumn();
            ClosingIndexCol = new DataGridViewTextBoxColumn();
            GainLossCol = new DataGridViewTextBoxColumn();
            conditionGeneratorGB = new GroupBox();
            specificStudyComboBox = new ComboBox();
            specificStudyLabel = new Label();
            additionalParametersGB = new GroupBox();
            atrsTextBox = new TextBox();
            atrsLabel = new Label();
            stDevTextBox = new TextBox();
            movAvg2ComboBox = new ComboBox();
            movAvg2Label = new Label();
            movAvg1ComboBox = new ComboBox();
            movAvg1Label = new Label();
            stDevLabel = new Label();
            lengthTextBox = new TextBox();
            marketDatatypeLabel = new Label();
            marketDatatypeComboBox = new ComboBox();
            lengthLabel = new Label();
            addStudyValueButton = new Button();
            groupBox1 = new GroupBox();
            clearButton = new Button();
            backspaceButton = new Button();
            crossesBelowButton = new Button();
            crossesAboveButton = new Button();
            orButton = new Button();
            notEqualButton = new Button();
            andButton = new Button();
            greaterThanButton = new Button();
            greaterEqThanButton = new Button();
            equalButton = new Button();
            lessEqThanButton = new Button();
            lessThanButton = new Button();
            rightParentButton = new Button();
            leftParentButton = new Button();
            multButton = new Button();
            divButton = new Button();
            minusButton = new Button();
            plusButton = new Button();
            gainStopTextBox = new TextBox();
            valueTextBox = new TextBox();
            gainStopCheckBox = new CheckBox();
            currentConditionGB = new GroupBox();
            expressionLabel = new Label();
            displaceNumericUpDown = new NumericUpDown();
            lossStopTextBox = new TextBox();
            label2 = new Label();
            studyComboBox = new ComboBox();
            lossStopCheckBox = new CheckBox();
            valueRadioButton = new RadioButton();
            posEffectComboBox = new ComboBox();
            posEffectLabel = new Label();
            studyRadioButton = new RadioButton();
            tabControl1.SuspendLayout();
            graphTab.SuspendLayout();
            plotTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainPlotPictureBox).BeginInit();
            plotUpperControlsPanel.SuspendLayout();
            studiesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            strategiesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)resultsDataGridView).BeginInit();
            conditionGeneratorGB.SuspendLayout();
            additionalParametersGB.SuspendLayout();
            groupBox1.SuspendLayout();
            currentConditionGB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)displaceNumericUpDown).BeginInit();
            SuspendLayout();
            // 
            // loadFileButton
            // 
            loadFileButton.Dock = DockStyle.Right;
            loadFileButton.Location = new Point(1175, 0);
            loadFileButton.Name = "loadFileButton";
            loadFileButton.Size = new Size(75, 40);
            loadFileButton.TabIndex = 1;
            loadFileButton.Text = "Load File";
            loadFileButton.UseVisualStyleBackColor = true;
            loadFileButton.Click += LoadFileButton_Click;
            // 
            // logScaleCheckBox
            // 
            logScaleCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logScaleCheckBox.AutoSize = true;
            logScaleCheckBox.Checked = true;
            logScaleCheckBox.CheckState = CheckState.Checked;
            logScaleCheckBox.Location = new Point(973, 12);
            logScaleCheckBox.Name = "logScaleCheckBox";
            logScaleCheckBox.Size = new Size(94, 19);
            logScaleCheckBox.TabIndex = 2;
            logScaleCheckBox.Text = "Use log scale";
            logScaleCheckBox.UseVisualStyleBackColor = true;
            logScaleCheckBox.CheckedChanged += LogScaleCheckBox_CheckedChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(graphTab);
            tabControl1.Controls.Add(studiesTab);
            tabControl1.Controls.Add(strategiesTab);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1264, 985);
            tabControl1.TabIndex = 4;
            // 
            // graphTab
            // 
            graphTab.Controls.Add(plotTableLayoutPanel);
            graphTab.Controls.Add(plotUpperControlsPanel);
            graphTab.Location = new Point(4, 24);
            graphTab.Name = "graphTab";
            graphTab.Padding = new Padding(3);
            graphTab.Size = new Size(1256, 957);
            graphTab.TabIndex = 0;
            graphTab.Text = "Plot";
            graphTab.UseVisualStyleBackColor = true;
            // 
            // plotTableLayoutPanel
            // 
            plotTableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            plotTableLayoutPanel.BackColor = Color.FromArgb(21, 26, 31);
            plotTableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            plotTableLayoutPanel.ColumnCount = 2;
            plotTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            plotTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            plotTableLayoutPanel.Controls.Add(mainPlotPictureBox, 0, 0);
            plotTableLayoutPanel.Controls.Add(priceLevelPanel, 1, 0);
            plotTableLayoutPanel.Location = new Point(3, 49);
            plotTableLayoutPanel.Name = "plotTableLayoutPanel";
            plotTableLayoutPanel.RowCount = 1;
            plotTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            plotTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            plotTableLayoutPanel.Size = new Size(1250, 905);
            plotTableLayoutPanel.TabIndex = 5;
            // 
            // mainPlotPictureBox
            // 
            mainPlotPictureBox.Dock = DockStyle.Fill;
            mainPlotPictureBox.Location = new Point(1, 1);
            mainPlotPictureBox.Margin = new Padding(0);
            mainPlotPictureBox.Name = "mainPlotPictureBox";
            mainPlotPictureBox.Size = new Size(1207, 903);
            mainPlotPictureBox.TabIndex = 0;
            mainPlotPictureBox.TabStop = false;
            // 
            // priceLevelPanel
            // 
            priceLevelPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            priceLevelPanel.Location = new Point(1209, 1);
            priceLevelPanel.Margin = new Padding(0);
            priceLevelPanel.Name = "priceLevelPanel";
            priceLevelPanel.Size = new Size(40, 903);
            priceLevelPanel.TabIndex = 1;
            // 
            // plotUpperControlsPanel
            // 
            plotUpperControlsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            plotUpperControlsPanel.Controls.Add(editStudiesButton);
            plotUpperControlsPanel.Controls.Add(logScaleCheckBox);
            plotUpperControlsPanel.Controls.Add(loadFileButton);
            plotUpperControlsPanel.Location = new Point(3, 3);
            plotUpperControlsPanel.Name = "plotUpperControlsPanel";
            plotUpperControlsPanel.Size = new Size(1250, 40);
            plotUpperControlsPanel.TabIndex = 4;
            // 
            // editStudiesButton
            // 
            editStudiesButton.Location = new Point(1073, 0);
            editStudiesButton.Name = "editStudiesButton";
            editStudiesButton.Size = new Size(96, 40);
            editStudiesButton.TabIndex = 3;
            editStudiesButton.Text = "Edit Studies";
            editStudiesButton.UseVisualStyleBackColor = true;
            editStudiesButton.Click += EditStudiesButton_Click;
            // 
            // studiesTab
            // 
            studiesTab.Controls.Add(dataGridView1);
            studiesTab.Controls.Add(loadFileButton2);
            studiesTab.Location = new Point(4, 24);
            studiesTab.Name = "studiesTab";
            studiesTab.Padding = new Padding(3);
            studiesTab.Size = new Size(1256, 957);
            studiesTab.TabIndex = 1;
            studiesTab.Text = "Studies";
            studiesTab.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(27, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(511, 636);
            dataGridView1.TabIndex = 1;
            // 
            // loadFileButton2
            // 
            loadFileButton2.Location = new Point(544, 619);
            loadFileButton2.Name = "loadFileButton2";
            loadFileButton2.Size = new Size(75, 23);
            loadFileButton2.TabIndex = 0;
            loadFileButton2.Text = "Load File";
            loadFileButton2.UseVisualStyleBackColor = true;
            loadFileButton2.Click += LoadFileButton2_Click;
            // 
            // strategiesTab
            // 
            strategiesTab.Controls.Add(computeButton);
            strategiesTab.Controls.Add(addConditionButton);
            strategiesTab.Controls.Add(startOverButton);
            strategiesTab.Controls.Add(conditionsListLabel);
            strategiesTab.Controls.Add(resultsDataGridView);
            strategiesTab.Controls.Add(conditionGeneratorGB);
            strategiesTab.Location = new Point(4, 24);
            strategiesTab.Name = "strategiesTab";
            strategiesTab.Padding = new Padding(3);
            strategiesTab.Size = new Size(1256, 957);
            strategiesTab.TabIndex = 2;
            strategiesTab.Text = "Strategies";
            strategiesTab.UseVisualStyleBackColor = true;
            // 
            // computeButton
            // 
            computeButton.Enabled = false;
            computeButton.Font = new Font("Segoe UI", 9F);
            computeButton.Location = new Point(540, 619);
            computeButton.Name = "computeButton";
            computeButton.Size = new Size(100, 23);
            computeButton.TabIndex = 40;
            computeButton.Text = "Compute";
            computeButton.UseVisualStyleBackColor = true;
            computeButton.Click += ComputeButton_Click;
            // 
            // addConditionButton
            // 
            addConditionButton.Font = new Font("Segoe UI", 9F);
            addConditionButton.Location = new Point(359, 590);
            addConditionButton.Name = "addConditionButton";
            addConditionButton.Size = new Size(175, 23);
            addConditionButton.TabIndex = 27;
            addConditionButton.Text = "Add Opening Condition";
            addConditionButton.UseVisualStyleBackColor = true;
            addConditionButton.Click += AddCondition_Click;
            // 
            // startOverButton
            // 
            startOverButton.Font = new Font("Segoe UI", 9F);
            startOverButton.Location = new Point(540, 590);
            startOverButton.Name = "startOverButton";
            startOverButton.Size = new Size(100, 23);
            startOverButton.TabIndex = 39;
            startOverButton.Text = "Start Over";
            startOverButton.UseVisualStyleBackColor = true;
            startOverButton.Click += StartOverButton_Click;
            // 
            // conditionsListLabel
            // 
            conditionsListLabel.AutoSize = true;
            conditionsListLabel.Location = new Point(646, 6);
            conditionsListLabel.Name = "conditionsListLabel";
            conditionsListLabel.Size = new Size(47, 15);
            conditionsListLabel.TabIndex = 13;
            conditionsListLabel.Text = "Results:";
            // 
            // resultsDataGridView
            // 
            resultsDataGridView.AllowUserToAddRows = false;
            resultsDataGridView.AllowUserToDeleteRows = false;
            resultsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resultsDataGridView.Columns.AddRange(new DataGridViewColumn[] { OpeningIndexCol, ClosingIndexCol, GainLossCol });
            resultsDataGridView.Location = new Point(646, 24);
            resultsDataGridView.Name = "resultsDataGridView";
            resultsDataGridView.ReadOnly = true;
            resultsDataGridView.RowHeadersVisible = false;
            resultsDataGridView.Size = new Size(771, 195);
            resultsDataGridView.TabIndex = 12;
            // 
            // OpeningIndexCol
            // 
            OpeningIndexCol.HeaderText = "Opening Index";
            OpeningIndexCol.Name = "OpeningIndexCol";
            OpeningIndexCol.ReadOnly = true;
            OpeningIndexCol.Width = 502;
            // 
            // ClosingIndexCol
            // 
            ClosingIndexCol.HeaderText = "Closing Index";
            ClosingIndexCol.Name = "ClosingIndexCol";
            ClosingIndexCol.ReadOnly = true;
            ClosingIndexCol.Width = 104;
            // 
            // GainLossCol
            // 
            GainLossCol.HeaderText = "Gain/Loss%";
            GainLossCol.Name = "GainLossCol";
            GainLossCol.ReadOnly = true;
            GainLossCol.Width = 94;
            // 
            // conditionGeneratorGB
            // 
            conditionGeneratorGB.Controls.Add(specificStudyComboBox);
            conditionGeneratorGB.Controls.Add(specificStudyLabel);
            conditionGeneratorGB.Controls.Add(additionalParametersGB);
            conditionGeneratorGB.Controls.Add(addStudyValueButton);
            conditionGeneratorGB.Controls.Add(groupBox1);
            conditionGeneratorGB.Controls.Add(gainStopTextBox);
            conditionGeneratorGB.Controls.Add(valueTextBox);
            conditionGeneratorGB.Controls.Add(gainStopCheckBox);
            conditionGeneratorGB.Controls.Add(currentConditionGB);
            conditionGeneratorGB.Controls.Add(displaceNumericUpDown);
            conditionGeneratorGB.Controls.Add(lossStopTextBox);
            conditionGeneratorGB.Controls.Add(label2);
            conditionGeneratorGB.Controls.Add(studyComboBox);
            conditionGeneratorGB.Controls.Add(lossStopCheckBox);
            conditionGeneratorGB.Controls.Add(valueRadioButton);
            conditionGeneratorGB.Controls.Add(posEffectComboBox);
            conditionGeneratorGB.Controls.Add(posEffectLabel);
            conditionGeneratorGB.Controls.Add(studyRadioButton);
            conditionGeneratorGB.Location = new Point(8, 6);
            conditionGeneratorGB.Name = "conditionGeneratorGB";
            conditionGeneratorGB.Size = new Size(632, 578);
            conditionGeneratorGB.TabIndex = 11;
            conditionGeneratorGB.TabStop = false;
            conditionGeneratorGB.Text = "Condition Generator";
            // 
            // specificStudyComboBox
            // 
            specificStudyComboBox.FormattingEnabled = true;
            specificStudyComboBox.Location = new Point(96, 67);
            specificStudyComboBox.Name = "specificStudyComboBox";
            specificStudyComboBox.Size = new Size(121, 23);
            specificStudyComboBox.TabIndex = 38;
            // 
            // specificStudyLabel
            // 
            specificStudyLabel.AutoSize = true;
            specificStudyLabel.Location = new Point(6, 70);
            specificStudyLabel.Name = "specificStudyLabel";
            specificStudyLabel.Size = new Size(84, 15);
            specificStudyLabel.TabIndex = 37;
            specificStudyLabel.Text = "Specific Study:";
            // 
            // additionalParametersGB
            // 
            additionalParametersGB.Controls.Add(atrsTextBox);
            additionalParametersGB.Controls.Add(atrsLabel);
            additionalParametersGB.Controls.Add(stDevTextBox);
            additionalParametersGB.Controls.Add(movAvg2ComboBox);
            additionalParametersGB.Controls.Add(movAvg2Label);
            additionalParametersGB.Controls.Add(movAvg1ComboBox);
            additionalParametersGB.Controls.Add(movAvg1Label);
            additionalParametersGB.Controls.Add(stDevLabel);
            additionalParametersGB.Controls.Add(lengthTextBox);
            additionalParametersGB.Controls.Add(marketDatatypeLabel);
            additionalParametersGB.Controls.Add(marketDatatypeComboBox);
            additionalParametersGB.Controls.Add(lengthLabel);
            additionalParametersGB.Location = new Point(6, 94);
            additionalParametersGB.Name = "additionalParametersGB";
            additionalParametersGB.Size = new Size(620, 120);
            additionalParametersGB.TabIndex = 36;
            additionalParametersGB.TabStop = false;
            additionalParametersGB.Text = "Additional Parameters";
            // 
            // atrsTextBox
            // 
            atrsTextBox.Location = new Point(454, 45);
            atrsTextBox.Name = "atrsTextBox";
            atrsTextBox.Size = new Size(80, 23);
            atrsTextBox.TabIndex = 44;
            // 
            // atrsLabel
            // 
            atrsLabel.AutoSize = true;
            atrsLabel.Location = new Point(315, 53);
            atrsLabel.Name = "atrsLabel";
            atrsLabel.Size = new Size(35, 15);
            atrsLabel.TabIndex = 43;
            atrsLabel.Text = "ATRs:";
            // 
            // stDevTextBox
            // 
            stDevTextBox.Location = new Point(145, 45);
            stDevTextBox.Name = "stDevTextBox";
            stDevTextBox.Size = new Size(80, 23);
            stDevTextBox.TabIndex = 42;
            // 
            // movAvg2ComboBox
            // 
            movAvg2ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            movAvg2ComboBox.FormattingEnabled = true;
            movAvg2ComboBox.Location = new Point(454, 74);
            movAvg2ComboBox.Name = "movAvg2ComboBox";
            movAvg2ComboBox.Size = new Size(160, 23);
            movAvg2ComboBox.TabIndex = 41;
            // 
            // movAvg2Label
            // 
            movAvg2Label.AutoSize = true;
            movAvg2Label.Location = new Point(315, 82);
            movAvg2Label.Name = "movAvg2Label";
            movAvg2Label.Size = new Size(133, 15);
            movAvg2Label.TabIndex = 40;
            movAvg2Label.Text = "Moving Average Type 2:";
            // 
            // movAvg1ComboBox
            // 
            movAvg1ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            movAvg1ComboBox.FormattingEnabled = true;
            movAvg1ComboBox.Location = new Point(145, 74);
            movAvg1ComboBox.Name = "movAvg1ComboBox";
            movAvg1ComboBox.Size = new Size(160, 23);
            movAvg1ComboBox.TabIndex = 39;
            // 
            // movAvg1Label
            // 
            movAvg1Label.AutoSize = true;
            movAvg1Label.Location = new Point(6, 82);
            movAvg1Label.Name = "movAvg1Label";
            movAvg1Label.Size = new Size(133, 15);
            movAvg1Label.TabIndex = 38;
            movAvg1Label.Text = "Moving Average Type 1:";
            // 
            // stDevLabel
            // 
            stDevLabel.AutoSize = true;
            stDevLabel.Location = new Point(6, 53);
            stDevLabel.Name = "stDevLabel";
            stDevLabel.Size = new Size(115, 15);
            stDevLabel.TabIndex = 36;
            stDevLabel.Text = "Standard Deviations:";
            // 
            // lengthTextBox
            // 
            lengthTextBox.Location = new Point(145, 16);
            lengthTextBox.Name = "lengthTextBox";
            lengthTextBox.Size = new Size(80, 23);
            lengthTextBox.TabIndex = 35;
            // 
            // marketDatatypeLabel
            // 
            marketDatatypeLabel.AutoSize = true;
            marketDatatypeLabel.Location = new Point(315, 24);
            marketDatatypeLabel.Name = "marketDatatypeLabel";
            marketDatatypeLabel.Size = new Size(97, 15);
            marketDatatypeLabel.TabIndex = 34;
            marketDatatypeLabel.Text = "Market Datatype:";
            // 
            // marketDatatypeComboBox
            // 
            marketDatatypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            marketDatatypeComboBox.FormattingEnabled = true;
            marketDatatypeComboBox.Location = new Point(454, 16);
            marketDatatypeComboBox.Name = "marketDatatypeComboBox";
            marketDatatypeComboBox.Size = new Size(80, 23);
            marketDatatypeComboBox.TabIndex = 33;
            // 
            // lengthLabel
            // 
            lengthLabel.AutoSize = true;
            lengthLabel.Location = new Point(6, 24);
            lengthLabel.Name = "lengthLabel";
            lengthLabel.Size = new Size(47, 15);
            lengthLabel.TabIndex = 32;
            lengthLabel.Text = "Length:";
            // 
            // addStudyValueButton
            // 
            addStudyValueButton.Location = new Point(535, 62);
            addStudyValueButton.Name = "addStudyValueButton";
            addStudyValueButton.Size = new Size(85, 23);
            addStudyValueButton.TabIndex = 35;
            addStudyValueButton.Text = "Add";
            addStudyValueButton.UseVisualStyleBackColor = true;
            addStudyValueButton.Click += AddStudyButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(clearButton);
            groupBox1.Controls.Add(backspaceButton);
            groupBox1.Controls.Add(crossesBelowButton);
            groupBox1.Controls.Add(crossesAboveButton);
            groupBox1.Controls.Add(orButton);
            groupBox1.Controls.Add(notEqualButton);
            groupBox1.Controls.Add(andButton);
            groupBox1.Controls.Add(greaterThanButton);
            groupBox1.Controls.Add(greaterEqThanButton);
            groupBox1.Controls.Add(equalButton);
            groupBox1.Controls.Add(lessEqThanButton);
            groupBox1.Controls.Add(lessThanButton);
            groupBox1.Controls.Add(rightParentButton);
            groupBox1.Controls.Add(leftParentButton);
            groupBox1.Controls.Add(multButton);
            groupBox1.Controls.Add(divButton);
            groupBox1.Controls.Add(minusButton);
            groupBox1.Controls.Add(plusButton);
            groupBox1.Location = new Point(6, 220);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(333, 99);
            groupBox1.TabIndex = 14;
            groupBox1.TabStop = false;
            groupBox1.Text = "Condition Operators";
            // 
            // clearButton
            // 
            clearButton.Font = new Font("Segoe UI", 9F);
            clearButton.Location = new Point(294, 58);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(30, 30);
            clearButton.TabIndex = 30;
            clearButton.Text = "C";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearButton_Click;
            // 
            // backspaceButton
            // 
            backspaceButton.Font = new Font("Segoe UI", 9F);
            backspaceButton.Location = new Point(294, 22);
            backspaceButton.Name = "backspaceButton";
            backspaceButton.Size = new Size(30, 30);
            backspaceButton.TabIndex = 21;
            backspaceButton.Text = "←";
            backspaceButton.UseVisualStyleBackColor = true;
            backspaceButton.Click += BackspaceButton_Click;
            // 
            // crossesBelowButton
            // 
            crossesBelowButton.Font = new Font("Segoe UI", 12F);
            crossesBelowButton.Location = new Point(222, 58);
            crossesBelowButton.Name = "crossesBelowButton";
            crossesBelowButton.Size = new Size(30, 30);
            crossesBelowButton.TabIndex = 28;
            crossesBelowButton.Text = "∨";
            crossesBelowButton.UseVisualStyleBackColor = true;
            crossesBelowButton.Click += CrossesBelowButton_Click;
            // 
            // crossesAboveButton
            // 
            crossesAboveButton.Font = new Font("Segoe UI", 12F);
            crossesAboveButton.Location = new Point(222, 22);
            crossesAboveButton.Name = "crossesAboveButton";
            crossesAboveButton.Size = new Size(30, 30);
            crossesAboveButton.TabIndex = 19;
            crossesAboveButton.Text = "∧";
            crossesAboveButton.UseVisualStyleBackColor = true;
            crossesAboveButton.Click += CrossesAboveButton_Click;
            // 
            // orButton
            // 
            orButton.Font = new Font("Segoe UI", 9F);
            orButton.Location = new Point(258, 58);
            orButton.Name = "orButton";
            orButton.Size = new Size(30, 30);
            orButton.TabIndex = 29;
            orButton.Text = "|";
            orButton.UseVisualStyleBackColor = true;
            orButton.Click += OrButton_Click;
            // 
            // notEqualButton
            // 
            notEqualButton.Font = new Font("Segoe UI", 12F);
            notEqualButton.Location = new Point(186, 58);
            notEqualButton.Name = "notEqualButton";
            notEqualButton.Size = new Size(30, 30);
            notEqualButton.TabIndex = 27;
            notEqualButton.Text = "≠";
            notEqualButton.UseVisualStyleBackColor = true;
            notEqualButton.Click += NotEqualButton_Click;
            // 
            // andButton
            // 
            andButton.Font = new Font("Segoe UI", 9F);
            andButton.Location = new Point(258, 22);
            andButton.Name = "andButton";
            andButton.Size = new Size(30, 30);
            andButton.TabIndex = 20;
            andButton.Text = "&&";
            andButton.UseVisualStyleBackColor = true;
            andButton.Click += AndButton_Click;
            // 
            // greaterThanButton
            // 
            greaterThanButton.Font = new Font("Segoe UI", 12F);
            greaterThanButton.Location = new Point(150, 58);
            greaterThanButton.Name = "greaterThanButton";
            greaterThanButton.Size = new Size(30, 30);
            greaterThanButton.TabIndex = 26;
            greaterThanButton.Text = ">";
            greaterThanButton.UseVisualStyleBackColor = true;
            greaterThanButton.Click += GreaterThanButton_Click;
            // 
            // greaterEqThanButton
            // 
            greaterEqThanButton.Font = new Font("Segoe UI", 12F);
            greaterEqThanButton.Location = new Point(114, 58);
            greaterEqThanButton.Name = "greaterEqThanButton";
            greaterEqThanButton.Size = new Size(30, 30);
            greaterEqThanButton.TabIndex = 25;
            greaterEqThanButton.Text = "≥";
            greaterEqThanButton.UseVisualStyleBackColor = true;
            greaterEqThanButton.Click += GreaterEqThanButton_Click;
            // 
            // equalButton
            // 
            equalButton.Font = new Font("Segoe UI", 12F);
            equalButton.Location = new Point(78, 58);
            equalButton.Name = "equalButton";
            equalButton.Size = new Size(30, 30);
            equalButton.TabIndex = 24;
            equalButton.Text = "=";
            equalButton.UseVisualStyleBackColor = true;
            equalButton.Click += EqualButton_Click;
            // 
            // lessEqThanButton
            // 
            lessEqThanButton.Font = new Font("Segoe UI", 12F);
            lessEqThanButton.Location = new Point(42, 58);
            lessEqThanButton.Name = "lessEqThanButton";
            lessEqThanButton.Size = new Size(30, 30);
            lessEqThanButton.TabIndex = 23;
            lessEqThanButton.Text = "≤";
            lessEqThanButton.UseVisualStyleBackColor = true;
            lessEqThanButton.Click += LessEqThanButton_Click;
            // 
            // lessThanButton
            // 
            lessThanButton.Font = new Font("Segoe UI", 12F);
            lessThanButton.Location = new Point(6, 58);
            lessThanButton.Name = "lessThanButton";
            lessThanButton.Size = new Size(30, 30);
            lessThanButton.TabIndex = 22;
            lessThanButton.Text = "<";
            lessThanButton.UseVisualStyleBackColor = true;
            lessThanButton.Click += LessThanButton_Click;
            // 
            // rightParentButton
            // 
            rightParentButton.Font = new Font("Segoe UI", 12F);
            rightParentButton.Location = new Point(186, 22);
            rightParentButton.Name = "rightParentButton";
            rightParentButton.Size = new Size(30, 30);
            rightParentButton.TabIndex = 18;
            rightParentButton.Text = ")";
            rightParentButton.UseVisualStyleBackColor = true;
            rightParentButton.Click += RightParenButton_Click;
            // 
            // leftParentButton
            // 
            leftParentButton.Font = new Font("Segoe UI", 12F);
            leftParentButton.Location = new Point(150, 22);
            leftParentButton.Name = "leftParentButton";
            leftParentButton.Size = new Size(30, 30);
            leftParentButton.TabIndex = 17;
            leftParentButton.Text = "(";
            leftParentButton.UseVisualStyleBackColor = true;
            leftParentButton.Click += LeftParenButton_Click;
            // 
            // multButton
            // 
            multButton.Font = new Font("Segoe UI", 12F);
            multButton.Location = new Point(78, 22);
            multButton.Name = "multButton";
            multButton.Size = new Size(30, 30);
            multButton.TabIndex = 15;
            multButton.Text = "*";
            multButton.UseVisualStyleBackColor = true;
            multButton.Click += MultButton_Click;
            // 
            // divButton
            // 
            divButton.Font = new Font("Segoe UI", 12F);
            divButton.Location = new Point(114, 22);
            divButton.Name = "divButton";
            divButton.Size = new Size(30, 30);
            divButton.TabIndex = 16;
            divButton.Text = "/";
            divButton.UseVisualStyleBackColor = true;
            divButton.Click += DivButton_Click;
            // 
            // minusButton
            // 
            minusButton.Font = new Font("Segoe UI", 12F);
            minusButton.Location = new Point(42, 22);
            minusButton.Name = "minusButton";
            minusButton.Size = new Size(30, 30);
            minusButton.TabIndex = 14;
            minusButton.Text = "-";
            minusButton.UseVisualStyleBackColor = true;
            minusButton.Click += MinusButton_Click;
            // 
            // plusButton
            // 
            plusButton.Font = new Font("Segoe UI", 12F);
            plusButton.Location = new Point(6, 22);
            plusButton.Name = "plusButton";
            plusButton.Size = new Size(30, 30);
            plusButton.TabIndex = 13;
            plusButton.Text = "+";
            plusButton.UseVisualStyleBackColor = true;
            plusButton.Click += PlusButton_Click;
            // 
            // gainStopTextBox
            // 
            gainStopTextBox.Location = new Point(451, 296);
            gainStopTextBox.Name = "gainStopTextBox";
            gainStopTextBox.Size = new Size(58, 23);
            gainStopTextBox.TabIndex = 34;
            // 
            // valueTextBox
            // 
            valueTextBox.Location = new Point(382, 35);
            valueTextBox.Name = "valueTextBox";
            valueTextBox.Size = new Size(128, 23);
            valueTextBox.TabIndex = 12;
            // 
            // gainStopCheckBox
            // 
            gainStopCheckBox.AutoSize = true;
            gainStopCheckBox.Location = new Point(344, 298);
            gainStopCheckBox.Name = "gainStopCheckBox";
            gainStopCheckBox.Size = new Size(101, 19);
            gainStopCheckBox.TabIndex = 33;
            gainStopCheckBox.Text = "Gain Stop (%):";
            gainStopCheckBox.UseVisualStyleBackColor = true;
            gainStopCheckBox.CheckedChanged += StrategyControls_CheckedChanged;
            // 
            // currentConditionGB
            // 
            currentConditionGB.Controls.Add(expressionLabel);
            currentConditionGB.Location = new Point(6, 325);
            currentConditionGB.Name = "currentConditionGB";
            currentConditionGB.Size = new Size(620, 159);
            currentConditionGB.TabIndex = 33;
            currentConditionGB.TabStop = false;
            currentConditionGB.Text = "Current Condition:";
            // 
            // expressionLabel
            // 
            expressionLabel.AutoSize = true;
            expressionLabel.Location = new Point(6, 19);
            expressionLabel.MaximumSize = new Size(492, 0);
            expressionLabel.Name = "expressionLabel";
            expressionLabel.Size = new Size(0, 15);
            expressionLabel.TabIndex = 0;
            // 
            // displaceNumericUpDown
            // 
            displaceNumericUpDown.Location = new Point(415, 64);
            displaceNumericUpDown.Maximum = new decimal(new int[] { 4000, 0, 0, 0 });
            displaceNumericUpDown.Name = "displaceNumericUpDown";
            displaceNumericUpDown.Size = new Size(95, 23);
            displaceNumericUpDown.TabIndex = 6;
            // 
            // lossStopTextBox
            // 
            lossStopTextBox.Location = new Point(451, 267);
            lossStopTextBox.Name = "lossStopTextBox";
            lossStopTextBox.Size = new Size(58, 23);
            lossStopTextBox.TabIndex = 33;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(323, 72);
            label2.Name = "label2";
            label2.Size = new Size(54, 15);
            label2.TabIndex = 7;
            label2.Text = "Displace:";
            // 
            // studyComboBox
            // 
            studyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            studyComboBox.FormattingEnabled = true;
            studyComboBox.Location = new Point(67, 36);
            studyComboBox.Name = "studyComboBox";
            studyComboBox.Size = new Size(250, 23);
            studyComboBox.TabIndex = 11;
            studyComboBox.SelectedIndexChanged += StudyComboBox_SelectedIndexChanged;
            // 
            // lossStopCheckBox
            // 
            lossStopCheckBox.AutoSize = true;
            lossStopCheckBox.Location = new Point(345, 269);
            lossStopCheckBox.Name = "lossStopCheckBox";
            lossStopCheckBox.Size = new Size(100, 19);
            lossStopCheckBox.TabIndex = 32;
            lossStopCheckBox.Text = "Loss Stop (%):";
            lossStopCheckBox.UseVisualStyleBackColor = true;
            lossStopCheckBox.CheckedChanged += StrategyControls_CheckedChanged;
            // 
            // valueRadioButton
            // 
            valueRadioButton.AutoSize = true;
            valueRadioButton.Location = new Point(323, 36);
            valueRadioButton.Name = "valueRadioButton";
            valueRadioButton.Size = new Size(53, 19);
            valueRadioButton.TabIndex = 10;
            valueRadioButton.Text = "Value";
            valueRadioButton.UseVisualStyleBackColor = true;
            valueRadioButton.CheckedChanged += StrategyControls_CheckedChanged;
            // 
            // posEffectComboBox
            // 
            posEffectComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            posEffectComboBox.FormattingEnabled = true;
            posEffectComboBox.Location = new Point(345, 238);
            posEffectComboBox.Name = "posEffectComboBox";
            posEffectComboBox.Size = new Size(164, 23);
            posEffectComboBox.TabIndex = 31;
            // 
            // posEffectLabel
            // 
            posEffectLabel.AutoSize = true;
            posEffectLabel.Location = new Point(345, 220);
            posEffectLabel.Name = "posEffectLabel";
            posEffectLabel.Size = new Size(147, 15);
            posEffectLabel.TabIndex = 28;
            posEffectLabel.Text = "Opening Position Method:";
            // 
            // studyRadioButton
            // 
            studyRadioButton.AutoSize = true;
            studyRadioButton.Checked = true;
            studyRadioButton.Location = new Point(6, 37);
            studyRadioButton.Name = "studyRadioButton";
            studyRadioButton.Size = new Size(55, 19);
            studyRadioButton.TabIndex = 9;
            studyRadioButton.TabStop = true;
            studyRadioButton.Text = "Study";
            studyRadioButton.UseVisualStyleBackColor = true;
            studyRadioButton.CheckedChanged += StrategyControls_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 985);
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Form1";
            Resize += Form1_Resize;
            tabControl1.ResumeLayout(false);
            graphTab.ResumeLayout(false);
            plotTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainPlotPictureBox).EndInit();
            plotUpperControlsPanel.ResumeLayout(false);
            plotUpperControlsPanel.PerformLayout();
            studiesTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            strategiesTab.ResumeLayout(false);
            strategiesTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)resultsDataGridView).EndInit();
            conditionGeneratorGB.ResumeLayout(false);
            conditionGeneratorGB.PerformLayout();
            additionalParametersGB.ResumeLayout(false);
            additionalParametersGB.PerformLayout();
            groupBox1.ResumeLayout(false);
            currentConditionGB.ResumeLayout(false);
            currentConditionGB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)displaceNumericUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button loadFileButton;
        private CheckBox logScaleCheckBox;
        private TabControl tabControl1;
        private TabPage graphTab;
        private TabPage studiesTab;
        private Panel plotUpperControlsPanel;
        private TabPage strategiesTab;
        private DataGridView dataGridView1;
        private Button loadFileButton2;
        private Label label2;
        private NumericUpDown displaceNumericUpDown;
        private GroupBox conditionGeneratorGB;
        private RadioButton valueRadioButton;
        private RadioButton studyRadioButton;
        private TextBox valueTextBox;
        private ComboBox studyComboBox;
        private GroupBox groupBox1;
        private Button plusButton;
        private Button lessThanButton;
        private Button rightParentButton;
        private Button leftParentButton;
        private Button multButton;
        private Button divButton;
        private Button minusButton;
        private Button notEqualButton;
        private Button greaterThanButton;
        private Button greaterEqThanButton;
        private Button equalButton;
        private Button lessEqThanButton;
        private Button andButton;
        private Button addConditionButton;
        private Button orButton;
        private ComboBox posEffectComboBox;
        private Label posEffectLabel;
        private CheckBox lossStopCheckBox;
        private TextBox gainStopTextBox;
        private CheckBox gainStopCheckBox;
        private TextBox lossStopTextBox;
        private Button crossesAboveButton;
        private Button crossesBelowButton;
        private GroupBox currentConditionGB;
        private Button clearButton;
        private Button backspaceButton;
        private Label expressionLabel;
        private Button addStudyValueButton;
        private GroupBox additionalParametersGB;
        private TextBox lengthTextBox;
        private Label marketDatatypeLabel;
        private ComboBox marketDatatypeComboBox;
        private Label lengthLabel;
        private ComboBox movAvg1ComboBox;
        private Label movAvg1Label;
        private Label stDevLabel;
        private TextBox atrsTextBox;
        private Label atrsLabel;
        private TextBox stDevTextBox;
        private ComboBox movAvg2ComboBox;
        private Label movAvg2Label;
        private Label specificStudyLabel;
        private ComboBox specificStudyComboBox;
        private DataGridView resultsDataGridView;
        private Label conditionsListLabel;
        private Button startOverButton;
        private Button computeButton;
        private DataGridViewTextBoxColumn OpeningIndexCol;
        private DataGridViewTextBoxColumn ClosingIndexCol;
        private DataGridViewTextBoxColumn GainLossCol;
        private TableLayoutPanel plotTableLayoutPanel;
        private PictureBox mainPlotPictureBox;
        private Panel priceLevelPanel;
        private Button editStudiesButton;
    }
}