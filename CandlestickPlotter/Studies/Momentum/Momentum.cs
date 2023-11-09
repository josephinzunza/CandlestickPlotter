using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using CandleStickPlotter.Utils;

namespace CandleStickPlotter.Studies.Momentum
{

    internal class RelativeStrenghtIndex : Study
    {
        public RelativeStrenghtIndex(int length, MarketDataType marketDataType = MarketDataType.Close,
            MovingAverageType movingAverageType = MovingAverageType.WildersMovingAverage)
        {
            Length = length;
            MarketDataType = marketDataType;
            MovingAverageType = movingAverageType;
            RSI = new Column<double>();
        }
        public int Length { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public Column<double> RSI { get; private set; }
        public override void Calculate(Column<double> column)
        {
            RSI = Calculate(column, Length, MovingAverageType);
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

    internal class TTMSqueeze : Study
    {
        public TTMSqueeze(int length, double ATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            Length = length;
            this.ATRs = ATRs;
            StDevs = stDevs;
            MarketDataType = marketDataType;
            Histogram = new Column<double>();
            Signal = new Column<byte>();
        }
        public int Length { get; set; }
        public double ATRs { get; set; }
        public double StDevs { get; set; }
        public MarketDataType MarketDataType { get; set; }
        public Column<byte> Signal { get; private set; }
        public Column<double> Histogram { get; private set; }
        public override void Calculate(Table<double> ohlcvData)
        {
            (Histogram, Signal) = Calculate(ohlcvData, Length, ATRs, StDevs, MarketDataType);
        }
        public static (Column<double>, Column<byte>) Calculate(Table<double> ohlcvData, int length,
            double nATRs = 1.5, double stDevs = 2.0, MarketDataType marketDataType = MarketDataType.Close)
        {
            //
            // Reminder: Add alert line parameter later on
            Column<double> column = Utils.Utils.GetMarketData(ohlcvData, marketDataType);
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
            return $"TTM_Squeeze(Length={Length}, ATRs={ATRs}, StDevs={StDevs}, MarketDataType={MarketDataType})";
        }
    }

}
