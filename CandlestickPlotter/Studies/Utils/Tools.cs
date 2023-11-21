using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Tools;

namespace CandleStickPlotter.Studies.Tools
{
    public class LeastSquaresLinearRegression : Study
    {
        public LeastSquaresLinearRegression(int length)
        {
            Length = length;
            Slope = new Column<double>();
            Intercept = new Column<double>();
            R2 = new Column<double>();
            Predicted = new Column<double>();
            Plots = new Plot[4]
            {
            new Plot()
            {
                Name = "Slope",
                Type = Plot.PlotType.Line,
                Values = Slope,
                Visible = false,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("Slope", true, Color.Cyan)
                }
            },
            new Plot()
            {
                Name = "Intercept",
                Type = Plot.PlotType.Line,
                Values = Intercept,
                Visible = false,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("Intercept", true, Color.Cyan)
                }
            },
            new Plot()
            {
                Name = "R2",
                Type = Plot.PlotType.Line,
                Values = R2,
                Visible = false,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("R2", true, Color.Cyan)
                }
            },
            new Plot()
            {
                Name = "Predicted",
                Type = Plot.PlotType.Line,
                Values = Slope,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("Predicted", true, Color.Cyan)
                }
            },
            };
        }
        public int Length { get; set; }
        public Column<double> Slope { get; set; }
        public Column<double> Intercept { get; set; }
        public Column<double> R2 { get; set; }
        public Column<double> Predicted { get; set; }
        public MarketDataType MarketDataType { get; set; } = MarketDataType.Close;
        public override void Calculate(Column<double> column)
        {
            Table<double> result = Calculate(column, Length);
            Slope = result.Columns[0];
            Intercept = result.Columns[1];
            R2 = result.Columns[2];
            Predicted = result.Columns[3];
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType)
        {
            MarketDataType = marketDataType;
            Table<double> result = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length);
            Slope = result.Columns[0];
            Intercept = result.Columns[1];
            R2 = result.Columns[2];
            Predicted = result.Columns[3];
        }
        public static Table<double> Calculate(Column<double> column, int length)
        {
            Column<double> y = column;
            Table<double> result = new(new Column<double>[4]
            {
            new Column<double>("Slope", y.Length),
            new Column<double>("Intercept", y.Length),
            new Column<double>("R2", y.Length),
            new Column<double>("Predicted", y.Length)
            });
            length--;
            double xSum = length * (length + 1) / 2;
            double xSquaredSum = (length * (length + 1) * (2 * length + 1)) / 6;
            length++;
            int i = Column.CountLeadingNaNs(y);
            Table.SetLeadingNaNs(result, i + length - 1);
            int stop = i + length, j = 0;
            double ySquaredSum = 0;
            double xySum = 0;
            double ySum = 0;
            for (; i < stop; i++, j++)
            {

                xySum += j * y[i];
                ySum += y[i];
                ySquaredSum += y[i] * y[i];
            }
            double SSxy = length * xySum - xSum * ySum;
            double SSxx = length * xSquaredSum - xSum * xSum;
            double SSyy = length * ySquaredSum - ySum * ySum;
            result[0][i - 1] = SSxy / SSxx;
            result[1][i - 1] = (ySum - result[0][i - 1] * xSum) / length;
            result[2][i - 1] = SSxy * SSxy / (SSxx * SSyy);
            result[3][i - 1] = result[0][i - 1] * (length - 1) + result[1][i - 1];
            j = i - length;
            for (; i < y.Length; i++, j++)
            {
                xySum = xySum + y[i] * (length - 1) - ySum + y[j];
                ySum = ySum + y[i] - y[j];
                ySquaredSum = ySquaredSum + y[i] * y[i] - y[j] * y[j];
                SSxy = length * xySum - xSum * ySum;
                SSyy = length * ySquaredSum - ySum * ySum;
                result[0][i] = SSxy / SSxx;
                result[1][i] = (ySum - result[0][i] * xSum) / length;
                result[2][i] = SSxy * SSxy / (SSxx * SSyy);
                result[3][i] = result[0][i] * (length - 1) + result[1][i];
            }
            return result;
        }
        public override string NameString()
        {
            return $"LinearRegression(Displace={Displace}, Length={Length}, Price={MarketDataType})";
        }
    }
    public class StandardDeviation : Study
    {
        public StandardDeviation(int length, uint degreesOfFreedom = 1)
        {
            Length = length;
            DegreesOfFreedom = degreesOfFreedom;
            StDev = new Column<double>();
            Plots = new Plot[1]
            {
            new Plot()
            {
                Name = "StDev",
                Type = Plot.PlotType.Line,
                Values = StDev,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("StDev", Color.Cyan)
                }
            }
            };
        }
        public int Length { get; set; }
        public uint DegreesOfFreedom { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public Column<double> StDev { get; set; }
        public override void Calculate(Column<double> column)
        {
            StDev = Calculate(column, Length, DegreesOfFreedom);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType)
        {
            MarketDataType = marketDataType;
            StDev = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length, DegreesOfFreedom);
        }
        public static Column<double> Calculate(Column<double> column, int length, uint degreesOfFreedom = 0)
        {
            Column<double> result = new($"StDv{length}", column.Length);
            int start = 0;
            int stop = Column.CountLeadingNaNs(column);
            int i = start;
            for (; i < stop; i++)
                result[i] = double.NaN;
            stop += length - 1;
            double rollingSum = 0;
            for (; i < stop; i++)
            {
                rollingSum += column[i];
                result[i] = double.NaN;
            }
            double rollingMean;
            double sumOfSquares;
            int j;
            for (; i < column.Length; i++)
            {
                sumOfSquares = 0;
                rollingSum += column[i];
                rollingMean = rollingSum / length;
                for (j = i - length + 1; j <= i; j++)
                    sumOfSquares += Math.Pow(rollingMean - column[j], 2);
                result[i] = Math.Sqrt(sumOfSquares / (length - degreesOfFreedom));
                rollingSum -= column[i - length + 1];
            }
            return result;
        }
        public override string NameString()
        {
            return $"StandardDeviation(Price={MarketDataType}, Length={Length})";
        }
    }
    public class TrueRange : Study
    {
        public TrueRange()
        {
            Plots = new Plot[1]
            {
            new Plot()
            {
                Name = "TRI",
                Type = Plot.PlotType.Line,
                Values = TRI,
                SubPlots = new SubPlot[1]
                {
                    new SubPlot("TrueRangeIndicator", Color.Cyan)
                }
            }
            };
        }
        public Column<double> TRI { get; set; } = new Column<double>();
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            TRI = Calculate(ohlcvData);
        }
        public static double Calculate(double high, double low, double previousClose)
        {
            return Math.Max(high - low, Math.Max(high - previousClose, low - previousClose));
        }
        public static new Column<double> Calculate(Table<double> ohlcvData)
        {
            Column<double> result = new("TrueRange", ohlcvData.RowCount);
            result[0] = double.NaN;
            for (int i = 1; i < ohlcvData.RowCount; i++)
            {
                result[i] = Calculate(ohlcvData["High"][i], ohlcvData["Low"][i], ohlcvData["Close"][i - 1]);
            }
            return result;
        }
        public override string NameString()
        {
            return "TrueRangeIndicator";
        }
    }
}