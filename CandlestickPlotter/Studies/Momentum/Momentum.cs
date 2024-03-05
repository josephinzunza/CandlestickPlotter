using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Studies.Tools;
using CandleStickPlotter.Tools;

namespace CandleStickPlotter.Studies.Momentum
{

    public class RelativeStrenghtIndex : Study
    {
        public RelativeStrenghtIndex(int length = 14, MarketDataType marketDataType = MarketDataType.Close,
            MovingAverageType movingAverageType = MovingAverageType.WildersMovingAverage, double overSoldLevel = 30.0, double overBoughtLevel = 70.0)
        {
            Length = length;
            MarketDataType = marketDataType;
            MovingAverageType = movingAverageType;
            Oversold = overSoldLevel;
            Overbought = overBoughtLevel;
            RSI = new Column<double>();
            DefaultPlotLocation = PlotLocation.Lower;
            Plots =
            [
                new()
                {
                    Name = "RSI",
                    Type = Plot.PlotType.Line,
                    Values = RSI,
                    SubPlots =
                    [
                        new("RSI", true, Color.Black),
                        new("Oversold", Color.Cyan),
                        new("Overbought", Color.Red)
                    ],
                }
            ];
        }
        public int Length { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public Column<double> RSI { get; private set; }
        public double Oversold { get; set; }
        public double Overbought { get; set; }
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType)
        {
            MarketDataType = marketDataType;
            RSI = Calculate(CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, MarketDataType), Length, MovingAverageType);
            Plots[0].SubPlots[1].Indices = RSI < Oversold;
            Plots[0].SubPlots[2].Indices = RSI > Overbought;
        }
        public static Column<double> Calculate(Column<double> column, int length, MovingAverageType movingAverageType =
            MovingAverageType.WildersMovingAverage)
        {
            Column<double> diffColumn = column.Diff(1);
            Column<double> netChgAvg;
            Column<double> totChgAvg;
            if (movingAverageType == MovingAverageType.WildersMovingAverage)
            {
                netChgAvg = WildersMovingAverage.Calculate(diffColumn, length);
                totChgAvg = WildersMovingAverage.Calculate(diffColumn.Abs(), length);
            }
            else if (movingAverageType == MovingAverageType.ExponentialMovingAverage)
            {
                netChgAvg = ExponentialMovingAverage.Calculate(diffColumn, length);
                totChgAvg = ExponentialMovingAverage.Calculate(diffColumn.Abs(), length);
            }
            else if (movingAverageType == MovingAverageType.SimpleMovingAverage)
            {
                netChgAvg = SimpleMovingAverage.Calculate(diffColumn, length);
                totChgAvg = SimpleMovingAverage.Calculate(diffColumn.Abs(), length);
            }
            else
            {
                netChgAvg = WeightedMovingAverage.Calculate(diffColumn, length);
                totChgAvg = WeightedMovingAverage.Calculate(diffColumn.Abs(), length);
            }
            Column<double> chgRatio = netChgAvg.Div(totChgAvg, Column<double>.DivByZeroPolicy.SET_TO_ZERO);
            return (chgRatio + 1) * 50;
        }
        public override string NameString()
        {
            return $"RSI(Length={Length}, MarketDataType={MarketDataType}, MovingAverageType={MovingAverageType})";
        }
    }

    public class TTMSqueeze : Study
    {
        public TTMSqueeze(int length = 20, double ATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            Length = length;
            this.ATRs = ATRs;
            StDevs = stDevs;
            MarketDataType = marketDataType;
            Histogram = new Column<double>();
            Signal = new Column<byte>();
            DefaultPlotLocation = PlotLocation.Lower;
            Plots =
            [
                new Plot()
                {
                    Name = "Histogram",
                    Type = Plot.PlotType.Histogram,
                    Values = Histogram,
                    SubPlots =
                    [
                        new("Negative and Down", Color.Red),
                        new("Negative and Up", Color.Yellow),
                        new("Positive and Down", Color.Blue),
                        new("Positive and Up", Color.Cyan)
                    ],
                },
                new Plot()
                {
                    Name = "VolComp",
                    Type = Plot.PlotType.Dot,
                    Values = new Column<double>(Signal.Length, 0.0),
                    SubPlots =
                    [
                        new("Alert", Color.Green),
                        new("Normal", Color.Red)
                    ]
                }
            ];
        }
        public int Length { get; set; }
        public double ATRs { get; set; }
        public double StDevs { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public Column<byte> Signal { get; private set; }
        public Column<double> Histogram { get; private set; }
        // Used to supress CA1861 warning (for improved performance)
        private static readonly int[] ZeroIndex = [0];
        public override void Calculate(Table<double> table)
        {
            Calculate(table, MarketDataType);
        }
        public override void Calculate(Table<double> ohlcvData, MarketDataType marketDataType = MarketDataType.Close)
        {
            (Histogram, Signal) = Calculate(ohlcvData, Length, ATRs, StDevs, MarketDataType);
            Column<double> HistogramPrevious = new(Histogram);
            HistogramPrevious.Displace(1);
            int[] histLessThanHistPrev = Histogram < HistogramPrevious;
            Plots[0].SubPlots[0].Indices = (Histogram < 0).Intersect(histLessThanHistPrev).ToArray();
            if (Plots[0].SubPlots[0].Indices[0] == 1) // If this group has index 1 also put index 0 in there
                Plots[0].SubPlots[0].Indices = Plots[0].SubPlots[0].Indices.Union(ZeroIndex).ToArray();
            Plots[0].SubPlots[1].Indices = (Histogram < 0).Except(Plots[0].SubPlots[0].Indices).ToArray();
            if (Plots[0].SubPlots[1].Indices[0] == 1) // If this group has index 1 also put index 0 in there
                Plots[0].SubPlots[1].Indices = Plots[0].SubPlots[1].Indices.Union(ZeroIndex).ToArray();
            Plots[0].SubPlots[2].Indices = (Histogram > 0).Intersect(histLessThanHistPrev).ToArray();
            if (Plots[0].SubPlots[2].Indices[0] == 1) // If this group has index 1 also put index 0 in there
                Plots[0].SubPlots[2].Indices = Plots[0].SubPlots[2].Indices.Union(ZeroIndex).ToArray();
            Plots[0].SubPlots[3].Indices = (Histogram > 0).Except(Plots[0].SubPlots[2].Indices).ToArray();
            if (Plots[0].SubPlots[3].Indices[0] == 1) // If this group has index 1 also put index 0 in there
                Plots[0].SubPlots[3].Indices = Plots[0].SubPlots[3].Indices.Union(ZeroIndex).ToArray();

            Plots[1].SubPlots[0].Indices = Signal == 0;
            Plots[1].SubPlots[1].Indices = Signal == 1;
        }
        public static (Column<double>, Column<byte>) Calculate(Table<double> ohlcvData, int length,
            double nATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            //
            // Reminder: Add alert line parameter later on
            Column<double> column = CandleStickPlotter.Tools.Tools.GetMarketData(ohlcvData, marketDataType);
            Column<double> middleDonchianChannel = DonchianChannels.Calculate(ohlcvData, length)[1]; // Middle channel
            Column<double> temp = ExponentialMovingAverage.Calculate(column, length);
            temp = column - (temp + middleDonchianChannel) / 2;
            Column<double> histogram = LeastSquaresLinearRegression.Calculate(temp, length)[3] * 100; // Predicted value of Least Squares Regression
            Table<double> bollingerBands = BollingerBands.Calculate(column, length, stDevs);
            Table<double> keltnerChannels = KeltnerChannels.Calculate(ohlcvData, length, nATRs);
            Column<byte> signal = new("Signal", ohlcvData.RowCount);
            for (int i = 0; i < signal.Length; i++)
                // Lower Bollinger band is > lower Keltner channel AND upper Bollinger band is < upper Keltner channel
                signal[i] = Convert.ToByte(bollingerBands[0][i] > keltnerChannels[0][i] && bollingerBands[2][i] < keltnerChannels[2][i]);
            return (histogram, signal);
        }
        public override string NameString()
        {
            return $"TTM_Squeeze(MarketDataType={MarketDataType}, Length={Length}, ATRs={ATRs}, StDevs={StDevs})";
        }
    }

}
