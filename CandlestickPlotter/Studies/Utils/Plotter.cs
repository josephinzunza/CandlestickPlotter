using CandleStickPlotter.Studies;
using CandleStickPlotter.DataTypes;

namespace CandleStickPlotter.Tools
{
    public class PricePlotter
    {
        private const int VerticalPadding = 3;
        public bool IsLogScale { get; set; } = true;
        public double MinValue { get; set; } = 0.0;
        public double MaxValue { get; set; } = 0.0;
        public Label[] PriceLabels { get; set; }
        public PictureBox PlotPictureBox { get; set; }
        private Deque<Study> Studies { get; set; } = new Deque<Study>();
        public Table<double> OHLCVData { get; set; }
        public PricePlotter(Table<double> ohlcvData, PictureBox pictureBox, Label[] priceLabels)
        {
            OHLCVData = ohlcvData;
            PlotPictureBox = pictureBox;
            MinValue = OHLCVData[2].MinValue; // Lowest of Low
            MaxValue = OHLCVData[1].MaxValue; // Highest of High
            PlotPictureBox.Paint += PlotPictureBox_Paint;
            PriceLabels = priceLabels;
        }
        public void AddStudy(Study study)
        {
            MinValue = OHLCVData[2].MinValue; // Lowest of Low
            MaxValue = OHLCVData[1].MaxValue; // Highest of High
            foreach (Plot plot in study.Plots)
            {
                MinValue = Math.Min(MinValue, plot.MinValue);
                MaxValue = Math.Max(MaxValue, plot.MaxValue);
            }
            Studies.PushBack(study);
        }
        public void DemoteStudy(int index)
        {
            Studies.MoveDown(index);
        }
        public void PromoteStudy(int index)
        {
            Studies.MoveUp(index);
        }
        public void RemoveStudyAt(int index)
        {
            Studies.RemoveAt(index);
            foreach (Study study in Studies)
            {
                foreach (Plot plot in study.Plots)
                {
                    MinValue = Math.Min(MinValue, plot.MinValue);
                    MaxValue = Math.Max(MaxValue, plot.MaxValue);
                }
            }
        }
        private void PlotPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            Plot();
        }
        private int[] GetCandlestickSpacing(int extraSpace)
        {
            int[] spacing = new int[OHLCVData.RowCount];
            if (extraSpace == 0)
            {
                Array.Fill(spacing, 1);
                return spacing;
            }
            int pattern = (OHLCVData.RowCount - 1) / extraSpace;
            for (int i = 0;  i < OHLCVData.RowCount; i++)
            {
                if (extraSpace > 0 && i % pattern == 0)
                {
                    spacing[i] = 2;
                    extraSpace--;
                }
                else
                    spacing[i] = 1;
            }
            return spacing;
        } 
        public void Plot()
        {
            Bitmap bitmap = new(PlotPictureBox.Width, PlotPictureBox.Height);
            Graphics g = Graphics.FromImage(bitmap);
            PlotPrice(g);
            PlotStudies(g);
            PlotPictureBox.Image = bitmap;
        }
        private void PlotPrice(Graphics g)
        {
            Pen upTickPen = new(Color.Green, 1);
            Pen downTickPen = new(Color.Red, 1);
            SolidBrush downTickBrush = new(Color.Red);
            Pen dojiPen = new(Color.Gray, 1);
            Pen dottedPen = new(Color.Gray, 1)
            {
                DashPattern = new float[] { 1, 9 }
            };
            double tempMin = IsLogScale ? Math.Log10(MinValue) : MinValue;
            double tempMax = IsLogScale ? Math.Log10(MaxValue) : MaxValue;
            double pixelsPerDollar = (PlotPictureBox.Height - VerticalPadding * 2) / (tempMax - tempMin);

            // Plot dotted lines and set the labels
            double divSize = (tempMax - tempMin) * pixelsPerDollar / 10;
            double linePos = (tempMax - tempMin) * pixelsPerDollar + 1;
            int i = 0;
            for (; i < 10; i++)
            {
                PriceLabels[i].Location = new Point(0, (int)linePos - PriceLabels[i].Height / 2);
                g.DrawLine(dottedPen, 0, (float)linePos, PlotPictureBox.Width, (float)linePos);
                linePos -= divSize;
            }
            PriceLabels[10].Location = new Point(0, (int)linePos - PriceLabels[i].Height / 2 + PriceLabels[10].Height / 2);
            g.DrawLine(dottedPen, 0, (float)linePos, PlotPictureBox.Width, (float)linePos);

            // Plot the candlesticks
            int xPos = 1;
            double yPos, low, high, height;
            int width = (PlotPictureBox.Width - OHLCVData.RowCount - 1) / OHLCVData.RowCount;
            float xMid;
            int[] candlestickSpacing = GetCandlestickSpacing((PlotPictureBox.Width - OHLCVData.RowCount - 1) % OHLCVData.RowCount);
            for (i = 0; i < OHLCVData.RowCount; i++)
            {
                xMid = xPos + (width >> 1);
                if (OHLCVData[0][i] < OHLCVData[3][i]) // Green candle
                {
                    if (IsLogScale)
                    {
                        height = (Math.Log10(OHLCVData[3][i]) - Math.Log10(OHLCVData[0][i])) * pixelsPerDollar;
                        yPos = (tempMax - Math.Log10(OHLCVData[3][i])) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - Math.Log10(OHLCVData[1][i])) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - Math.Log10(OHLCVData[2][i])) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    else
                    {
                        height = (OHLCVData[3][i] - OHLCVData[0][i]) * pixelsPerDollar;
                        yPos = (tempMax - OHLCVData[3][i]) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - OHLCVData[1][i]) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - OHLCVData[2][i]) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    g.DrawRectangle(upTickPen, xPos, (float)yPos, width - 1, (float)height);
                    g.DrawLine(upTickPen, xMid, (float)high, xMid, (float)yPos);
                    g.DrawLine(upTickPen, xMid, (float)(yPos + height), xMid, (float)low);
                }
                else if (OHLCVData[0][i] > OHLCVData[3][i])
                {
                    if (IsLogScale)
                    {
                        height = (Math.Log10(OHLCVData[0][i]) - Math.Log10(OHLCVData[3][i])) * pixelsPerDollar;
                        yPos = (tempMax - Math.Log10(OHLCVData[0][i])) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - Math.Log10(OHLCVData[1][i])) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - Math.Log10(OHLCVData[2][i])) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    else
                    {
                        height = (OHLCVData[0][i] - OHLCVData[3][i]) * pixelsPerDollar;
                        yPos = (tempMax - OHLCVData[0][i]) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - OHLCVData[1][i]) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - OHLCVData[2][i]) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    g.FillRectangle(downTickBrush, xPos, (float)yPos, width, (float)height);
                    g.DrawLine(downTickPen, xMid, (float)high, xMid, (float)yPos);
                    g.DrawLine(downTickPen, xMid, (float)(yPos + height), xMid, (float)low);
                }
                else
                {
                    if (IsLogScale)
                    {
                        yPos = (tempMax - Math.Log10(OHLCVData[0][i])) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - Math.Log10(OHLCVData[1][i])) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - Math.Log10(OHLCVData[2][i])) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    else
                    {
                        yPos = (tempMax - OHLCVData[0][i]) * pixelsPerDollar + candlestickSpacing[i];
                        high = (tempMax - OHLCVData[1][i]) * pixelsPerDollar + candlestickSpacing[i];
                        low = (tempMax - OHLCVData[2][i]) * pixelsPerDollar + candlestickSpacing[i];
                    }
                    g.DrawLine(dojiPen, xPos, (float)yPos, xPos + width, (float)yPos);
                    g.DrawLine(downTickPen, xMid, (float)low, xMid, (float)high);
                }
                xPos += width + candlestickSpacing[i];
            }
        }
        private void PlotStudies(Graphics g)
        {

        }
        public void SetLabelPrices()
        {
            if (IsLogScale)
            {
                double priceIncrement = (Math.Log10(MaxValue) - Math.Log10(MinValue)) / 10;
                double currentPrice = Math.Log10(MinValue);
                for (int i = 0; i <= 10; i++)
                {
                    PriceLabels[i].Text = string.Format("{0:0.00}", Math.Pow(10, currentPrice));
                    currentPrice += priceIncrement;
                }
            }
            else
            {
                double priceIncrement = (MaxValue - MinValue) / 10;
                double currentPrice = MinValue;
                for (int i = 0; i <= 10; i++)
                {
                    PriceLabels[i].Text = string.Format("{0:0.00}", currentPrice);
                    currentPrice += priceIncrement;
                }
            }
        }
    }
    
    public class StudyPlotter
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public PictureBox PictureBox { get; set; }
        public StudyPlotter(List<Plot> studyPlots, PictureBox pictureBox)
        {
            PictureBox = pictureBox;
            PictureBox.Paint += PictureBox_Paint;
        }

        private void PictureBox_Paint(object? sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Plot()
        {

        }
    }
}
