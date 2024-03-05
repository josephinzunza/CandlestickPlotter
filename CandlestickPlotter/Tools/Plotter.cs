using CandleStickPlotter.Studies;
using CandleStickPlotter.DataTypes;
using System.Drawing.Drawing2D;
using Microsoft.VisualBasic.Logging;

namespace CandleStickPlotter.Tools
{
    public class PricePlotter
    {
        private bool isLogScale = true;
        private Color dojiColor = Color.Gray;
        private Color downtickColor = Color.FromArgb(255, 255, 54, 0);
        private Color gridColor = Color.Gray;
        private Color uptickColor = Color.FromArgb(255, 0, 191, 0);
        public bool IsLogScale
        {
            get { return isLogScale; }
            set
            {
                isLogScale = value;
                if (isLogScale)
                {
                    AdjustLog10Values();
                    PlotMax = MaxValueLog10;
                    PlotMin = MinValueLog10;
                    PlotData = OHLCDataLog10;
                }
                else
                {
                    PlotMax = MaxValue;
                    PlotMin = MinValue;
                    PlotData = OHLCVData;
                }
                PlotPictureBox.Refresh();
                SetLabelPrices();
            }
        }
        public Color DojiColor
        {
            get { return dojiColor; }
            set
            {
                dojiColor = value;
                DojiPen = new Pen(dojiColor);
            }
        }
        public Color GridColor
        {
            get { return gridColor; }
            set
            {
                gridColor = value;
                GridPen = new Pen(gridColor, 1) { DashPattern = [1, 9] };
            }
        }
        public Color DowntickColor
        {
            get { return downtickColor; }
            set
            {
                downtickColor = value;
                DowntickBrush = new SolidBrush(downtickColor);
            }
        }
        public Color UptickColor
        {
            get { return uptickColor; }
            set
            {
                uptickColor = value;
                UptickPen = new Pen(uptickColor);
            }
        }
        private Pen DojiPen { get; set; } = new(Color.Gray, 1);
        private SolidBrush DowntickBrush { get; set; } = new(Color.FromArgb(255, 255, 54, 0));
        private Pen DowntickPen { get; set; } = new(Color.FromArgb(255, 255, 54, 0), 1);
        private Pen GridPen { get; set; } = new(Color.Gray, 1) { DashPattern = [1, 9] };
        private Pen UptickPen { get; set; } = new(Color.FromArgb(255, 0, 191, 0), 1);
        private int CandlestickWidth { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        private double MinValueLog10 { get; set; }
        private double MaxValueLog10 { get; set; }
        private double PixelsPerDollar { get; set; }
        private double PlotMin { get; set; }
        private double PlotMax { get; set; }
        private double LogShift { get; set; }
        private int VerticalShift { get; set; } = 1;
        private int[] CandlestickSpacing { get; set; } = [];
        private int[] XPositions { get; set; }
        public Label[] PriceLabels { get; set; }
        public PictureBox PlotPictureBox { get; set; }
        private Deque<Study> Studies { get; set; } = new Deque<Study>();
        public Table<double> OHLCVData { get; set; }
        private Table<double> OHLCDataLog10 { get; set; }
        private Table<double> PlotData { get; set; }
        public PricePlotter(Table<double> ohlcvData, PictureBox pictureBox, Label[] priceLabels)
        {
            OHLCVData = ohlcvData;
            MinValue = OHLCVData[2].MinValue; // Lowest of Low
            MaxValue = OHLCVData[1].MaxValue; // Highest of High
            OHLCDataLog10 = new Table<double>([
                new Column<double>("Open", OHLCVData.RowCount),
                new Column<double>("High", OHLCVData.RowCount),
                new Column<double>("Low", OHLCVData.RowCount),
                new Column<double>("Close", OHLCVData.RowCount)
            ]);
            AdjustLog10Values();
            PlotMin = MinValueLog10;
            PlotMax = MaxValueLog10;
            PlotData = OHLCDataLog10;
            CandlestickSpacing = new int[OHLCVData.RowCount];
            PlotPictureBox = pictureBox;
            PlotPictureBox.Paint += PlotPictureBox_Paint;
            PriceLabels = priceLabels;
            XPositions = new int[OHLCVData.RowCount];
            SetLabelPrices();
        }
        public void AddStudy(Study study)
        {
            foreach (Plot plot in study.Plots)
            {
                MinValue = Math.Min(MinValue, plot.MinValue);
                MaxValue = Math.Max(MaxValue, plot.MaxValue);
            }
            if (IsLogScale)
            {
                AdjustLog10Values();
                PlotMax = MaxValueLog10;
                PlotMin = MinValueLog10;
                PlotData = OHLCDataLog10;
            }
            else
            {
                PlotMax = MaxValue;
                PlotMin = MinValue;
                PlotData = OHLCVData;
            }
            SetLabelPrices();
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
            if (IsLogScale)
            {
                AdjustLog10Values();
                PlotMax = MaxValueLog10;
                PlotMin = MinValueLog10;
                PlotData = OHLCDataLog10;
            }
            else
            {
                PlotMax = MaxValue;
                PlotMin = MinValue;
                PlotData = OHLCVData;
            }
            SetLabelPrices();
        }
        private void SetGridAndPriceLabels(Graphics g)
        {
            // Plot dotted lines and set the labels\
            double divSize = (PlotMax - PlotMin) * PixelsPerDollar / 10;
            double linePos = (PlotMax - PlotMin) * PixelsPerDollar + VerticalShift;
            for (int i = 0; i < 10; i++)
            {
                PriceLabels[i].Location = new Point(0, (int)linePos - PriceLabels[i].Height / 2);
                g.DrawLine(GridPen, 0, (float)linePos, PlotPictureBox.Width, (float)linePos);
                linePos -= divSize;
            }
            PriceLabels[10].Location = new Point(0, (int)linePos - PriceLabels[10].Height / 2 + PriceLabels[10].Height / 2);
            g.DrawLine(GridPen, 0, (float)linePos, PlotPictureBox.Width, (float)linePos);
        }
        private void PlotPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            PlotEverything(e.Graphics);
        }
        private void SetCandlestickSpacing(int extraSpace)
        {
            if (extraSpace == 0)
                Array.Fill(CandlestickSpacing, 1);
            int pattern = (OHLCVData.RowCount - 1) / extraSpace;
            for (int i = 0; i < OHLCVData.RowCount; i++)
            {
                if (extraSpace > 0 && i % pattern == 0)
                {
                    CandlestickSpacing[i] = 2;
                    extraSpace--;
                }
                else
                    CandlestickSpacing[i] = 1;
            }
        }
        private Column<double> ApplyLogShift(Column<double> column)
        {
            Column<double> result = new(column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = Math.Log10(column[i] + LogShift);
            return result;
        }
        private void AdjustLog10Values()
        {
            // If the minimum value is < 1 then shift it to 1
            if (MinValue < 0)
            {
                for (int i = 0; i < OHLCDataLog10.ColumnCount; i++)
                {
                    OHLCDataLog10[i] = new Column<double>(OHLCVData.RowCount);
                    for (int j = 0; j < OHLCVData.RowCount; j++)
                        OHLCDataLog10[i][j] = Math.Log10(OHLCVData[i][j] - MinValue + 1);
                    MinValueLog10 = 0;
                    MaxValueLog10 = Math.Log10(MaxValue - MinValue + 1);
                }
                LogShift = -MinValue + 1;
            }
            else if (MinValue < 1)
            {
                for (int i = 0; i < OHLCDataLog10.ColumnCount; i++)
                {
                    OHLCDataLog10[i] = new Column<double>(OHLCVData.RowCount);
                    for (int j = 0; j < OHLCVData.RowCount; j++)
                        OHLCDataLog10[i][j] = Math.Log10(OHLCVData[i][j] + 1);
                    MinValueLog10 = Math.Log10(MinValue + 1);
                    MaxValueLog10 = Math.Log10(MaxValue + 1);
                }
                LogShift = 1;
            }
            else
            {
                for (int i = 0; i < OHLCDataLog10.ColumnCount; i++)
                {
                    OHLCDataLog10[i] = new Column<double>(OHLCVData.RowCount);
                    for (int j = 0; j < OHLCVData.RowCount; j++)
                        OHLCDataLog10[i][j] = Math.Log10(OHLCVData[i][j]);
                    MinValueLog10 = Math.Log10(MinValue);
                    MaxValueLog10 = Math.Log10(MaxValue);
                    LogShift = 0;
                }
            }
        }
        public void PlotEverything(Graphics g)
        {
            PlotPrice(g);
            PlotStudies(g);
        }
        private void PlotPrice(Graphics g)
        {
            // Plot the candlesticks
            int xPos = 1;
            double yPos, low, high, height;
            float xMid;
            PixelsPerDollar = (PlotPictureBox.Height - VerticalShift * 2) / (PlotMax - PlotMin);
            SetGridAndPriceLabels(g);
            CandlestickWidth = (PlotPictureBox.Width - PlotData.RowCount - 1) / PlotData.RowCount;
            SetCandlestickSpacing((PlotPictureBox.Width - PlotData.RowCount - 1) % PlotData.RowCount);
            for (int i = 0; i < PlotData.RowCount; i++)
            {
                XPositions[i] = xPos;
                xMid = xPos + (CandlestickWidth >> 1);
                if (PlotData[0][i] < PlotData[3][i]) // Green candle
                {
                    height = (PlotData[3][i] - PlotData[0][i]) * PixelsPerDollar;
                    yPos = (PlotMax - PlotData[3][i]) * PixelsPerDollar + VerticalShift;
                    high = (PlotMax - PlotData[1][i]) * PixelsPerDollar + VerticalShift;
                    low = (PlotMax - PlotData[2][i]) * PixelsPerDollar + VerticalShift;
                    g.DrawRectangle(UptickPen, xPos, (float)yPos, CandlestickWidth - 1, (float)height);
                    g.DrawLine(UptickPen, xMid, (float)high, xMid, (float)yPos);
                    g.DrawLine(UptickPen, xMid, (float)(yPos + height), xMid, (float)low);
                }
                else if (PlotData[0][i] > PlotData[3][i])
                {
                    height = (PlotData[0][i] - PlotData[3][i]) * PixelsPerDollar;
                    yPos = (PlotMax - PlotData[0][i]) * PixelsPerDollar + VerticalShift;
                    high = (PlotMax - PlotData[1][i]) * PixelsPerDollar + VerticalShift;
                    low = (PlotMax - PlotData[2][i]) * PixelsPerDollar + VerticalShift;
                    g.FillRectangle(DowntickBrush, xPos, (float)yPos, CandlestickWidth, (float)height);
                    g.DrawLine(DowntickPen, xMid, (float)high, xMid, (float)yPos);
                    g.DrawLine(DowntickPen, xMid, (float)(yPos + height), xMid, (float)low);
                }
                else
                {
                    yPos = (PlotMax - PlotData[0][i]) * PixelsPerDollar + VerticalShift;
                    high = (PlotMax - PlotData[1][i]) * PixelsPerDollar + VerticalShift;
                    low = (PlotMax - PlotData[2][i]) * PixelsPerDollar + VerticalShift;
                    g.DrawLine(DojiPen, xPos, (float)yPos, xPos + CandlestickWidth, (float)yPos);
                    g.DrawLine(DojiPen, xMid, (float)low, xMid, (float)high);
                }
                xPos += CandlestickWidth + CandlestickSpacing[i];
            }
        }
        private void PlotStudies(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (Study study in Studies)
            {
                foreach (Plot plot in study.Plots)
                {
                    if (plot.Visible)
                        PlotSinglePlot(plot, g);
                }
            }
            g.SmoothingMode = SmoothingMode.Default;
        }
        private void PlotSinglePlot(Plot plot, Graphics g)
        {
            int i;
            // To avoid recalculating logs oever multiple iterations and cecking if it's a log-scale plot
            Column<double> tempValues = IsLogScale ? ApplyLogShift(plot.Values) : plot.Values;

            switch (plot.Type)
            {
                case Plot.PlotType.Line:
                    {
                        int x1;
                        double y1;
                        int x2;
                        double y2;
                        // To have each line start and end at the middle of the candlestick positions
                        int hShift = CandlestickWidth / 2;
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            Pen pen = new(subPlot.Color, 1);
                            x1 = XPositions[0] + hShift;
                            if (subPlot.PlotEveryIndex)
                            {
                                i = Column.CountNaNsAtBottom(tempValues);
                                y1 = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                for (; i < plot.Values.Length; i++)
                                {
                                    x2 = XPositions[i] + hShift;
                                    y2 = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                    g.DrawLine(pen, new PointF(x1, (float)y1), new PointF(x2, (float)y2));
                                    x1 = x2;
                                    y1 = y2;
                                }
                            }
                            else
                            {
                                i = subPlot.Indices[0] == 0 ? 1 : 0;
                                for (; i < subPlot.Indices.Length; i++)
                                {
                                    x1 = XPositions[subPlot.Indices[i] - 1] + hShift;
                                    x2 = XPositions[subPlot.Indices[i]] + hShift;
                                    y1 = (PlotMax - tempValues[subPlot.Indices[i] - 1]) * PixelsPerDollar + VerticalShift;
                                    y2 = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar + VerticalShift;
                                    g.DrawLine(pen, new PointF(x1, (float)y1), new PointF(x2, (float)y2));
                                }
                            }
                            pen.Dispose();
                        }
                        break;
                    }
                case Plot.PlotType.Histogram:
                    {
                        g.SmoothingMode = SmoothingMode.Default;
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            SolidBrush brush = new(subPlot.Color);
                            int histogramWidth = Math.Max(CandlestickWidth / 2, 1);
                            double yPos;
                            double height;
                            for (i = Column.CountNaNsAtBottom(tempValues); i < tempValues.Length; i++)
                            {
                                yPos = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                height = PlotPictureBox.Height - yPos - 1;
                                g.FillRectangle(brush, XPositions[i], (int)yPos, histogramWidth, (int)height);
                            }
                            brush.Dispose();
                        }
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        break;
                    }
                case Plot.PlotType.Dot:
                    {
                        double yPos;
                        float shiftToCenter = CandlestickWidth / 2;
                        int dotSize = CandlestickWidth + 1;
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            SolidBrush brush = new(subPlot.Color);
                            if (subPlot.PlotEveryIndex)
                            {
                                i = Column.CountNaNsAtBottom(tempValues);
                                for (; i < plot.Values.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[i]) * PixelsPerDollar - shiftToCenter + VerticalShift;
                                    g.FillEllipse(brush, XPositions[i] - 1, (int)yPos, dotSize, dotSize);

                                }
                            }
                            else
                            {
                                for (i = 0; i < subPlot.Indices.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar - shiftToCenter + VerticalShift;
                                    g.FillEllipse(brush, XPositions[subPlot.Indices[i]] - 1, (float)yPos, dotSize, dotSize);
                                }
                            }
                        }
                        break;
                    }
                case Plot.PlotType.Square:
                    {
                        g.SmoothingMode = SmoothingMode.Default;
                        double yPos;
                        float shiftToCenter = CandlestickWidth / 2;
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            SolidBrush brush = new(subPlot.Color);
                            if (subPlot.PlotEveryIndex)
                            {
                                for (i = 0; i < plot.Values.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[i]) * PixelsPerDollar - shiftToCenter + VerticalShift;
                                    g.FillRectangle(brush, XPositions[i], (float)yPos, CandlestickWidth, CandlestickWidth);
                                }
                            }
                            else
                            {
                                for (i = 0; i < subPlot.Indices.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar - shiftToCenter + VerticalShift;
                                    g.FillRectangle(brush, XPositions[subPlot.Indices[i]], (float)yPos, CandlestickWidth, CandlestickWidth);
                                }
                            }
                            brush.Dispose();
                        }
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        break;
                    }
                case Plot.PlotType.ArrowDown:
                    {
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            const int arrowWidth = 8;
                            const int halfArrowWidth = arrowWidth / 2;
                            const int arrowHeight = 7;
                            const int tailWidth = 2;
                            const int halfTailWidth = tailWidth / 2;
                            int xMiddleShift = CandlestickWidth / 2;
                            int xPos;
                            double yPos;
                            SolidBrush brush = new(subPlot.Color);
                            if (subPlot.PlotEveryIndex)
                            {
                                for (i = 0; i < plot.Values.Length; i++)
                                {
                                    xPos = XPositions[i] + xMiddleShift;
                                    yPos = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                    // Arrow width is used as both width and the height of half the arrow
                                    Point p1 = new(xPos, (int)yPos);
                                    yPos -= arrowHeight;
                                    Point p2 = new(xPos - halfArrowWidth, (int)yPos);
                                    Point p3 = new(xPos + halfArrowWidth, (int)yPos);
                                    g.FillPolygon(brush, new Point[] { p1, p2, p3 });
                                    g.FillRectangle(brush, xPos - halfTailWidth, (float)(yPos - arrowHeight - 1), tailWidth, arrowHeight);
                                }
                            }
                            else
                            {
                                for (i = 0; i < subPlot.Indices.Length; i++)
                                {
                                    xPos = XPositions[subPlot.Indices[i]] + xMiddleShift;
                                    yPos = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar + VerticalShift;
                                    // Arrow width is used as both width and the height of half the arrow
                                    Point p1 = new(xPos, (int)yPos);
                                    yPos -= arrowHeight;
                                    Point p2 = new(xPos - halfArrowWidth, (int)yPos);
                                    Point p3 = new(xPos + halfArrowWidth, (int)yPos);
                                    g.FillPolygon(brush, new Point[] { p1, p2, p3 });
                                    g.FillRectangle(brush, xPos - halfTailWidth, (float)(yPos - arrowHeight - 1), tailWidth, arrowHeight);
                                }
                            }
                            brush.Dispose();
                        }
                        break;
                    }
                case Plot.PlotType.ArrowUp:
                    {
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            const int arrowWidth = 8;
                            const int halfArrowWidth = arrowWidth / 2;
                            const int arrowHeight = 7;
                            const int tailWidth = 2;
                            const int halfTailWidth = tailWidth / 2;
                            int xMiddleShift = CandlestickWidth / 2;
                            int xPos;
                            double yPos;
                            SolidBrush brush = new(subPlot.Color);
                            if (subPlot.PlotEveryIndex)
                            {
                                for (i = 0; i < plot.Values.Length; i++)
                                {
                                    xPos = XPositions[i] + xMiddleShift;
                                    yPos = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                    // Arrow width is used as both width and the height of half the arrow
                                    Point p1 = new(xPos, (int)yPos);
                                    yPos += arrowHeight + 1;
                                    Point p2 = new(xPos - halfArrowWidth, (int)yPos);
                                    Point p3 = new(xPos + halfArrowWidth, (int)yPos);
                                    g.FillPolygon(brush, new Point[] { p1, p2, p3 });
                                    g.FillRectangle(brush, xPos - halfTailWidth, (float)(yPos - 1), tailWidth, arrowHeight);
                                }
                            }
                            else
                            {
                                for (i = 0; i < plot.Values.Length; i++)
                                {
                                    xPos = XPositions[subPlot.Indices[i]] + xMiddleShift;
                                    yPos = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar + VerticalShift;
                                    // Arrow width is used as both width and the height of half the arrow
                                    Point p1 = new(xPos, (int)yPos);
                                    yPos += arrowHeight + 1;
                                    Point p2 = new(xPos - halfArrowWidth, (int)yPos);
                                    Point p3 = new(xPos + halfArrowWidth, (int)yPos);
                                    g.FillPolygon(brush, new Point[] { p1, p2, p3 });
                                    g.FillRectangle(brush, xPos - halfTailWidth, (float)(yPos - 1), tailWidth, arrowHeight);
                                }
                            }
                            brush.Dispose();
                        }
                        break;
                    }
                case Plot.PlotType.Dash:
                    {
                        double yPos;
                        foreach (SubPlot subPlot in plot.SubPlots)
                        {
                            Pen pen = new(subPlot.Color);
                            if (subPlot.PlotEveryIndex)
                            {
                                for (i = Column.CountNaNsAtBottom(tempValues); i < tempValues.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[i]) * PixelsPerDollar + VerticalShift;
                                    g.DrawLine(pen, XPositions[i], (float)yPos, (XPositions[i] + CandlestickWidth), (float)yPos);
                                }
                            }
                            else
                            {
                                for (i = Column.CountNaNsAtBottom(tempValues); i < tempValues.Length; i++)
                                {
                                    yPos = (PlotMax - tempValues[subPlot.Indices[i]]) * PixelsPerDollar + VerticalShift;
                                    g.DrawLine(pen, XPositions[subPlot.Indices[i]], (float)yPos, (XPositions[subPlot.Indices[i]] + CandlestickWidth), (float)yPos);
                                }
                            }
                        }
                        break;
                    }
            }
        }
        private void SetLabelPrices()
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
}