using CandleStickPlotter.Forms;
using CandleStickPlotter.Studies;

namespace CandleStickPlotter.CustomControls
{
    internal class StudyListPanel : Panel
    {
        private const int DefaultItemHeight = 50;
        private int SelectedPanelIndex { get; set; } = -1;
        public List<List<Study>> StudyList { get; set; } = [];
        public StudyGroupPanel this[Index index]
        {
            get { return (StudyGroupPanel)Controls[index]; }
        }
        public StudyListPanel() { }
        public void Initialize()
        {
            StudyGroupPanel sgp1 = new(Width, "Price")
            {
                Location = new Point(0, 0),
                Margin = new Padding(0,5,0,5)
            };
            StudyGroupPanel sgp2 = new(Width, "Lower")
            {
                Location = new Point(0, sgp1.Bottom),
                Margin = new Padding(0, 5, 0, 5)
            };
            AddStudyGroupPanel(sgp1);
            AddStudyGroupPanel(sgp2);
            AddStudies();
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            AutoScroll = false;
            HorizontalScroll.Enabled = false;
            HorizontalScroll.Visible = false;
            HorizontalScroll.Maximum = 0;
            AutoScroll = true;
        }
        private void AddStudyGroupPanel(StudyGroupPanel studyGroupPanel)
        {
            Controls.Add(studyGroupPanel);
        }
        /// <summary>
        /// It adds the studies to the form given the StudyList. It creates the proper amount of
        /// StudyGroupPanels and their respective StudyGroupItems.
        /// </summary>
        private void AddStudies()
        {
            if (StudyList.Count > 0)
            {
                List<Study> studyList = StudyList[0];
                StudyGroupPanel priceSgp = this[0];
                // Add the Price studies
                for (int i = 0; i < studyList.Count; i++)
                {
                    if (i == 0)
                        priceSgp[0].Study = studyList[0];
                    else
                        priceSgp.AddStudy(studyList[i]);
                    SetEventHandlers(priceSgp[i]);
                }
                // Add the Lower studies
                StudyGroupPanel currentSgp;
                for (int i = 1; i < StudyList.Count; i++)
                {
                    studyList = StudyList[i];
                    currentSgp = this[i];
                    StudyGroupPanel sgpNew = new(Width, "Lower")
                    {
                        Location = new Point(0, this[^1].Bottom + 1)
                    };
                    for (int j = 0; j < studyList.Count; j++)
                    {
                        if (j == 0)
                            currentSgp[0].Study = studyList[0];
                        else
                            currentSgp.AddStudy(studyList[j]);
                        SetEventHandlers(currentSgp[j]);
                    }
                    Controls.Add(sgpNew);
                }
            }
            ResizeAndShiftAllPanels();
        }
        public void EditSelectedStudy()
        {
            // No study has been selected, so do nothing
            if (SelectedPanelIndex < 0) return;
            // Get the currently selected panel
            StudyGroupPanel selectedPanel = this[SelectedPanelIndex];
            if (selectedPanel.SelectedStudy == null) return;
            EditStudyForm editStudyForm = new(selectedPanel.SelectedStudy);
            if (editStudyForm.ShowDialog() == DialogResult.OK)
                selectedPanel.UpdateSelectedStudyText();
        }
        public List<List<Study>> GetStudies()
        {
            List<List<Study>> studiesList = [];
            StudyGroupPanel sgp;
            for (int i = 0; i < Controls.Count; i++)
            {
                sgp = this[i];
                List<Study> innerList = [];
                StudyGroupItem sgi;
                for (int j = 0; j < sgp.Count; j++)
                {
                    sgi = sgp[j];
                    if (sgi.Study is null) continue;
                    innerList.Add(sgi.Study);
                }
                if (innerList.Count > 0 || i == 0)
                    studiesList.Add(innerList);
            }
            return studiesList;
        }
        /// <summary>
        /// It moves the currently selected study down by one position. If the study is at the bottom
        /// position within its StudyGroup then the study will be moved to the top position of
        /// the StudyGroup above.
        /// </summary>
        public void MoveSelectedStudyDown()
        {
            // No study has been selected, so do nothing
            if (SelectedPanelIndex < 0) return;
            // Get the currently selected panel
            StudyGroupPanel selectedPanel = this[SelectedPanelIndex];
            // If the last item of the panel is selected
            if (selectedPanel.SelectedItemIndex == selectedPanel.Count - 1)
            {
                // The selected study is already at the bottom don't do anything
                if (SelectedPanelIndex == Controls.Count - 1) return;
                // If the selected study is not at the bottom:
                StudyGroupPanel sgpBelow = this[SelectedPanelIndex + 1];
                StudyGroupItem itemToMove = selectedPanel[^1];
                // If the StudyGroupPanel below is empty then do nothing
                if (sgpBelow[0].Study is null) return;
                // Add the study to the StudyGroupPanel below and move it to the top
                sgpBelow.StudyPanel.Controls.Add(itemToMove);
                sgpBelow.StudyPanel.Controls.SetChildIndex(itemToMove, 0);
                selectedPanel.SelectedItemIndex = -1;
                sgpBelow.SelectedItemIndex = 0;
                // If the study being moved is the last one in the selected panel
                if (selectedPanel.Count == 0)
                {
                    // If it is the Price StudyGroupPanel, then add an empty study, otherwise, remove the empty
                    // StudyGroupPanel
                    if (selectedPanel.Index == 0)
                    {
                        selectedPanel.AddStudy(null);
                        SelectedPanelIndex = 1;
                    }
                    else
                        RemoveStudyGroupPanelAt(selectedPanel.Index);
                }
                else
                    SelectedPanelIndex = sgpBelow.Index;
                ResizeAndShiftAllPanels();
            }
            // It's not the top item in the selected panel
            else
            {
                // Swap studies
                StudyGroupItem item = selectedPanel[selectedPanel.SelectedItemIndex];
                StudyGroupItem itemBelow = selectedPanel[selectedPanel.SelectedItemIndex + 1];
                Study? temp = item.Study;
                item.Study = itemBelow.Study;
                itemBelow.Study = temp;
                selectedPanel.SelectedItemIndex++;
                SelectItem(itemBelow);
                DeselectItem(item);
            }
        }
        /// <summary>
        /// It moves the currently selected study up by one position. If the study is at the top
        /// position within its StudyGroup then the study will be moved to the bottom position of
        /// the StudyGroup above.
        /// </summary>
        public void MoveSelectedStudyUp()
        {
            // No study has been selected, so do nothing
            if (SelectedPanelIndex < 0) return;
            // Get the currently selected panel
            StudyGroupPanel selectedPanel = this[SelectedPanelIndex];
            // If the first item of the panel is selected
            if (selectedPanel.SelectedItemIndex == 0)
            {
                // The selected study is already at the top:
                if (SelectedPanelIndex == 0) return;
                // If the selected study is not at the top:
                StudyGroupPanel sgpAbove = this[SelectedPanelIndex - 1];
                StudyGroupItem itemToMove = selectedPanel[0];
                // If the panel above has only one study and it's empty (basically an empty Price StudyGroupPanel)
                if (sgpAbove[^1].Study is  null)
                {
                    // Remove empty study
                    sgpAbove.RemoveStudyAt(0);
                    sgpAbove.StudyPanel.Controls.Add(itemToMove);
                }
                else
                {
                    //itemToMove.Index = sgpAbove.Count;
                    sgpAbove.StudyPanel.Controls.Add(itemToMove);
                }
                //selectedPanel.RemoveStudyAt(itemToMove.Index);
                SelectedPanelIndex = sgpAbove.Index;
                selectedPanel.SelectedItemIndex = -1;
                sgpAbove.SelectedItemIndex = sgpAbove.Count - 1;
                // If the study being moved is the last one in the selected panel, then delete the panel
                if (selectedPanel.Count == 0)
                    RemoveStudyGroupPanelAt(selectedPanel.Index);
                ResizeAndShiftAllPanels();
            }
            // It's not the top item in the selected panel
            else
            {
                // Swap studies
                StudyGroupItem item = selectedPanel[selectedPanel.SelectedItemIndex];
                StudyGroupItem itemAbove = selectedPanel[selectedPanel.SelectedItemIndex - 1];
                Study? temp = item.Study;
                item.Study = itemAbove.Study;
                itemAbove.Study = temp;
                selectedPanel.SelectedItemIndex--;
                SelectItem(itemAbove);
                DeselectItem(item);
            }            
        }
        /// <summary>
        /// Deletes a StudyGroup with the given index and moves the subsequent panels up.
        /// </summary>
        /// <param name="index"></param>
        private void RemoveStudyGroupPanelAt(int index)
        {
            Controls.RemoveAt(index);
        }
        public void AddStudy(Study study)
        {
            StudyGroupPanel lastSgp;
            // The study is a lower study, so it needs a new StudyGroup by default
            if (study.DefaultPlotLocation == Study.PlotLocation.Lower)
            {
                lastSgp = this[^1];
                StudyGroupPanel sgpNew = new(Width, "Lower")
                {
                    Location = new Point(0, lastSgp.Bottom + 1)
                };
                if (lastSgp[0].Study is null)
                    lastSgp[0].Study = study;
                Controls.Add(sgpNew);
            }
            // The study is a price study
            else
            {
                lastSgp = (StudyGroupPanel)Controls[0];
                if (lastSgp[0].Study is not null)
                    lastSgp.AddStudy(study);
                else lastSgp[0].Study = study;
            }
            SetEventHandlers(lastSgp[^1]);
            ResizeAndShiftAllPanels();
        }
        public void RemoveSelectedStudy()
        {
            if (SelectedPanelIndex < 0) return;
            StudyGroupPanel sgp = this[SelectedPanelIndex];
            sgp.RemoveSelectedStudy();
            sgp.SelectedItemIndex = -1;
            SelectedPanelIndex = -1;
            // It was the last study in the panel
            if (sgp.Count == 0)
            {
                // It's a price study, so keep the "empty" study
                if (sgp.Index == 0)
                    sgp.AddStudy(null);
                // It's not the last StudyGroupPanel for lower studies
                else
                    RemoveStudyGroupPanelAt(sgp.Index);
            }
            ResizeAndShiftAllPanels();
        }
        private void SetEventHandlers(StudyGroupItem sgi)
        {
            sgi.MouseEnter += StudyGroupItem_MouseEnter;
            sgi.MouseLeave += StudyGroupItem_MouseLeave;
            sgi.Click += StudyGroupItem_Click;
        }

