using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Utils;

namespace CandleStickPlotter.Studies.Momentum
{

    internal class RelativeStrenghtIndex
    {
        public RelativeStrenghtIndex(int length, MovingAverageType movingAverageType = MovingAverageType.WildersMovingAverage)
        {
            Length = length;
            MovingAverageType = movingAverageType;
        }
        public int Length { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length, MovingAverageType);
        }
        public static ColumnDouble Calculate(ColumnDouble column, int length, MovingAverageType movingAverageType =
            MovingAverageType.WildersMovingAverage)
        {
            ColumnDouble diffColumn = column.Diff(1);
            ColumnDouble netChgAvg;
            ColumnDouble totChgAvg;
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
            ColumnDouble chgRatio = netChgAvg.Div(totChgAvg, ColumnDouble.DivByZeroPolicy.SET_TO_ZERO);
            return (chgRatio + 1) * 50;
        }
    }

    internal class TTMSqueeze
    {
        public TTMSqueeze(int length, double ATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            Length = length;
            this.ATRs = ATRs;
            StDevs = stDevs;
            MarketDataType = marketDataType;
        }
        public int Length { get; set; }
        public double ATRs { get; set; }
        public double StDevs { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public TableDouble Calculate(OhlcData ohlcData)
        {
            return Calculate(ohlcData, Length, ATRs, StDevs, MarketDataType);
        }
        public static TableDouble Calculate(OhlcData ohlcData, int length,
            double nATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            //
            // Reminder: Add alert line parameter later on
            ColumnDouble column = Utils.Utils.GetMarketData(ohlcData, marketDataType);
            ColumnDouble middleDonchianChannel = DonchianChannels.Calculate(ohlcData, length)[1]; // Middle channel
            ColumnDouble temp = ExponentialMovingAverage.Calculate(column, length);
            temp = column - (temp + middleDonchianChannel) / 2;
            ColumnDouble histogram = LeastSquaresLinearRegression.Calculate(temp, length)[3] * 100; // Predicted value of Least Squares Regression
            TableDouble bollingerBands = BollingerBands.Calculate(column, length, stDevs);
            TableDouble keltnerChannels = KeltnerChannels.Calculate(ohlcData, length, nATRs);
            ColumnBool signal = new("Signal", ohlcData.Length);
            for (int i = 0; i < signal.Length; i++)
                // Lower Bollinger band is > lower Keltner channel AND upper Bollinger band is < upper Keltner channel
                signal[i] = bollingerBands[0][i] > keltnerChannels[0][i] && bollingerBands[2][i] < keltnerChannels[2][i];
            TableDouble result = new(new ColumnDouble[]
            {
                histogram
            })
            {
                BooleanColumns = new ColumnBool
                [] { signal }
            };
            return result;
        }
    }

}
