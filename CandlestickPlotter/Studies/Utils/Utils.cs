using CandleStickPlotter.DataTypes;

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
        public TableDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length);
        }
        public static TableDouble Calculate(ColumnDouble column, int length)
        {
            ColumnDouble y = column;
            TableDouble result = new(new ColumnDouble[4]
            {
                new ColumnDouble("Slope", y.Length),
                new ColumnDouble("Intercept", y.Length),                
                new ColumnDouble("R2", y.Length),
                new ColumnDouble("Predicted", y.Length)
            });
            length--;
            double xSum = length * (length + 1) / 2;
            double xSquaredSum = (length * (length + 1) * (2 * length + 1)) / 6;
            length++;
            int i = y.CountLeadingNaNs();
            result.SetLeadingNaNs(i + length - 1);
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
    /*
    public class LinearRegression
    {
        public LinearRegression(int length)
        {
            Length = length;
        }

        public int Length { get; set; }

        public static TableDouble Calculate(ColumnDouble column, int length)
        {
            string filename = "C:\\Users\\joseh\\source\\repos\\CandlestickPlotter\\CandlestickPlotter\\Scripts\\temp.csv";
            column.ToCSV(filename);
            Process process = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = @"C:\Users\joseh\AppData\Local\Programs\Python\Python39\python.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    Arguments = $"{@"C:\Users\joseh\source\repos\CandleStickPlotter\CandleStickPlotter\Scripts\linreg.py"} {length}"
                }
            };
            process.Start();
            process.WaitForExit();
            MessageBox.Show(process.StandardError.ReadToEnd());
            return TableDouble.FromCSV(filename, leadingNaNs: length - 1);
        }
    }*/

    public class StandardDeviation
    {
        public StandardDeviation(int length, uint degreesOfFreedom = 1) 
        {
            Length = length;
            DegreesOfFreedom = degreesOfFreedom;
        }
        public int Length { get; set; }
        public uint DegreesOfFreedom {  get; set; }
        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length, DegreesOfFreedom);
        }
        public static ColumnDouble Calculate(ColumnDouble column, int length, uint degreesOfFreedom = 0)
        {
            ColumnDouble result = new($"StDv{length}", column.Length);
            int start = 0;
            int stop = column.CountLeadingNaNs();
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

        public static ColumnDouble Calculate(OhlcData ohlcData)
        {
            ColumnDouble result = new("TrueRange", ohlcData.Length);
            result[0] = double.NaN;
            for (int i = 1; i < ohlcData.Length; i++)
            {
                result[i] = TrueRange.Calculate(ohlcData.High[i], ohlcData.Low[i], ohlcData.Close[i - 1]);
            }
            return result;
        }
    }
    public static class Utils
    {
        public static ColumnDouble GetMarketData(OhlcData ohlcData, MarketDataType dataType)
        {
            ColumnDouble data = dataType switch
            {
                MarketDataType.Close => ohlcData.Close,
                MarketDataType.High => ohlcData.High,
                MarketDataType.Low => ohlcData.Low,
                MarketDataType.Open => ohlcData.Open,
                MarketDataType.HL2 => (ohlcData.Low + ohlcData.High) / 2,
                MarketDataType.HLC3 => (ohlcData.Low + ohlcData.High + ohlcData.Close) / 3,
                MarketDataType.OHLC4 => (ohlcData.Open + ohlcData.High + ohlcData.Low + ohlcData.Close) / 4,
                MarketDataType.Volume => new ColumnDouble(ohlcData.Volume),
                _ => ohlcData.Close
            };
            return data;
        }
    }
}