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
        public Column<double> Calculate(Table<double> ohlcData)
        {
            return Calculate(ohlcData, Length, MovingAverageType);
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
    }
    public class DonchianChannels
    {
        public DonchianChannels(int length)
        {
            Length = length;
        }

        public int Length { get; set; }
        public Table<double> Calculate(Table<double> ohlcData)
        {
            return Calculate(ohlcData, Length);
        }

        public static Table<double> Calculate(Table<double> ohlcData, int length)
        {


            Column<double> lowerChannel = ohlcData["Low"].Min(length);
            Column<double> upperChannel = ohlcData["High"].Max(length);
            Column<double> middleChannel = (lowerChannel +  upperChannel) / 2;
            lowerChannel.Name = "Lower";
            middleChannel.Name = "Middle";
            upperChannel.Name = "Upper";
            Column<double>[] columns = { lowerChannel, middleChannel, upperChannel };
            return new Table<double>(columns);
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
        public Table<double> Calculate(Table<double> ohlcData)
        {
            return Calculate(ohlcData, Length, ATRs);
        }
        public static Table<double> Calculate(Table<double> ohlcData, int length, double atrs = 1.5, MovingAverageType averageType =
            MovingAverageType.SimpleMovingAverage, MovingAverageType trueRangeAverageType = MovingAverageType.SimpleMovingAverage,
            MarketDataType marketDataType = MarketDataType.Close)
        {
            Column<double> data = Utils.Utils.GetMarketData(ohlcData, marketDataType);
            Column<double> average = averageType switch
            {
                MovingAverageType.ExponentialMovingAverage => ExponentialMovingAverage.Calculate(data, length),
                MovingAverageType.SimpleMovingAverage => SimpleMovingAverage.Calculate(data, length),
                MovingAverageType.WildersMovingAverage => WildersMovingAverage.Calculate(data, length),
                MovingAverageType.WeightedMovingAverage => WeightedMovingAverage.Calculate(data, length),
                _ => SimpleMovingAverage.Calculate(data, length)
            }; ;
            Column<double> shift = atrs * AverageTrueRange.Calculate(ohlcData, length, trueRangeAverageType);
            Table<double> result = new(new Column<double>[3]
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
