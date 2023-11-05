using CandleStickPlotter.Strategies;

namespace CandleStickPlotter
{
    public partial class StudiesForm : Form
    {
        public StudiesForm()
        {
            InitializeComponent();
            foreach (AvailableStudies study in Enum.GetValues(typeof(AvailableStudies)))
                availableStudiesListBox.Items.Add(study);
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
    }
}
