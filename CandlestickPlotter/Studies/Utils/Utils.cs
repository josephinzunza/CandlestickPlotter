using CandleStickPlotter.DataTypes;
using CandleStickPlotter.Strategies;
using CandleStickPlotter.Studies.Momentum;
using CandleStickPlotter.Studies.MovingAverages;
using CandleStickPlotter.Studies.Volatility;
using System.Globalization;

namespace CandleStickPlotter.Utils
{
    public enum MarketDataType : byte { Close, High, Low, Open, HL2, HLC3, OHLC4, Volume }


    public class LeastSquaresLinearRegression
    {
        public LeastSquaresLinearRegression(int length)
        {
            Length = length;
        }
        public int Length { get; set; }
        public Table<double> Calculate(Column<double> column)
        {
            return Calculate(column, Length);
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
    }
    public class StandardDeviation
    {
        public StandardDeviation(int length, uint degreesOfFreedom = 1)
        {
            Length = length;
            DegreesOfFreedom = degreesOfFreedom;
        }
        public int Length { get; set; }
        public uint DegreesOfFreedom { get; set; }
        public Column<double> Calculate(Column<double> column)
        {
            return Calculate(column, Length, DegreesOfFreedom);
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
    }
    public class TrueRange
    {
        public static double Calculate(double high, double low, double previousClose)
        {
            return Math.Max(high - low, Math.Max(high - previousClose, low - previousClose));
        }

        public static Column<double> Calculate(Table<double> ohlcData)
        {
            Column<double> result = new("TrueRange", ohlcData.RowCount);
            result[0] = double.NaN;
            for (int i = 1; i < ohlcData.RowCount; i++)
            {
                result[i] = Calculate(ohlcData["High"][i], ohlcData["Low"][i], ohlcData["Close"][i - 1]);
            }
            return result;
        }
    }
    public static class Utils
    {
        public static Column<double> GetMarketData(Table<double> ohlcData, MarketDataType dataType)
        {
            Column<double> data = dataType switch
            {
                MarketDataType.Close => ohlcData["Close"],
                MarketDataType.High => ohlcData["High"],
                MarketDataType.Low => ohlcData["Low"],
                MarketDataType.Open => ohlcData["Open"],
                MarketDataType.HL2 => (ohlcData["Low"] + ohlcData["High"]) / 2,
                MarketDataType.HLC3 => (ohlcData["Low"] + ohlcData["High"] + ohlcData["Close"]) / 3,
                MarketDataType.OHLC4 => (ohlcData["Open"] + ohlcData["High"] + ohlcData["Low"] + ohlcData["Close"]) / 4,
                MarketDataType.Volume => ohlcData["Volume"],
                _ => ohlcData["Close"]
            };
            return data;
        }
        public static Type GetSudyType(StudyType studyType)
        {
            return studyType switch
            {
                StudyType.AverageTrueRange => typeof(AverageTrueRange),
                StudyType.BollingerBands => typeof(BollingerBands),
                StudyType.DonchianChannel => typeof(DonchianChannels),
                StudyType.ExponentialMovingAverage => typeof(ExponentialMovingAverage),
                StudyType.KeltnerChannels => typeof(KeltnerChannels),
                StudyType.LinearRegression => typeof(LeastSquaresLinearRegression),
                StudyType.RelativeStrengthIndex => typeof(RelativeStrenghtIndex),
                StudyType.SimpleMovingAverage => typeof(SimpleMovingAverage),
                StudyType.StandardDeviation => typeof(StandardDeviation),
                StudyType.TrueRange => typeof(TrueRange),
                StudyType.TTMSqueeze => typeof(TTMSqueeze),
                StudyType.WeigthedMovingAverage => typeof(WeightedMovingAverage),
                StudyType.WildersMovingAverage => typeof(WildersMovingAverage),
                _ => typeof(object),
            };
        }

        public static (DateTime[], Table<double>, long[]) GetOHLCVTFromCSV(string filename)
        {
            using StreamReader reader = new(filename);
            string[] lines = reader.ReadToEnd().Split('\n');
            string[] vals = lines[0].Split(',');
            DateTime[] dates = new DateTime[lines.Length - 1];
            Column<double>[] ohlcv = new Column<double>[vals.Length - 1];
            long[] volume = new long[lines.Length - 1];
            int i = 0;
            for (; i < ohlcv.Length; i++)
                ohlcv[i] = new Column<double>(vals[i + 1], lines.Length - 1);
            i = 0;
            int stop = lines.Length - 1;
            for (; i < stop; i++)
            {
                vals = lines[i + 1].Split(',');
                dates[i] = DateTime.ParseExact(vals[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                for (int j = 1; j < vals.Length; j++)
                    ohlcv[j - 1][i] = Convert.ToDouble(vals[j]);
                volume[i] = Convert.ToInt64(vals[^1]);
            }
            return (dates, new Table<double>(ohlcv), volume);
        }
    }
}