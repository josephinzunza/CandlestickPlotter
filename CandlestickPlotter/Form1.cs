using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Strategies;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Utils;

namespace CandleStickPlotter
{
    public partial class Form1 : Form
    {
        private OhlcData? _ohlcData;
        private readonly Label[] priceLabels;
        private double min = double.NaN;
        private double max = double.NaN;
        private readonly List<string> currentExpression = new();
        private readonly List<string> expresionText = new();
        bool isClosingCondition = false;
        ClosingCondition closingCondition;
        OpeningCondition openingCondition;
        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowHeadersVisible = false;
            posEffectComboBox.DataSource = Enum.GetValues(typeof(PositionType));
            marketDatatypeComboBox.DataSource = Enum.GetValues(typeof(MarketDataType));
            movAvg1ComboBox.DataSource = Enum.GetValues(typeof(MovingAverageType));
            movAvg2ComboBox.DataSource = Enum.GetValues(typeof(MovingAverageType));
            studyComboBox.DataSource = Enum.GetValues(typeof(AvailableStudies));
            CheckedControlsChanged();
            resultsDataGridView.RowHeadersVisible = false;
            priceLabels = new Label[11];
            for (int i = 0; i < priceLabels.Length; i++)
            {
                priceLabels[i] = new()
                {
                    Visible = false,
                    ForeColor = Color.Gray
                };
                priceLevelPanel.Controls.Add(priceLabels[i]);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Plot();
        }

        private void Plot()
        {
            if (_ohlcData == null || double.IsNaN(min) || double.IsNaN(max)) return;
            Control mainPlotCell = plotTableLayoutPanel.GetControlFromPosition(0, 0);
            double yPixels = plotTableLayoutPanel.GetControlFromPosition(0, 0).Size.Height - 5; // 5 Pixels of combined vertical margin
            double pixelsPerDollar = yPixels / (max - min);
            Bitmap bitmap = new(mainPlotCell.Width, mainPlotCell.Height);
            Graphics plot = Graphics.FromImage(bitmap);
            Pen upTickPen = new(Color.Green, 1);
            Pen downTickPen = new(Color.Red, 1);
            SolidBrush downTickBrush = new(Color.Red);
            // Plot dotted lines and set the labels
            double divSize = (max - min) * pixelsPerDollar / 10;
            double linePos = (max - min) * pixelsPerDollar + 1;
            Pen dottedPen = new(Color.Gray, 1)
            {
                DashPattern = new float[] { 1, 9 }
            };
            for (int i = 0; i <= 10; i++)
            {
                priceLabels[i].Location = new System.Drawing.Point(0, (int)linePos - priceLabels[i].Height / 2);
                plot.DrawLine(dottedPen, 0, (float)linePos, mainPlotCell.Width, (float)linePos);
                linePos -= divSize;
            }
            priceLabels[10].Location = new Point(0, priceLabels[10].Location.Y + priceLabels[10].Height / 2);

            int xPos = 1;
            double yPos, height, low, high;
            int candleWidth = (mainPlotCell.Width - 251) / 250;
            float xTemp;
            for (int i = 0; i < _ohlcData.Length; i++)
            {
                xTemp = xPos + (candleWidth >> 1);
                if (_ohlcData.Open[i] < _ohlcData.Close[i])
                {
                    if (logScaleCheckBox.Checked)
                    {
                        height = (Math.Log10(_ohlcData.Close[i]) - Math.Log10(_ohlcData.Open[i])) * pixelsPerDollar;
                        yPos = (max - Math.Log10(_ohlcData.Close[i])) * pixelsPerDollar + 1;
                        high = (max - Math.Log10(_ohlcData.High[i])) * pixelsPerDollar + 1;
                        low = (max - Math.Log10(_ohlcData.Low[i])) * pixelsPerDollar + 1;
                    }
                    else
                    {
                        height = (_ohlcData.Close[i] - _ohlcData.Open[i]) * pixelsPerDollar;
                        yPos = (max - _ohlcData.Close[i]) * pixelsPerDollar + 1;
                        high = (max - _ohlcData.High[i]) * pixelsPerDollar + 1;
                        low = (max - _ohlcData.Low[i]) * pixelsPerDollar + 1;
                    }

                    plot.DrawRectangle(upTickPen, xPos, (float)yPos, candleWidth - 1, (float)height);
                    plot.DrawLine(upTickPen, xTemp, (float)high, xTemp, (float)yPos);
                    plot.DrawLine(upTickPen, xTemp, (float)(yPos + height), xTemp, (float)low);
                }
                else
                {
                    if (logScaleCheckBox.Checked)
                    {
                        height = (Math.Log10(_ohlcData.Open[i]) - Math.Log10(_ohlcData.Close[i])) * pixelsPerDollar;
                        yPos = (max - Math.Log10(_ohlcData.Open[i])) * pixelsPerDollar + 1;
                        high = (max - Math.Log10(_ohlcData.High[i])) * pixelsPerDollar + 1;
                        low = (max - Math.Log10(_ohlcData.Low[i])) * pixelsPerDollar + 1;
                    }
                    else
                    {
                        height = (_ohlcData.Open[i] - _ohlcData.Close[i]) * pixelsPerDollar;
                        yPos = (max - _ohlcData.Open[i]) * pixelsPerDollar + 1;
                        high = (max - _ohlcData.High[i]) * pixelsPerDollar + 1;
                        low = (max - _ohlcData.Low[i]) * pixelsPerDollar + 1;
                    }
                    plot.FillRectangle(downTickBrush, xPos, (float)yPos, candleWidth, (float)height);
                    plot.DrawLine(downTickPen, xTemp, (float)high, xTemp, (float)yPos);
                    plot.DrawLine(downTickPen, xTemp, (float)(yPos + height), xTemp, (float)low);
                }
                xPos = xPos + 1 + candleWidth;
            }
            mainPlotPictureBox.Image = bitmap;
        }

