using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Tools;

namespace CandleStickPlotter.Studies.Tools
{
    public class LeastSquaresLinearRegression : Study
    {
        public LeastSquaresLinearRegression(int length = 9, MarketDataType marketDataType = MarketDataType.Close, int displace = 0)
        {
            Length = length;
            MarketDataType = marketDataType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
            new()
            {
                Name = "Slope",
                Type = Plot.PlotType.Line,
                Visible = false,
                SubPlots =
                [
                    new("Slope", true, Color.Cyan)
                ]
            },
            new()
            {
                Name = "Intercept",
                Type = Plot.PlotType.Line,
                Visible = false,
                SubPlots =
                [
                    new("Intercept", true, Color.Cyan)
                ]
            },
            new()
            {
                Name = "R2",
                Type = Plot.PlotType.Line,
                Visible = false,
                SubPlots =
                [
                    new("R2", true, Color.Cyan)
                ]
            },
            new()
            {
                Name = "Predicted",
                Type = Plot.PlotType.Line,
                
                SubPlots =
                [
                    new("Predicted", true, Color.Cyan)
                ]
            },
            ];
        }
        public int Length { get; set; }
        public Column<double> Slope { get; set; } = new();
        public Column<double> Intercept { get; set; } = new();
        public Column<double> R2 { get; set; } = new();
        public Column<double> Predicted { get; set; } = new();
        public MarketDataType MarketDataType { get; set; } = MarketDataType.Close;
        public override void Calculate(Table<double> ohlcvData)
        {
            Column<double> column = CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType);
            Calculate(column);
        }
        public override void Calculate(Column<double> column)
        {
            Table<double> result = Calculate(column, Length);
            result.Displace(Displace);
            Slope = result[0];
            Intercept = result[1];
            R2 = result[2];
            Predicted = result[3];
            Plots[0].Values = Slope;
            Plots[1].Values = Intercept;
            Plots[2].Values = R2;
            Plots[3].Values = Predicted;
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType)
        {
            MarketDataType = marketDataType;
            Calculate(ohlcvData);
        }
        public static Table<double> Calculate(Column<double> column, int length)
        {
            Column<double> y = column;
            Table<double> result = new(
            [
                new("Slope", y.Length),
                new("Intercept", y.Length),
                new("R2", y.Length),
                new("Predicted", y.Length)
            ]);
            length--;
            double xSum = length * (length + 1) / 2;
            double xSquaredSum = (length * (length + 1) * (2 * length + 1)) / 6;
            length++;
            int i = Column.CountNaNsAtBottom(y);
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
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "MarketDataType", typeof(MarketDataType) },
            { "Displace", typeof(int) }
        };
        */
    }
    public class StandardDeviation : Study
    {
        public StandardDeviation(int length = 14, MarketDataType marketDataType = MarketDataType.Close, uint degreesOfFreedom = 0)
        {
            Length = length;
            MarketDataType = marketDataType;
            DegreesOfFreedom = degreesOfFreedom;
            DefaultPlotLocation = PlotLocation.Lower;
            Plots =
            [
            new()
            {
                Name = "StDev",
                Type = Plot.PlotType.Line,
                
                SubPlots =
                [
                    new("StDev", Color.Cyan)
                ]
            }
            ];
        }
        public int Length { get; set; }
        public uint DegreesOfFreedom { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public Column<double> StDev { get; set; } = new();
        public override void Calculate(Table<double> ohlcvData)
        {
            Column<double> column = CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType);
            Calculate(column);
        }
        public override void Calculate(Column<double> column)
        {
            StDev = Calculate(column, Length, DegreesOfFreedom);
            Plots[0].Values = StDev;
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType)
        {
            MarketDataType = marketDataType;
            Calculate(ohlcvData);
        }
        public static Column<double> Calculate(Column<double> column, int length, uint degreesOfFreedom = 0)
        {
            Column<double> result = new($"StDv{length}", column.Length);
            int start = 0;
            int stop = Column.CountNaNsAtBottom(column);
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
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "MarketDataType", typeof(MarketDataType) }
        };
        */
    }
    public class TrueRange : Study
    {
        public TrueRange()
        {
            DefaultPlotLocation = PlotLocation.Lower;
            Plots =
            [
            new()
            {
                Name = "TRI",
                Type = Plot.PlotType.Line,
                SubPlots =
                [
                    new("TrueRangeIndicator", Color.Cyan)
                ]
            }
            ];
        }
        public Column<double> TRI { get; set; } = new();
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            TRI = Calculate(ohlcvData);
            Plots[0].Values = TRI;
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
            return "TrueRangeIndicator()";
        }
    }
}