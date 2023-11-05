using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Utils;

namespace CandleStickPlotter.Studies.Volatility
{
    public class AverageTrueRange
    {
        public AverageTrueRange(int length, MovingAverageType movingAverageType)
        {
            Length = length;
            MovingAverageType = movingAverageType;
        }
        public int Length { get; set; }
        public MovingAverageType MovingAverageType { get; set; }
        public ColumnDouble Calculate(OhlcData ohlcData)
        {
            return Calculate(ohlcData, Length, MovingAverageType);
        }
        public static ColumnDouble Calculate(OhlcData data, int length, MovingAverageType movieAverageType = MovingAverageType.WildersMovingAverage)
        {
            ColumnDouble result = TrueRange.Calculate(data);
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
    }
    public class BollingerBands
    {
        public BollingerBands(int length, double stdDevs)
        {
            Length = length;
            StDevs = stdDevs;
        }
        public int Length { get; set; }
        public double StDevs { get; set; }

        public static TableDouble Calculate(ColumnDouble column, int length, double stDevs = 2.0, MovingAverageType averageType = MovingAverageType.SimpleMovingAverage)
        {
            ColumnDouble[] columns = new ColumnDouble[3];
            ColumnDouble movingAverage = averageType switch
            {
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(column, length),
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(column, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(column, length),
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(column, length),
                _ => SimpleMovingAverage.Calculate(column, length),
            };
            ColumnDouble shift = StandardDeviation.Calculate(column, length
                ) * stDevs;
            columns[1] = movingAverage;
            columns[0] = movingAverage - shift;
            columns[2] = movingAverage + shift;
            columns[0].Name = "Lower";
            columns[1].Name = "Middle";
            columns[2].Name = "Upper";
            return new TableDouble(columns);
        }
    }
    public class DonchianChannels
    {
        public DonchianChannels(int length)
        {
            Length = length;
        }

        public int Length { get; set; }
        public TableDouble Calculate(OhlcData ohlcData)
        {
            return Calculate(ohlcData, Length);
        }

        public static TableDouble Calculate(OhlcData ohlcData, int length)
        {


            ColumnDouble lowerChannel = ohlcData.Low.Min(length);
            ColumnDouble upperChannel = ohlcData.High.Max(length);
            ColumnDouble middleChannel = (lowerChannel +  upperChannel) / 2;
            lowerChannel.Name = "Lower";
            middleChannel.Name = "Middle";
            upperChannel.Name = "Upper";
            ColumnDouble[] columns = { lowerChannel, middleChannel, upperChannel };
            return new TableDouble(columns);
        }
    }

    public class KeltnerChannels
    {
        public KeltnerChannels(int length, double aTRs)
        {
            Length = length;
            ATRs = aTRs;
        }
        public int Length { get; set; }
        public double ATRs { get; set; }
        public TableDouble Calculate(OhlcData ohlcData)
        {
            return Calculate(ohlcData, Length, ATRs);
        }
        public static TableDouble Calculate(OhlcData ohlcData, int length, double atrs = 1.5, MovingAverageType averageType =
            MovingAverageType.SimpleMovingAverage, MovingAverageType trueRangeAverageType = MovingAverageType.SimpleMovingAverage,
            MarketDataType marketDataType = MarketDataType.Close)
        {
            ColumnDouble data = Utils.Utils.GetMarketData(ohlcData, marketDataType);
            ColumnDouble average = averageType switch
            {
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(data, length),
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(data, length),
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(data, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(data, length),
                _ => SimpleMovingAverage.Calculate(data, length)
            }; ;
            ColumnDouble shift = atrs * AverageTrueRange.Calculate(ohlcData, length, trueRangeAverageType);
            TableDouble result = new(new ColumnDouble[3]
            {
                average - shift,
                average,
                average + shift
            });
            result[0].Name = "Lower";
            result[1].Name = "Middle";
            result[2].Name = "Upper";
            return result;
        }
    }

}
