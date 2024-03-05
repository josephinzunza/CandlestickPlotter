using CandleStickPlotter.Strategies;
using CandleStickPlotter.Studies;
using CandleStickPlotter.Studies.Momentum;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Tools;
using CandleStickPlotter.Studies.Volatility;

namespace CandleStickPlotter
{
    public partial class StudiesForm : Form
    {
        public List<List<Study>> Studies { get; private set; } = [];
        public StudiesForm(List<List<Study>> studyList)
        {
            InitializeComponent();
            Studies = studyList;
            studyListPanel.StudyList = Studies;
            foreach (StudyType study1 in Enum.GetValues(typeof(StudyType)))
                availableStudiesListBox.Items.Add(study1);
            availableStudiesListBox.SelectedIndex = 0;
        }
        private void AddSelected_Click(object sender, EventArgs e)
        {
            if (availableStudiesListBox.SelectedItem is null)
            {
                MessageBox.Show("No stduy has been selected","Error",MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            Study studyToAdd = GetDefaultStudy(Enum.Parse<StudyType>(availableStudiesListBox.GetItemText(availableStudiesListBox.SelectedItem)!));
            studyListPanel.AddStudy(studyToAdd);
        }
        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            studyListPanel.MoveSelectedStudyDown();
        }
        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            studyListPanel.MoveSelectedStudyUp();
        }
        private void EditButton_Click(object sender, EventArgs e)
        {
            studyListPanel.EditSelectedStudy();
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            studyListPanel.RemoveSelectedStudy();
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            Studies = studyListPanel.GetStudies();
            DialogResult = DialogResult.OK;
        }
        private void StudyGroupList_Load(object? sender, EventArgs e)
        {
            studyListPanel.Initialize();
        }
        private static Study GetDefaultStudy(StudyType studyType)
        {
            Study study = studyType switch
            {
                StudyType.AverageTrueRange => new AverageTrueRange(),
                StudyType.BollingerBands => new BollingerBands(),
                StudyType.DonchianChannel => new DonchianChannels(),
                StudyType.ExponentialMovingAverage => new ExponentialMovingAverage(),
                StudyType.KeltnerChannels => new KeltnerChannels(),
                StudyType.LeastSquaresLinearRegression => new LeastSquaresLinearRegression(),
                StudyType.RelativeStrengthIndex => new RelativeStrenghtIndex(),
                StudyType.SimpleMovingAverage => new SimpleMovingAverage(),
                StudyType.StandardDeviation => new StandardDeviation(),
                StudyType.TrueRange => new TrueRange(),
                StudyType.TTMSqueeze => new TTMSqueeze(),
                StudyType.WeigthedMovingAverage => new WeightedMovingAverage(),
                StudyType.WildersMovingAverage => new WildersMovingAverage(),
                _ => throw new NotImplementedException()
            };
            return study;
        }
    }
}
