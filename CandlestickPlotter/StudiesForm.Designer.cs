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
            label1 = new Label();
            label2 = new Label();
            okButton = new Button();
            cancelButton = new Button();
            availableStudiesListBox = new ListBox();
            addStudy = new Button();
            addedStudiesListBox = new ListBox();
            panel1 = new Panel();
            editButton = new Button();
            removeButton = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(99, 15);
            label1.TabIndex = 1;
            label1.Text = "Available Studies:";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.Location = new Point(0, 0);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 3;
            label2.Text = "Added Studies:";
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.AutoEllipsis = true;
            okButton.Location = new Point(713, 424);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(632, 424);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;
            // 
            // availableStudiesListBox
            // 
            availableStudiesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            availableStudiesListBox.FormattingEnabled = true;
            availableStudiesListBox.ItemHeight = 15;
            availableStudiesListBox.Location = new Point(12, 27);
            availableStudiesListBox.Name = "availableStudiesListBox";
            availableStudiesListBox.Size = new Size(169, 394);
            availableStudiesListBox.TabIndex = 6;
            // 
            // addStudy
            // 
            addStudy.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            addStudy.Location = new Point(12, 424);
            addStudy.Name = "addStudy";
            addStudy.Size = new Size(169, 23);
            addStudy.TabIndex = 7;
            addStudy.Text = "Add Selected";
            addStudy.UseVisualStyleBackColor = true;
            addStudy.Click += AddSelected_Click;
            // 
            // addedStudiesListBox
            // 
            addedStudiesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            addedStudiesListBox.FormattingEnabled = true;
            addedStudiesListBox.ItemHeight = 15;
            addedStudiesListBox.Location = new Point(251, 27);
            addedStudiesListBox.Name = "addedStudiesListBox";
            addedStudiesListBox.Size = new Size(537, 394);
            addedStudiesListBox.TabIndex = 8;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(label2);
            panel1.Location = new Point(251, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(537, 15);
            panel1.TabIndex = 9;
            // 
            // editButton
            // 
            editButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            editButton.Enabled = false;
            editButton.Location = new Point(251, 424);
            editButton.Name = "editButton";
            editButton.Size = new Size(75, 23);
            editButton.TabIndex = 11;
            editButton.Text = "Edit";
            editButton.UseVisualStyleBackColor = true;
            editButton.Click += EditButton_Click;
            // 
            // removeButton
            // 
            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            removeButton.Enabled = false;
            removeButton.Location = new Point(332, 424);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(75, 23);
            removeButton.TabIndex = 10;
            removeButton.Text = "Remove";
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += RemoveButton_Click;
            // 
            // StudiesForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(800, 450);
            Controls.Add(editButton);
            Controls.Add(removeButton);
            Controls.Add(panel1);
            Controls.Add(addedStudiesListBox);
            Controls.Add(addStudy);
            Controls.Add(availableStudiesListBox);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(label1);
            MinimumSize = new Size(600, 0);
            Name = "StudiesForm";
            Text = "Edit Studies";
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private Button okButton;
        private Button cancelButton;
        private ListBox availableStudiesListBox;
        private Button addStudy;
        private ListBox addedStudiesListBox;
        private Panel panel1;
        private Button editButton;
        private Button removeButton;
    }
}