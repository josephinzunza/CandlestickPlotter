namespace CandleStickPlotter.Forms
{
    partial class EditStudyForm
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
            parametersGroupBox = new GroupBox();
            studyNameLabel = new Label();
            acceptButton = new Button();
            cancelButton = new Button();
            plotsGroupBox = new GroupBox();
            plotsTabControl = new TabControl();
            colorDialog1 = new ColorDialog();
            plotsGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // parametersGroupBox
            // 
            parametersGroupBox.AutoSize = true;
            parametersGroupBox.Location = new Point(12, 35);
            parametersGroupBox.Name = "parametersGroupBox";
            parametersGroupBox.Padding = new Padding(10, 20, 10, 20);
            parametersGroupBox.Size = new Size(368, 173);
            parametersGroupBox.TabIndex = 0;
            parametersGroupBox.TabStop = false;
            parametersGroupBox.Text = "Parameters";
            // 
            // studyNameLabel
            // 
            studyNameLabel.Location = new Point(12, 9);
            studyNameLabel.Name = "studyNameLabel";
            studyNameLabel.Size = new Size(368, 23);
            studyNameLabel.TabIndex = 1;
            studyNameLabel.Text = "Study Name";
            // 
            // acceptButton
            // 
            acceptButton.Anchor = AnchorStyles.None;
            acceptButton.Location = new Point(224, 415);
            acceptButton.Name = "acceptButton";
            acceptButton.Size = new Size(75, 23);
            acceptButton.TabIndex = 2;
            acceptButton.Text = "Accept";
            acceptButton.UseVisualStyleBackColor = true;
            acceptButton.Click += AcceptButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.None;
            cancelButton.Location = new Point(305, 415);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // plotsGroupBox
            // 
            plotsGroupBox.Controls.Add(plotsTabControl);
            plotsGroupBox.Location = new Point(12, 214);
            plotsGroupBox.Name = "plotsGroupBox";
            plotsGroupBox.Size = new Size(368, 195);
            plotsGroupBox.TabIndex = 4;
            plotsGroupBox.TabStop = false;
            plotsGroupBox.Text = "Plots";
            // 
            // plotsTabControl
            // 
            plotsTabControl.Location = new Point(6, 22);
            plotsTabControl.Name = "plotsTabControl";
            plotsTabControl.SelectedIndex = 0;
            plotsTabControl.Size = new Size(356, 167);
            plotsTabControl.TabIndex = 0;
            // 
            // EditStudyForm
            // 
            AcceptButton = acceptButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            CancelButton = cancelButton;
            ClientSize = new Size(392, 450);
            Controls.Add(plotsGroupBox);
            Controls.Add(cancelButton);
            Controls.Add(acceptButton);
            Controls.Add(studyNameLabel);
            Controls.Add(parametersGroupBox);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            Name = "EditStudyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Edit Study";
            Load += EditStudyForm_Load;
            plotsGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox parametersGroupBox;
        private Label studyNameLabel;
        private Button acceptButton;
        private Button cancelButton;
        private GroupBox plotsGroupBox;
        private TabControl plotsTabControl;
        private ColorDialog colorDialog1;
    }
}