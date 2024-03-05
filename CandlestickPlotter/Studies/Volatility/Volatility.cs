using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Tools;
using CandleStickPlotter.Studies.Tools;

namespace CandleStickPlotter.Studies.Volatility
{
    public class AverageTrueRange : Study
    {
        public AverageTrueRange(int length = 14, MovingAverageType movingAverageType = MovingAverageType.WildersMovingAverage)
        {
            Length = length;
            MovingAverageType = movingAverageType;
            DefaultPlotLocation = PlotLocation.Lower;
            Plots =
            [
                new Plot()
                {
                    Name = "ATR",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot("ATR", true, Color.Cyan)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public Column<double> ATR { get; private set; } = new();
        public override void Calculate(Table<double> ohlcvData)
        {
            ATR = Calculate(ohlcvData, Length, MovingAverageType);
        }
        public static Column<double> Calculate(Table<double> data, int length, MovingAverageType movieAverageType = MovingAverageType.WildersMovingAverage)
        {
            Column<double> result = TrueRange.Calculate(data);
            result = movieAverageType switch
            {
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(result, length),
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(result, length),
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(result, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(result, length),
                _ => WildersMovingAverage.Calculate(result, length),
            };
            result.Name = $"ATR{length}";
            return result;
        }
        public override string NameString()
        {
            return $"ATR(Length={Length}, MovingAverageType={MovingAverageType})";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "MovingAverageType", typeof(MovingAverageType) }
        };*/
    }
    public class BollingerBands : Study
    {
        public BollingerBands(int length = 20, double stdDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close,
            MovingAverageType movingAverageType = MovingAverageType.SimpleMovingAverage, int displace = 0)
        {
            Length = length;
            StDevs = stdDevs;
            MarketDataType = marketDataType;
            MovingAverageType = movingAverageType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new Plot()
                {
                    Name = "LowerBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true, Color.Fuchsia)
                    ]
                },
                new Plot()
                {
                    Name = "MiddleBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true)
                    ]
                },
                new Plot()
                {
                    Name = "UpperBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true, Color.Red)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public double StDevs { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public Column<double> LowerBand { get; private set; } = new();
        public Column<double> MiddleBand { get; private set; } = new();
        public Column<double> UpperBand { get; private set; } = new();
        public override void Calculate(Table<double> ohlcvData)
        {
            Column<double> column = CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType);
            Calculate(column);
        }
        public override void Calculate(Column<double> column)
        {
            Table<double> result = Calculate(column, Length, StDevs, MovingAverageType);
            result.Displace(Displace);
            LowerBand = result[0];
            MiddleBand = result[1];
            UpperBand = result[2];
            Plots[0].Values = LowerBand;
            Plots[1].Values = MiddleBand;
            Plots[2].Values = UpperBand;
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            MarketDataType = marketDataType;
            Calculate(ohlcvData);
        }
        public static Table<double> Calculate(Column<double> column, int length, double stDevs = 2.0, MovingAverageType averageType = MovingAverageType.SimpleMovingAverage)
        {
            Column<double>[] columns = new Column<double>[3];
            Column<double> movingAverage = averageType switch
            {
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(column, length),
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(column, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(column, length),
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(column, length),
                _ => SimpleMovingAverage.Calculate(column, length),
            };
            Column<double> shift = StandardDeviation.Calculate(column, length
                ) * stDevs;
            columns[1] = movingAverage;
            columns[0] = movingAverage - shift;
            columns[2] = movingAverage + shift;
            columns[0].Name = "Lower";
            columns[1].Name = "Middle";
            columns[2].Name = "Upper";
            return new Table<double>(columns);
        }
        public override string NameString()
        {
            return $"BollingerBands(Length={Length}, StDevs={StDevs}, MarketDataType={MarketDataType}, Displace={Displace}, , MovingAverageType={MovingAverageType})";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "StDevs", typeof(double) },
            { "MarketDataType", typeof(MarketDataType) },
            { "MovingAverageType", typeof(MovingAverageType) },
            { "Displace", typeof(int) }
        };*/
    }
    public class DonchianChannels : Study
    {
        public DonchianChannels(int length = 20)
        {
            Length = length;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new Plot()
                {
                    Name = "LowerBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true)
                    ]
                },
                new Plot()
                {
                    Name = "MiddleBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true, Color.Yellow)
                    ]
                },
                new Plot()
                {
                    Name = "UpperBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true)
                    ]
                }
            ];
        }
        public Column<double> LowerBand { get; set; } = new();
        public Column<double> MiddleBand { get; set; } = new();
        public Column<double> UpperBand { get; set; } = new();
        public int Length { get; set; }
        public override void Calculate(Table<double> ohlcvData)
        {
            Table<double> result = Calculate(ohlcvData, Length);
            LowerBand = result[0];
            MiddleBand = result[1];
            UpperBand = result[2];
            Plots[0].Values = LowerBand;
            Plots[1].Values = MiddleBand;
            Plots[2].Values = UpperBand;
        }
        public static Table<double> Calculate(Table<double> ohlcData, int length)
        {
            Column<double> lowerChannel = ohlcData["Low"].Min(length);
            Column<double> upperChannel = ohlcData["High"].Max(length);
            Column<double> middleChannel = (lowerChannel + upperChannel) / 2;
            lowerChannel.Name = "Lower";
            middleChannel.Name = "Middle";
            upperChannel.Name = "Upper";
            Column<double>[] columns = [lowerChannel, middleChannel, upperChannel];
            return new Table<double>(columns);
        }
        public override string NameString()
        {
            return $"DonchianChannels(Length={Length})";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) }
        };
        */
    }

    public class KeltnerChannels : Study
    {
        public KeltnerChannels(int length = 20, double aTRs = 1.5, MarketDataType marketDataType = MarketDataType.Close,
            MovingAverageType averageType = MovingAverageType.SimpleMovingAverage,
            MovingAverageType trueRangeAverageType = MovingAverageType.SimpleMovingAverage, int displace = 0)
        {
            Length = length;
            ATRs = aTRs;
            MarketDataType = marketDataType;
            MovingAverageType = averageType;
            TrueRangeAverageType = trueRangeAverageType;
            Displace = displace;
            DefaultPlotLocation = PlotLocation.Price;
            Plots =
            [
                new Plot()
                {
                    Name = "LowerBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true, Color.IndianRed)
                    ]
                },
                new Plot()
                {
                    Name = "Average",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true, Color.LightYellow)
                    ]
                },
                new Plot()
                {
                    Name = "UpperBand",
                    Type = Plot.PlotType.Line,
                    SubPlots =
                    [
                        new SubPlot(true)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public double ATRs { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public MovingAverageType TrueRangeAverageType { get; set; }
        public Column<double> LowerBand { get; set; } = new();
        public Column<double> Average {  get; set; } = new();
        public Column<double> UpperBand { get; set; } = new();
        public override void Calculate(Table<double> ohlcvData)
        {
            Table<double> result = Calculate(ohlcvData, Length, ATRs, MarketDataType, MovingAverageType, TrueRangeAverageType);
            result.Displace(Displace);
            LowerBand = result[0];
            Average = result[1];
            UpperBand = result[2];
            Plots[0].Values = LowerBand;
            Plots[1].Values = Average;
            Plots[2].Values = UpperBand;
        }
        public static Table<double> Calculate(Table<double> ohlcvData, int length, double atrs = 1.5, MarketDataType marketDataType = MarketDataType.Close,
            MovingAverageType averageType = MovingAverageType.SimpleMovingAverage,
            MovingAverageType trueRangeAverageType = MovingAverageType.SimpleMovingAverage)
        {
            Column<double> data = CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, marketDataType);
            Column<double> average = averageType switch
            {
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(data, length),
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(data, length),
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(data, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(data, length),
                _ => SimpleMovingAverage.Calculate(data, length)
            }; ;
            Column<double> shift = atrs * AverageTrueRange.Calculate(ohlcvData, length, trueRangeAverageType);
            Table<double> result = new(
            [
                average - shift,
                average,
                average + shift
            ]);
            result[0].Name = "Lower";
            result[1].Name = "Middle";
            result[2].Name = "Upper";
            return result;
        }
        public override string NameString()
        {
            return $"KeltnerChannels(Displace={Displace}, ATRs={ATRs}, Length={Length}, Price={MarketDataType}";
        }
        /*
        public override Dictionary<string, Type> Parameters => new()
        {
            { "Length", typeof(int) },
            { "ATRs", typeof(double) },
            { "MarketDataType", typeof(MarketDataType) },
            { "MovingAverageType", typeof(MovingAverageType) },
            { "TrueRangeAverageType", typeof(MovingAverageType) },
            { "Displace", typeof(int) }
        };
        */
    }
}