        private void ResizeAndShiftAllPanels()
        {
            StudyGroupPanel sgp;
            for (int i = 0; i < Controls.Count; i++)
            {
                sgp = this[i];
                if (i == 0) sgp.Location = new Point(0, 0);
                else sgp.Location = new Point(0, this[i - 1].Bottom);
                sgp.Index = i;
                sgp.Size = new Size(sgp.Width, sgp.Count * DefaultItemHeight);
                StudyGroupItem sgi;
                for (int j = 0; j < sgp.Count; j++)
                {
                    sgi = sgp[j];
                    if (j == 0) sgi.Location = new Point(0, 0);
                    else sgi.Location = new Point(0, sgp[j - 1].Bottom);
                    if (sgi.Study is not null)
                        sgi.Index = j;
                    sgi.Size = new Size(sgp.StudyPanel.Width, DefaultItemHeight);
                }
                sgp.StudyPanel.Size = new Size(sgp.StudyPanel.Width, sgp[^1].Bottom + 1);
            }
        }
        /// <summary>
        /// Deselects a given item.
        /// </summary>
        /// <param name="item">The item that will be unselected.</param>
        private static void DeselectItem(StudyGroupItem item)
        {
            item.BackColor = Color.Transparent;
            item.ForeColor = Color.LightGray;
        }
        /// <summary>
        /// Selects a given item.
        /// </summary>
        /// <param name="item">The item that will be selected.</param>
        private static void SelectItem(StudyGroupItem item)
        {
            item.BackColor = Color.FromArgb(255, 8, 74, 102);
            item.ForeColor = Color.White;
        }
        private void StudyGroupItem_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is null) return;
            StudyGroupItem item = (StudyGroupItem)sender;
            // Parent once to get the StudyPanel and Parent once more to get the StudyGroupPanel
            StudyGroupPanel parent = (StudyGroupPanel)item.Parent!.Parent!;
            if (item.Index == parent.SelectedItemIndex && parent.Index == SelectedPanelIndex) return;
            SelectItem(item);
        }
        private void StudyGroupItem_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is null) return;
            StudyGroupItem item = (StudyGroupItem)sender;
            // Parent once to get the StudyPanel and Parent once more to get the StudyGroupPanel
            StudyGroupPanel parent = (StudyGroupPanel)item.Parent!.Parent!;
            if (item.Index == parent.SelectedItemIndex && parent.Index == SelectedPanelIndex) return;
            DeselectItem(item);
        }
        private void StudyGroupItem_Click(object? sender, EventArgs e)
        {
            if (sender is null) return;
            // Get the StudyGroupItem from the sender object and then get it's parent
            // which should be a StudyGroupPanel
            StudyGroupItem item = (StudyGroupItem)sender;
            // Parent once to get the StudyPanel and Parent once more to get the StudyGroupPanel
            StudyGroupPanel parent = (StudyGroupPanel)item.Parent!.Parent!;
            // User clicked on an already selected item, so deselect it
            if (item.Index == parent.SelectedItemIndex && parent.Index == SelectedPanelIndex)
            {
                parent.SelectedItemIndex = -1;
                SelectedPanelIndex = -1;
                DeselectItem(item);
            }
            // Nothing is selected
            else if (SelectedPanelIndex < 0)
            {
                SelectItem(item);
                parent.SelectedItemIndex = item.Index;
                SelectedPanelIndex = parent.Index;
            }
            else
            {
                StudyGroupPanel previouslySelectedPanel = this[SelectedPanelIndex];
                // A study is selected
                if (previouslySelectedPanel.SelectedItemIndex >= 0)
                {
                    StudyGroupItem previouslySelectedItem = previouslySelectedPanel[previouslySelectedPanel.SelectedItemIndex];
                    DeselectItem(previouslySelectedItem);
                }
                SelectItem(item);
                parent.SelectedItemIndex = item.Index;
                SelectedPanelIndex = parent.Index;
            }
        }
        #region StudyGroupPanel
        internal class StudyGroupPanel : Panel
        {
            // Height of items in the list
            public const int DefaultHeight = 50;
            public Label NameLabel { get; private set; }
            // The panel that will contain the studies
            public Panel StudyPanel { get; private set; }
            // Defualt margin of the items in the list
            public static Padding DefaultCustomMargin = new(5);
            public int Count => StudyPanel.Controls.Count;
            public int Index { get; set; }
            public int ItemWidth => StudyPanel.Width;
            public int SelectedItemIndex { get; set; } = -1;
            public Study? SelectedStudy => SelectedItemIndex < 0 ? null : this[SelectedItemIndex].Study;
            public StudyGroupItem this[int index] => (StudyGroupItem)StudyPanel.Controls[index];
            public StudyGroupPanel(int width, string name)
            {
                Width = width;
                BorderStyle = BorderStyle.FixedSingle;
                Padding = new Padding(0);
                Margin = new Padding(0);
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                NameLabel = new Label()
                {
                    Text = name,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Left,
                    ForeColor = Color.LightGray,
                    Margin = new Padding(0)
                };
                Controls.Add(NameLabel);
                StudyPanel = new Panel()
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                    Margin = new Padding(0),
                    Location = new Point(NameLabel.Right, 0),
                    Padding = new Padding(0),
                    Size = new Size(Width - NameLabel.Width, DefaultHeight),
                    BorderStyle = BorderStyle.None,
                };
                // Add an empty study to get the <empty> item
                AddStudy(null);
                Controls.Add(StudyPanel);
            }
            /// <summary>
            /// Appends a study to the StudyGroup at the end.
            /// </summary>remo
            /// <param name="study">Study to add. If it's null, the item will have the default text.</param>
            public void AddStudy(Study? study)
            {
                StudyGroupItem item = study != null ? new(study, Count) : new(study);
                StudyPanel.Controls.Add(item);
            }
            /// <summary>
            /// Deletes the selected study from the StudyGroup and moves up every study below by one position.
            /// </summary>
            /// <param name="index">Index of the study to remove.</param>
            public void RemoveSelectedStudy()
            {
                RemoveStudyAt(SelectedItemIndex);
            }
            /// <summary>
            /// Deletes a study from the StudyGroup and moves up every study below by one position.
            /// </summary>
            /// <param name="index">Index of the study to remove.</param>
            public void RemoveStudyAt(int index)
            {
                StudyPanel.Controls.RemoveAt(index);
            }
            /// <summary>
            /// Necessary override to add a border to the panel.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            }
            public void UpdateSelectedStudyText()
            {
                if (SelectedStudy is not null)
                    this[SelectedItemIndex].Text = SelectedStudy.NameString();
            }
        }
        #endregion
        #region StudyGroupItem
        /// <summary>
        /// Item used in a StudyGroupPanel.
        /// </summary>
        internal class StudyGroupItem : Label
        {
            // Default text is used only for empty study lists.
            // More than one empty (null) study can be added to a list, but that is not the intended use
            // and will not work properly.
            private readonly string DefaultText = "<empty>";
            private Study? study = null;
            public int Index { get; set; } = -1;
            public Study? Study
            {
                get
                {
                    return study;
                }
                set
                {
                    study = value;
                    if (value is null) Text = DefaultText;
                    else Text = value.NameString();
                }
            }
            public StudyGroupItem(Study? study) : base()
            {
                Study = study;
                ForeColor = Color.LightGray;
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                TextAlign = ContentAlignment.MiddleLeft;
            }
            public StudyGroupItem(Study? study, int index) : base()
            {
                Study = study;
                Index = index;
                ForeColor = Color.LightGray;
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                TextAlign = ContentAlignment.MiddleLeft;
            }
        }
        #endregion
    }
}