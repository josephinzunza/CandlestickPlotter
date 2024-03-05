using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Tools;

namespace CandleStickPlotter.Studies.MovingAverages
{
    public enum MovingAverageType : byte
    {
        ExponentialMovingAverage, SimpleMovingAverage, WeightedMovingAverage, WildersMovingAverage
    }
    public class ExponentialMovingAverage : Study
    {
        public ExponentialMovingAverage(int length = 9, MarketDataType marketDataType = MarketDataType.Close, int displace = 0)
        {
            Length = length;
            SmoothingFactor = 2.0 / (length + 1);
            EMA = new Column<double>();
            MarketDataType = marketDataType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new Plot()
                {
                    Name = "EMA",
                    Type = Plot.PlotType.Line,
                    Values = new Column<double>(),
                    SubPlots =
                    [
                        new SubPlot("EMA", true, Color.Cyan)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public double SmoothingFactor { get; set; }
        public Column<double> EMA { get; private set; }
        MarketDataType MarketDataType { get; set; }
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            MarketDataType = marketDataType;
            EMA = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length);
            EMA.Displace(Displace);
            Plots[0].Values = EMA;
        }
        public static Column<double> Calculate(Column<double> column, int length)
        {
            double smoothingFactor = 2.0 / (length + 1);
            return Calculate(column, length, smoothingFactor);
        }
        public static Column<double> Calculate(Column<double> column, int length, double smoothingFactor)
        {
            Column<double> result = new($"EMA{length}", column.Length);

            int i = 0, leadingNaNs = Column.CountNaNsAtBottom(column);
            for (; i < leadingNaNs; i++)
                result[i] = double.NaN;
            result[i] = column[i];
            i++;
            for (; i < column.Length; i++)
                result[i] = (column[i] - result[i - 1]) * smoothingFactor + result[i - 1];
            return result;
        }
        public override string NameString()
        {
            return $"ExponentialMovingAverage(MarketDataType={MarketDataType}, Length={Length}, Displace={Displace})";
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
    public class SimpleMovingAverage : Study
    {
        public SimpleMovingAverage(int length = 9, MarketDataType marketDataType = MarketDataType.Close, int displace = 0)
        {
            Length = length;
            SMA = new Column<double>();
            MarketDataType = marketDataType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new()
                {
                    Name = "SMA",
                    Type = Plot.PlotType.Line,
                    Values = new Column<double>(),
                    SubPlots =
                    [
                        new("SMA", true, Color.Cyan)
                    ]
                }
            ];
        }

        public int Length { get; set; }
        public Column<double> SMA { get; private set; }
        MarketDataType MarketDataType { get; set; }
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            MarketDataType = marketDataType;
            SMA = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length);
            SMA.Displace(Displace);
            Plots[0].Values = SMA;
        }
        public static Column<double> Calculate(Column<double> column, int length)
        {
            Column<double> result = new($"SMA{length}", column.Length);
            double sum = 0;
            int i = 0, stop = Column.CountNaNsAtBottom(column);
            for (; i < stop; i++)
                result[i] = double.NaN;
            stop += length - 1;
            for (; i < stop; i++)
            {
                result[i] = double.NaN;
                sum += column[i];
            }
            for (; i < result.Length; i++)
            {
                sum += column[i];
                result[i] = sum / length;
                sum -= column[i - length + 1];
            }
            return result;
        }
        public override string NameString()
        {
            return $"SimpleMovingAverage(MarketDataType={MarketDataType}, Length={Length}, Displace={Displace})";
        }
    }
    public class WeightedMovingAverage : Study
    {
        public WeightedMovingAverage(int length = 9, MarketDataType marketDataType = MarketDataType.Close, int displace = 0)
        {
            Length = length;
            SmoothingFactor = 2.0 / (length + 1);
            WMA = new Column<double>();
            MarketDataType = marketDataType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new()
                {
                    Name = "WMA",
                    Type = Plot.PlotType.Line,
                    Values = new Column<double>(),
                    SubPlots =
                    [
                        new("WMA", true,Color.Cyan)
                    ]
                }
            ];
        }

        public int Length { get; set; }
        public double SmoothingFactor { get; set; }
        public Column<double> WMA { get; private set; }
        MarketDataType MarketDataType { get; set; }
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            MarketDataType = marketDataType;
            WMA = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length);
            WMA.Displace(Displace);
            Plots[0].Values = WMA;
        }
        public static Column<double> Calculate(Column<double> column, int length)
        {
            Column<double> result = new ($"WMA{length}", column.Length);
            double divisor = length * (length + 1) / 2.0;
            double rollingSum = 0;
            double weightedSum = 0;
            int i = 0, stop = Column.CountNaNsAtBottom(column);
            for (; i < stop; i++)
                result[i] = double.NaN;
            stop += length;
            int trueIndex = 1;
            for (; i < stop; i++, trueIndex++)
            {
                result[i] = double.NaN;
                weightedSum += column[i] * trueIndex;
                rollingSum += column[i];
            }
            // Change the previous value since that index has enough historic information to calculate the first value of the WMA
            result[i - 1] = weightedSum / divisor;
            for (; i < column.Length; i++)
            {
                weightedSum = weightedSum - rollingSum + column[i] * length;
                rollingSum = rollingSum - column[i - length] + column[i];
                result[i] = weightedSum / divisor;
            }
            return result;
        }
        public override string NameString()
        {
            return $"WeightedMovingAverage(MarketDataType={MarketDataType}, Length={Length}, Displace={Displace})";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "MarketDataType", typeof(MarketDataType) },
            { "Displace", typeof(int) }
        };*/
    }
    public class WildersMovingAverage : Study
    {
        public WildersMovingAverage(int length = 14, MarketDataType marketDataType = MarketDataType.Close, int displace = 0)
        {
            Length = length;
            SmoothingFactor = 2.0 / (length + 1);
            WMA = new Column<double>();
            MarketDataType = marketDataType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new()
                {
                    Name = "WMA",
                    Type = Plot.PlotType.Line,
                    Values = new Column<double>(),
                    SubPlots =
                    [
                        new("WMA", true, Color.Cyan)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public double SmoothingFactor { get; set; }
        public Column<double> WMA { get; private set; }
        MarketDataType MarketDataType { get; set; }
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            MarketDataType = marketDataType;
            WMA = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length);
            WMA.Displace(Displace);
            Plots[0].Values = WMA;
        }
        public static Column<double> Calculate(Column<double> column, int length)
        {
            double smoothingFactor = 1.0 / length;
            Column<double> result = new($"WildersMA{length}", column.Length);
            int i = 0, stop = Column.CountNaNsAtBottom(column);
            double sum = 0;
            for (; i < stop; i++)
                result[i] = double.NaN;
            stop += length;
            for (; i < stop; i++)
            {
                result[i] = double.NaN;
                sum += column[i];
            }
            result[i - 1] = sum / length;
            for (; i < column.Length; i++)
                result[i] = (column[i] - result[i - 1]) * smoothingFactor + result[i - 1];
            return result;
        }
        public override string NameString()
        {
            return $"WildersMovingAverage(MarketDataType={MarketDataType}, Length={Length}, Displace={Displace})";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "MarketDataType", typeof(MarketDataType) },
            { "Displace", typeof(int) }
        };*/
    }
}
