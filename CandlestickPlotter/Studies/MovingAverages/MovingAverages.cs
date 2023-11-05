using CandleStickPlotter.DataTypes;

namespace CandleStickPlotter.Studies.MovingAverages
{
    public enum MovingAverageType : byte
    {
        ExponentialMovingAverage, SimpleMovingAverage, WeightedMovingAverage, WildersMovingAverage
    }
    
    public class ExponentialMovingAverage
    {
        public ExponentialMovingAverage(int length)
        {
            Length = length;
            SmoothingFactor = 2.0 / (length + 1);
        }

        public int Length { get; set; }
        public double SmoothingFactor { get; set; }
        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length);
        }

        public static ColumnDouble Calculate(ColumnDouble column, int length)
        {
            double smoothingFactor = 2.0 / (length + 1);
            return Calculate(column, length, smoothingFactor);
        }
        public static ColumnDouble Calculate(ColumnDouble column, int length, double smoothingFactor)
        {
            ColumnDouble result = new($"EMA{length}", column.Length);
            
            int i = 0, leadingNaNs = column.CountLeadingNaNs();
            for (; i < leadingNaNs; i++)
            {
                result[i] = double.NaN;
            }
            result[i] = column[i];
            i++;
            for (; i < column.Length; i++)
                result[i] = (column[i] - result[i - 1]) * smoothingFactor + result[i - 1];
            return result;
        }
    }
    public class SimpleMovingAverage
    {
        public SimpleMovingAverage(int length)
        {
            Length = length;
        }
        public int Length { get; set; }

        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length);
        }

        public static ColumnDouble Calculate(ColumnDouble column, int length)
        {
            ColumnDouble result = new($"SMA{length}", column.Length);
            double sum = 0;
            int i = 0, stop = column.CountLeadingNaNs();
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
    }
    public class WeightedMovingAverage
    {
        public WeightedMovingAverage(int length)
        {
            Length = length;
        }
        public int Length { get; set; }
        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length);
        }
        public static ColumnDouble Calculate(ColumnDouble column, int length)
        {
            ColumnDouble result = new ($"WMA{length}", column.Length);
            double divisor = length * (length + 1) / 2.0;
            double rollingSum = 0;
            double weightedSum = 0;
            int i = 0, stop = column.CountLeadingNaNs();
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
    }
    public class WildersMovingAverage
    {
        public WildersMovingAverage(int length)
        {
            Length = length;
        }
        public int Length { get; set; }
        public ColumnDouble Calculate(ColumnDouble column)
        {
            return Calculate(column, Length);
        }
        public static ColumnDouble Calculate(ColumnDouble column, int length)
        {
            double smoothingFactor = 1.0 / length;
            ColumnDouble result = new($"WildersMA{length}", column.Length);
            int i = 0, stop = column.CountLeadingNaNs();
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
    }
}
