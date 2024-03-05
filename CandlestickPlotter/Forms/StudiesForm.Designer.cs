namespace CandleStickPlotter
{
    partial class StudiesForm
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
            availableStudiesLabel = new Label();
            addedStudiesLabel = new Label();
            okButton = new Button();
            cancelButton = new Button();
            availableStudiesListBox = new ListBox();
            addStudy = new Button();
            editButton = new Button();
            removeButton = new Button();
            addedStudiesLabelPanel = new Panel();
            moveDownButton = new Button();
            moveUpButton = new Button();
            studyListPanel = new CustomControls.StudyListPanel();
            addedStudiesLabelPanel.SuspendLayout();
            SuspendLayout();
            // 
            // availableStudiesLabel
            // 
            availableStudiesLabel.AutoSize = true;
            availableStudiesLabel.BackColor = Color.FromArgb(51, 51, 51);
            availableStudiesLabel.ForeColor = Color.LightGray;
            availableStudiesLabel.Location = new Point(12, 9);
            availableStudiesLabel.Name = "availableStudiesLabel";
            availableStudiesLabel.Size = new Size(99, 15);
            availableStudiesLabel.TabIndex = 1;
            availableStudiesLabel.Text = "Available Studies:";
            // 
            // addedStudiesLabel
            // 
            addedStudiesLabel.Dock = DockStyle.Left;
            addedStudiesLabel.ForeColor = Color.LightGray;
            addedStudiesLabel.Location = new Point(0, 0);
            addedStudiesLabel.Margin = new Padding(0);
            addedStudiesLabel.Name = "addedStudiesLabel";
            addedStudiesLabel.Size = new Size(86, 15);
            addedStudiesLabel.TabIndex = 3;
            addedStudiesLabel.Text = "Added Studies:";
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.AutoEllipsis = true;
            okButton.BackColor = Color.FromArgb(0, 147, 0);
            okButton.FlatStyle = FlatStyle.Flat;
            okButton.ForeColor = Color.White;
            okButton.Location = new Point(713, 415);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 25);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = false;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.BackColor = Color.FromArgb(29, 29, 29);
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.ForeColor = Color.LightGray;
            cancelButton.Location = new Point(632, 415);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 25);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += CancelButton_Click;
            // 
            // availableStudiesListBox
            // 
            availableStudiesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            availableStudiesListBox.BackColor = Color.Black;
            availableStudiesListBox.ForeColor = Color.White;
            availableStudiesListBox.FormattingEnabled = true;
            availableStudiesListBox.ItemHeight = 15;
            availableStudiesListBox.Location = new Point(12, 27);
            availableStudiesListBox.Name = "availableStudiesListBox";
            availableStudiesListBox.Size = new Size(169, 379);
            availableStudiesListBox.TabIndex = 6;
            // 
            // addStudy
            // 
            addStudy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addStudy.BackColor = Color.FromArgb(29, 29, 29);
            addStudy.FlatStyle = FlatStyle.Flat;
            addStudy.ForeColor = Color.LightGray;
            addStudy.Location = new Point(12, 415);
            addStudy.Name = "addStudy";
            addStudy.Size = new Size(169, 25);
            addStudy.TabIndex = 7;
            addStudy.Text = "Add Selected";
            addStudy.UseVisualStyleBackColor = false;
            addStudy.Click += AddSelected_Click;
            // 
            // editButton
            // 
            editButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            editButton.BackColor = Color.FromArgb(29, 29, 29);
            editButton.BackgroundImage = Properties.Resources.settingsIcon;
            editButton.BackgroundImageLayout = ImageLayout.Stretch;
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.ForeColor = Color.LightGray;
            editButton.Location = new Point(344, 415);
            editButton.Name = "editButton";
            editButton.Size = new Size(25, 25);
            editButton.TabIndex = 11;
            editButton.UseVisualStyleBackColor = false;
            editButton.Click += EditButton_Click;
            // 
            // removeButton
            // 
            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            removeButton.BackColor = Color.FromArgb(29, 29, 29);
            removeButton.BackgroundImage = Properties.Resources.deleteIcon;
            removeButton.BackgroundImageLayout = ImageLayout.Stretch;
            removeButton.FlatStyle = FlatStyle.Flat;
            removeButton.ForeColor = Color.LightGray;
            removeButton.Location = new Point(313, 415);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(25, 25);
            removeButton.TabIndex = 10;
            removeButton.UseVisualStyleBackColor = false;
            removeButton.Click += RemoveButton_Click;
            // 
            // addedStudiesLabelPanel
            // 
            addedStudiesLabelPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            addedStudiesLabelPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            addedStudiesLabelPanel.BackColor = Color.FromArgb(51, 51, 51);
            addedStudiesLabelPanel.Controls.Add(addedStudiesLabel);
            addedStudiesLabelPanel.Location = new Point(251, 9);
            addedStudiesLabelPanel.Name = "addedStudiesLabelPanel";
            addedStudiesLabelPanel.Size = new Size(537, 15);
            addedStudiesLabelPanel.TabIndex = 13;
            // 
            // moveDownButton
            // 
            moveDownButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            moveDownButton.BackColor = Color.FromArgb(29, 29, 29);
            moveDownButton.BackgroundImage = Properties.Resources.arrowDownIcon;
            moveDownButton.BackgroundImageLayout = ImageLayout.Stretch;
            moveDownButton.FlatStyle = FlatStyle.Flat;
            moveDownButton.ForeColor = Color.LightGray;
            moveDownButton.Location = new Point(282, 415);
            moveDownButton.Name = "moveDownButton";
            moveDownButton.Size = new Size(25, 25);
            moveDownButton.TabIndex = 14;
            moveDownButton.UseVisualStyleBackColor = false;
            moveDownButton.Click += MoveDownButton_Click;
            // 
            // moveUpButton
            // 
            moveUpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            moveUpButton.BackColor = Color.FromArgb(29, 29, 29);
            moveUpButton.BackgroundImage = Properties.Resources.arrowUpIcon;
            moveUpButton.BackgroundImageLayout = ImageLayout.Stretch;
            moveUpButton.FlatStyle = FlatStyle.Flat;
            moveUpButton.ForeColor = Color.LightGray;
            moveUpButton.Location = new Point(251, 415);
            moveUpButton.Name = "moveUpButton";
            moveUpButton.Size = new Size(25, 25);
            moveUpButton.TabIndex = 15;
            moveUpButton.UseVisualStyleBackColor = false;
            moveUpButton.Click += MoveUpButton_Click;
            // 
            // studyListPanel
            // 
            studyListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            studyListPanel.Location = new Point(251, 27);
            studyListPanel.Name = "studyListPanel";
            studyListPanel.Size = new Size(537, 379);
            studyListPanel.TabIndex = 16;
            // 
            // StudiesForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            CancelButton = cancelButton;
            ClientSize = new Size(800, 452);
            Controls.Add(studyListPanel);
            Controls.Add(moveUpButton);
            Controls.Add(moveDownButton);
            Controls.Add(addedStudiesLabelPanel);
            Controls.Add(editButton);
            Controls.Add(removeButton);
            Controls.Add(addStudy);
            Controls.Add(availableStudiesListBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(availableStudiesLabel);
            MinimumSize = new Size(800, 400);
            Name = "StudiesForm";
            Text = "Edit Studies";
            Load += StudyGroupList_Load;
            addedStudiesLabelPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label availableStudiesLabel;
        private Label addedStudiesLabel;
        private Button okButton;
        private Button cancelButton;
        private ListBox availableStudiesListBox;
        private Button addStudy;
        private Button editButton;
        private Button removeButton;
        private Panel addedStudiesLabelPanel;
        private Button moveDownButton;
        private Button moveUpButton;
        private CustomControls.StudyListPanel studyListPanel;
    }
}