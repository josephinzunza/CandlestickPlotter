using CandleStickPlotter.Strategies;
using System.Text.Json;

namespace CandleStickPlotter
{
    public partial class StudiesForm : Form
    {
        public StudiesForm(List<string> studiesJsonStrings, List<StudyType> studyTypes)
        {
            InitializeComponent();
            if (studiesJsonStrings.Count != studyTypes.Count)
                throw new ArgumentException("List 'studiesJsonStrings' has not the same length as list 'studyTypes'.");
            foreach (StudyType study in Enum.GetValues(typeof(StudyType)))
                availableStudiesListBox.Items.Add(study);
            availableStudiesListBox.SelectedIndex = 0;

        }
        private void AddSelected_Click(object sender, EventArgs e)
        {

        }
        private void EditButton_Click(object sender, EventArgs e)
        {

        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {

        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void OkButton_Click(object sender, EventArgs e)
        {

        }
        private void AddJsonStudies(List<string> studiesJsonStrings, List<StudyType> studyTypes)
        {
            
            Type currentStudyType;
            for (int i = 0; i <  studiesJsonStrings.Count; i++)
            {
                currentStudyType = Utils.Utils.GetSudyType(studyTypes[i]);
            }
        }
    }
}