        private void GetMinMax()
        {
            if (_ohlcData == null) return;
            if (logScaleCheckBox.Checked)
            {
                max = Math.Log10(_ohlcData.High.Max());
                min = Math.Log10(_ohlcData.Low.Min());
            }
            else
            {
                max = _ohlcData.High.Max();
                min = _ohlcData.Low.Min();
            }
        }

        private void SetLabelPrices()
        {
            double priceIncrement = logScaleCheckBox.Checked ? (Math.Pow(10, max) - Math.Pow(10, min)) / 10 : (max - min) / 10;
            double currentPrice = logScaleCheckBox.Checked ? Math.Pow(10, min) : min;
            for (int i = 0; i <= 10; i++)
            {
                priceLabels[i].Text = string.Format("{0:0.00}", currentPrice);
                currentPrice += priceIncrement;
            }
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            StudiesForm studiesForm = new();
            studiesForm.ShowDialog();
            return;
            using OpenFileDialog ofd = new();
            ofd.Filter = "Comma-separated values files (*.csv)|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                _ohlcData = new OhlcData(filePath);
                foreach (Label label in priceLabels)
                    label.Visible = true;
                GetMinMax();
                SetLabelPrices();
                Plot();
            }
        }

        private void LogScaleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GetMinMax();
            Plot();
        }

        private void LoadFileButton2_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new();
            ofd.Filter = "Comma-separated values files (*.csv)|*.csv";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filePath = ofd.FileName;
                _ohlcData = new OhlcData(filePath);
                TableDouble lsReg = LeastSquaresLinearRegression.Calculate(_ohlcData.Close, 14);
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("date", "Date");
                dataGridView1.Columns.Add("slope", "Slope");
                dataGridView1.Columns.Add("intercept", "Intercept");
                dataGridView1.Columns.Add("predicted", "Predicted");
                dataGridView1.Columns.Add("r2", "R2");
                for (int i = 0; i < _ohlcData.Length; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = _ohlcData.Date[i].ToString();
                    dataGridView1.Rows[i].Cells[1].Value = lsReg[0][i].ToString();
                    dataGridView1.Rows[i].Cells[2].Value = lsReg[1][i].ToString();
                    dataGridView1.Rows[i].Cells[3].Value = lsReg[2][i].ToString();
                    dataGridView1.Rows[i].Cells[4].Value = lsReg[3][i].ToString();
                }
            }
        }
        private void CheckedControlsChanged()
        {
            studyComboBox.Enabled = studyRadioButton.Checked;
            displaceNumericUpDown.Enabled = studyRadioButton.Checked;
            valueTextBox.Enabled = valueRadioButton.Checked;
            lossStopTextBox.Enabled = lossStopCheckBox.Checked;
            gainStopTextBox.Enabled = gainStopCheckBox.Checked;
        }

        private void StrategyControls_CheckedChanged(object sender, EventArgs e)
        {
            CheckedControlsChanged();
        }
        private void PlusButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("+");
            expresionText.Add("+");
            expressionLabel.Text += "+ ";
        }
        private void MinusButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("-");
            expresionText.Add("-");
            expressionLabel.Text += "- ";
        }
        private void MultButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("*");
            expresionText.Add("*");
            expressionLabel.Text += "* ";
        }
        private void DivButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("/");
            expresionText.Add("/");
            expressionLabel.Text += "/ ";
        }
        private void LeftParenButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("(");
            expresionText.Add("(");
            expressionLabel.Text += "( ";
        }
        private void RightParenButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add(")");
            expresionText.Add(")");
            expressionLabel.Text += ") ";
        }
        private void CrossesAboveButton_Click(Object sender, EventArgs e)
        {
            currentExpression.Add("}");
            expresionText.Add("crosses above");
            expressionLabel.Text += "crosses above ";
        }
        private void AndButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("&");
            expresionText.Add("AND");
            expressionLabel.Text += "AND ";
        }
        private void BackspaceButton_Click(object sender, EventArgs e)
        {
            if (currentExpression.Count > 0)
                currentExpression.RemoveAt(currentExpression.Count - 1);
            if (expresionText.Count > 0)
                expresionText.RemoveAt(expresionText.Count - 1);
            expressionLabel.Text = "";
            foreach (string item in expresionText)
                expressionLabel.Text += item + " ";
        }
        private void LessThanButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("<");
            expresionText.Add("<");
            expressionLabel.Text += "< ";
        }
        private void LessEqThanButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("«");
            expresionText.Add("<=");
            expressionLabel.Text += "<= ";
        }
        private void EqualButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("=");
            expresionText.Add("=");
            expressionLabel.Text += "= ";
        }
        private void GreaterThanButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add(">");
            expresionText.Add(">");
            expressionLabel.Text += "> ";
        }
        private void GreaterEqThanButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("»");
            expresionText.Add(">=");
            expressionLabel.Text += ">= ";
        }
        private void NotEqualButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("!");
            expresionText.Add("!=");
            expressionLabel.Text += "!= ";
        }
        private void CrossesBelowButton_Click(Object sender, EventArgs e)
        {
            currentExpression.Add("{");
            expresionText.Add("crosses below");
            expressionLabel.Text += "crosses below ";
        }
        private void OrButton_Click(object sender, EventArgs e)
        {
            currentExpression.Add("|");
            expresionText.Add("OR");
            expressionLabel.Text += "OR ";
        }
        private void ClearButton_Click(object sender, EventArgs e)
        {
            currentExpression.Clear();
            expresionText.Clear();
            expressionLabel.Text = "";
        }
        private void AddStudyButton_Click(object sender, EventArgs e)
        {
            if (studyRadioButton.Checked)
            {
                string studyFullName = studyComboBox.Text;

                studyFullName += GetAdditionalParameters();
                if (specificStudyComboBox.Enabled)
                    studyFullName += '.' + specificStudyComboBox.Text;
                studyFullName += $"[{displaceNumericUpDown.Value}]";
                currentExpression.Add(studyFullName);
                expresionText.Add(studyFullName);
                expressionLabel.Text += studyFullName + " ";
            }
            else
            {
                currentExpression.Add(valueTextBox.Text);
                expresionText.Add(valueTextBox.Text);
                expressionLabel.Text += valueTextBox.Text + " ";
            }
        }

        private void StudyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisableAdditionalParameters();
        }

        private void AddCondition_Click(object sender, EventArgs e)
        {
            expressionLabel.Text = ExpressionToPostfix(currentExpression);
            if (isClosingCondition)
            {
                gainStopCheckBox.Enabled = true;
                gainStopCheckBox.Checked = false;
                lossStopCheckBox.Enabled = true;
                lossStopCheckBox.Checked = false;
                posEffectComboBox.Enabled = true;
                addConditionButton.Enabled = false;
                conditionGeneratorGB.Enabled = false;
                addConditionButton.Text = "Add Opening Condition";
                closingCondition = new ClosingCondition(ExpressionToPostfix(currentExpression));
            }
            else
            {
                double? stopLoss = null;
                double? stopGain = null;
                if (lossStopCheckBox.Checked && lossStopTextBox.Text != "") stopLoss = Convert.ToDouble(lossStopTextBox.Text);
                if (gainStopCheckBox.Checked && gainStopTextBox.Text != "") stopGain = Convert.ToDouble(gainStopTextBox.Text);
                PositionType positionType = Enum.Parse<PositionType>(posEffectComboBox.Text);
                gainStopCheckBox.Enabled = false;
                gainStopCheckBox.Checked = false;
                lossStopCheckBox.Enabled = false;
                lossStopCheckBox.Checked = false;
                posEffectComboBox.Enabled = false;
                addConditionButton.Text = "Add Closing Condition";
                openingCondition = new OpeningCondition(ExpressionToPostfix(currentExpression), positionType, stopLoss, stopGain);
                computeButton.Enabled = true;
            }
            isClosingCondition = true;
            currentExpression.Clear();
            expresionText.Clear();
        }

        private void ComputeButton_Click(object sender, EventArgs e)
        {
            _ohlcData = new OhlcData("C:\\Users\\joseh\\PycharmProjects\\algoTrading1\\csv\\AAPL.csv");
            Strategy s = new(openingCondition, closingCondition, _ohlcData);
            for (int i = 0; i < s.Result.GainLoss.Length; i++)
            {
                resultsDataGridView.Rows.Add();
                resultsDataGridView.Rows[^1].Cells[0].Value = s.Result.OpeningIndices[i];
                resultsDataGridView.Rows[^1].Cells[1].Value = s.Result.ClosingIndices[i];
                resultsDataGridView.Rows[^1].Cells[2].Value = s.Result.GainLoss[i] * 100;
            }
        }

        private void StartOverButton_Click(object sender, EventArgs e)
        {
            expressionLabel.Text = "";
            currentExpression.Clear();
            expresionText.Clear();
            conditionGeneratorGB.Enabled = true;
            addConditionButton.Enabled = true;
            isClosingCondition = false;
            computeButton.Enabled = false;
        }

        private void DisableAdditionalParameters()
        {
            AvailableStudies selectedStudy = Enum.Parse<AvailableStudies>(studyComboBox.Text);
            lengthTextBox.Enabled = false;
            lengthTextBox.Text = "";
            marketDatatypeComboBox.Enabled = false;
            marketDatatypeComboBox.SelectedIndex = 0;
            stDevTextBox.Enabled = false;
            stDevTextBox.Text = "";
            atrsTextBox.Enabled = false;
            atrsTextBox.Text = "";
            movAvg1ComboBox.Enabled = false;
            movAvg1ComboBox.SelectedIndex = 0;
            movAvg2ComboBox.Enabled = false;
            movAvg2ComboBox.SelectedIndex = 0;
            specificStudyComboBox.Enabled = false;
            specificStudyComboBox.DataSource = null;
            switch (selectedStudy)
            {
                case AvailableStudies.AverageTrueRange:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "14";
                    movAvg1ComboBox.Enabled = true;
                    movAvg1ComboBox.SelectedIndex = 3;
                    break;
                case AvailableStudies.BollingerBand:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "20";
                    marketDatatypeComboBox.Enabled = true;
                    stDevTextBox.Enabled = true;
                    stDevTextBox.Text = "2.0";
                    movAvg1ComboBox.Enabled = true;
                    movAvg1ComboBox.SelectedIndex = 1;
                    specificStudyComboBox.Enabled = true;
                    specificStudyComboBox.DataSource = Enum.GetValues(typeof(BandsAvailableValues));
                    break;
                case AvailableStudies.DonchianChannel:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "20";
                    specificStudyComboBox.Enabled = true;
                    specificStudyComboBox.DataSource = Enum.GetValues(typeof(BandsAvailableValues));
                    break;
                case AvailableStudies.ExponentialMovingAverage:
                case AvailableStudies.SimpleMovingAverage:
                case AvailableStudies.WeigthedMovingAverage:
                case AvailableStudies.WildersMovingAverage:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "9";
                    marketDatatypeComboBox.Enabled = true;
                    break;
                case AvailableStudies.KeltnerChannel:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "20";
                    marketDatatypeComboBox.Enabled = true;
                    atrsTextBox.Enabled = true;
                    atrsTextBox.Text = "1.5";
                    movAvg1ComboBox.Enabled = true;
                    movAvg1ComboBox.SelectedIndex = 1;
                    movAvg2ComboBox.Enabled = true;
                    movAvg2ComboBox.SelectedIndex = 1;
                    specificStudyComboBox.Enabled = true;
                    specificStudyComboBox.DataSource = Enum.GetValues(typeof(BandsAvailableValues));
                    break;
                case AvailableStudies.LinearRegression:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "14";
                    marketDatatypeComboBox.Enabled = true;
                    specificStudyComboBox.Enabled = true;
                    specificStudyComboBox.DataSource = Enum.GetValues(typeof(LinearRegressionAvailableValues));
                    break;
                case AvailableStudies.RelativeStrengthIndex:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "14";
                    marketDatatypeComboBox.Enabled = true;
                    movAvg1ComboBox.Enabled = true;
                    movAvg1ComboBox.SelectedIndex = 3;
                    break;
                case AvailableStudies.StandardDeviation:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "14";
                    marketDatatypeComboBox.Enabled = true;
                    break;
                case AvailableStudies.TTMSqueeze:
                    lengthTextBox.Enabled = true;
                    lengthTextBox.Text = "20";
                    marketDatatypeComboBox.Enabled = true;
                    stDevTextBox.Enabled = true;
                    stDevTextBox.Text = "2.0";
                    atrsTextBox.Enabled = true;
                    atrsTextBox.Text = "1.5";
                    specificStudyComboBox.Enabled = true;
                    specificStudyComboBox.DataSource = Enum.GetValues(typeof(TTMSqueezeAvailableValues));
                    break;
            }
        }
        private string GetAdditionalParameters()
        {
            string parameters = "(";
            if (lengthTextBox.Enabled)
                parameters += lengthTextBox.Text + ',';
            if (marketDatatypeComboBox.Enabled)
                parameters += marketDatatypeComboBox.Text + ',';
            if (stDevTextBox.Enabled)
                parameters += stDevTextBox.Text + ',';
            if (atrsTextBox.Enabled)
                parameters += atrsTextBox.Text + ',';
            if (movAvg1ComboBox.Enabled)
                parameters += movAvg1ComboBox.Text + ',';
            if (movAvg2ComboBox.Enabled)
                parameters += movAvg2ComboBox.Text + ',';
            parameters += ')';
            if (parameters.Length > 2) // There is at least one parameter in the parentheses
                return parameters.Remove(parameters.Length - 2, 1); // Remove the last comma added
            return parameters;
        }

        private static string ExpressionToPostfix(List<string> expression)
        {
            Dictionary<char, int> precedenceList = new()
            {
                { ')', 0 },
                { '|', 1 },
                { '&', 3 },
                { '=', 5 },
                { '!', 5 },
                { '»', 7 },
                { '«', 7 },
                { '}', 7 },
                { '{', 7 },
                { '>', 7 },
                { '<', 7 },
                { '+', 9 },
                { '-', 9 },
                { '*', 11 },
                { '/', 11 },
                { '(', 13 }
            };
            Stack<KeyValuePair<char, int>> stack = new();
            string token;
            string result = "";
            expression.Add(")");
            int index = 0;
            stack.Push(new KeyValuePair<char, int>('(', 0));
            while (stack.Count > 0 && index < expression.Count)
            {
                token = expression[index++];
                // If token is an operand
                if (token.Length > 1 || char.IsDigit(token[0]))
                    result += token + ' ';
                // Token is an operator
                else
                {
                    while (stack.Count > 0 && stack.Peek().Value >= precedenceList[token[0]])
                    {
                        KeyValuePair<char, int> top = stack.Pop();
                        if (top.Key != '(')
                            result += $"{top.Key} ";
                    }
                    if (token[0] != ')')
                        stack.Push(new KeyValuePair<char, int>(token[0], (precedenceList[token[0]] + 1) % 14));
                }
            }
            return result;
        }
    }
}