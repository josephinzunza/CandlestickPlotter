using System.Data.Common;
using System.Reflection.Metadata.Ecma335;

namespace CandleStickPlotter.DataTypes
{
    public class Column<T>
    {
        public enum DivByZeroPolicy : byte { NONE, SET_TO_NAN, SET_TO_ZERO }
        public Column()
        {
            Length = 0;
            Name = "";
            Values = null;
        }
        public Column(string name, int length)
        {
            Length = length;
            Name = name;
            Values = new T[length]; 
        }
        public int Length { get; set; }
        public string Name { get; set; }
        protected T[]? Values { get; set; }
        public T this[int index]
        {
            get => Values![index];
            set => Values![index] = value;
        }
    }
    public class ColumnBool
    {
        public int TrueValuesCount { get; private set; }
        public int Length { get; set; }
        public string Name { get; set; }
        protected bool[]? Values { get; set; }
        public ColumnBool()
        {
            Length = 0;
            Name = "";
            Values = null;
            TrueValuesCount = 0;
        }
        public ColumnBool(string name, int length)
        {
            Name = name;
            Length = length;
            Values = new bool[length];
            TrueValuesCount = 0;
        }
        public void Displace(int amount)
        {
            if (Values == null) return;
            if (amount == 0) return;
            if (amount > 0)
            {
                int i = Length - 1;
                for (; i >= amount; i--)
                    Values[i] = Values[i - amount];
                for (; i >= 0; i--)
                {
                    if (Values[i]) TrueValuesCount--;
                    Values[i] = false;
                }
            }
            else
            {
                int i = 0;
                int stop = Length - amount;
                for (; i < stop; i++)
                    Values[i] = Values[i + amount];
                for (; i < Length; i++)
                {
                    if (Values[i]) TrueValuesCount--;
                    Values[i] = false;
                }
            }
        }
        public override bool Equals(object? obj) => Equals(obj as ColumnBool);
        public bool Equals(ColumnBool? column)
        {
            if (column is null) return false;
            if (ReferenceEquals(this, column)) return true;
            if (GetType() != column.GetType()) return false;
            if (Length != column.Length) return false;
            if (Name !=  column.Name) return false;
            for (int i = 0; i < Length; i++)
            {
                if (Values![i] != column[i]) return false;
            }
            return true;
        }
        public override int GetHashCode() => HashCode.Combine(Name, Length, Values);
        public static ColumnBool operator ==(ColumnBool column1, ColumnBool column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] == column2[i];
            return result;
        }
        public static ColumnBool operator !=(ColumnBool column1, ColumnBool column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] != column2[i];
            return result;
        }
        public static ColumnBool operator &(ColumnBool column1, ColumnBool column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] && column2[i]; 
            return result;
        }
        public static ColumnBool operator |(ColumnBool column1, ColumnBool column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] || column2[i];
            return result;
        }
        public bool this[int index]
        {
            get => Values![index];
            set
            {
                // If the current value is true and the new value is false, then decrement the count of true values
                if (Values![index] && !value) TrueValuesCount--;
                // If the current value if false and the new value is true, then increment the count of true values
                else if (!Values![index] && value) TrueValuesCount++;
                Values![index] = value;
            }
        }

        public int[] GetTrueIndices()
        {
            List<int> indices = new();
            if (Values == null || Length == 0) return Array.Empty<int>();
            for (int i = 0; i < Length; i++)
            {
                if (Values![i]) indices.Add(i);
            }
            return indices.ToArray();
        }

        public int[] GetFalseIndices()
        {
            List<int> indices = new();
            if (Values == null || Length == 0) return Array.Empty<int>();
            for (int i = 0; i < Length; i++)
            {
                if (!Values![i]) indices.Add(i);
            }
            return indices.ToArray();
        }
    }
    public class ColumnDouble : Column<double>
    {
        public ColumnDouble(string name, int length) : base(name, length) { }
        public ColumnDouble(Column<long> column) : base(column.Name, column.Length)
        {
            for (int i = 0; i < Length; i++)
                Values![i] = column[i];
        }
        public ColumnDouble(double[] array)
        {
            Name = "";
            Length = array.Length;
            Values = array;
        }
        public override bool Equals(object? obj) => Equals(obj as ColumnDouble);
        public bool Equals(ColumnDouble? column)
        {
            if (column is null) return false;
            if (ReferenceEquals(this, column)) return true;
            if (GetType() != column.GetType()) return false;
            if (Length != column.Length) return false;
            if (Name != column.Name) return false;
            for (int i = 0; i < Length; i++)
            {
                if (Values![i] != column[i]) return false;
            }
            return true;
        }
        public override int GetHashCode() => HashCode.Combine(Name, Length, Values);
        public double Max() => Values!.Max()!;
        public double Min() => Values!.Min()!;
        public int CountLeadingNaNs()
        {
            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                if (!double.IsNaN(Values![i])) return count;
                count++;
            }
            return count;
        }
        public void Displace(int amount)
        {
            if (Values == null) return;
            if (amount == 0) return;
            if (amount > 0)
            {
                int i = Length - 1;
                for (; i >= amount; i--)
                    Values[i] = Values[i - amount];
                for (; i >= 0; i--)
                    Values[i] = double.NaN;
            }
            else
            {
                int i = 0;
                int stop = Length - amount;
                for (; i < stop; i++)
                    Values[i] = Values[i + amount];
                for (; i < Length; i++)
                    Values[i] = double.NaN;
            }
        }
        public ColumnDouble Diff(int periodsAgo)
        {
            ColumnDouble result = new($"{Name}.Diff({periodsAgo})", Length);
            for (int i = periodsAgo; i <  Length; i++)
                result[i] = Values![i] - Values[i - periodsAgo];
            return result;
        }
        public ColumnDouble Abs()
        {
            ColumnDouble result = new($"{Name}.Abs()", Length);
            for (int i = 0; i < Length; i++)
                result[i] = Math.Abs(Values![i]);
            return result;
        }
        public ColumnDouble Min(int length)
        {
            ColumnDouble result = new($"Min{length}", Length);
            int i = 0;
            int alreadySet = 0;
            KeyValuePair<int, double>[] pairArray = new KeyValuePair<int, double>[Length];
            i = length - 1;
            for (; i < Length; i++)
                result[i] = double.NaN;
            i = 0;
            for (; i < Length; i++)
                pairArray[i] = new KeyValuePair<int, double>(i, Values![i]);
            IEnumerable<KeyValuePair<int, double>> sorted = pairArray.OrderBy(arr => arr.Value);
            int stop;
            foreach (KeyValuePair<int, double> pair in sorted)
            {
                i = pair.Key;
                stop = i + length;
                for (; i < stop && i < Length; i++)
                {
                    if (double.IsNaN(result[i]))
                    {
                        result[i] = pair.Value;
                        alreadySet++;
                        if (alreadySet == Length)
                        {
                            i = 0;
                            stop = length + CountLeadingNaNs() - 1;
                            for (; i < stop; i++)
                                result[i] = double.NaN;
                            return result;
                        }
                    }
                }
            }
            i = 0;
            stop = length + CountLeadingNaNs() - 1;
            for (; i < stop; i++)
                result[i] = double.NaN;
            return result;
        }
        public ColumnDouble Max(int length)
        {
            ColumnDouble result = new($"Min{length}", Length);
            int i = 0;
            int alreadySet = 0;
            KeyValuePair<int, double>[] pairArray = new KeyValuePair<int, double>[Length];
            i = length - 1;
            for (; i < Length; i++)
                result[i] = double.NaN;
            i = 0;
            for (; i < Length; i++)
                pairArray[i] = new KeyValuePair<int, double>(i, Values![i]);
            IEnumerable<KeyValuePair<int, double>> sorted = pairArray.OrderByDescending(arr => arr.Value);
            int stop;
            foreach (KeyValuePair<int, double> pair in sorted)
            {
                i = pair.Key;
                stop = i + length;
                for (; i < stop && i < Length; i++)
                {
                    if (double.IsNaN(result[i]))
                    {
                        result[i] = pair.Value;
                        alreadySet++;
                        if (alreadySet == Length)
                        {
                            i = 0;
                            stop = length + CountLeadingNaNs() - 1;
                            for (; i < stop; i++)
                                result[i] = double.NaN;
                            return result;
                        }
                    }
                }
            }
            i = 0;
            stop = length + CountLeadingNaNs() - 1;
            for (; i < stop; i++)
                result[i] = double.NaN;
            return result;
        }
        public ColumnDouble Div(ColumnDouble denominator, DivByZeroPolicy divByZeroPolicy = DivByZeroPolicy.NONE)
        {
            if (Values!.Length != denominator.Values!.Length) throw new ArithmeticException();
            double policy = 0;
            if (divByZeroPolicy == DivByZeroPolicy.SET_TO_NAN)
                policy = double.NaN;
            ColumnDouble result = new("", Length);
            for (int i = 0; i < Length; i++)
            {
                if (denominator[i] == 0.0 && divByZeroPolicy != DivByZeroPolicy.NONE)
                    result[i] = policy;
                else
                    result[i] = Values[i] / denominator[i];
            }
            return result;
        }
        public static explicit operator ColumnDouble(Column<long> columnLong) => new(columnLong);
        public static ColumnDouble operator+(ColumnDouble column, double value)
        {
            ColumnDouble result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] + value;
            return result;
        }
        public static ColumnDouble operator +(double value, ColumnDouble column)
        {
            return column + value;
        }
        public static ColumnDouble operator -(ColumnDouble column, double value)
        {
            ColumnDouble result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] - value;
            return result;
        }
        public static ColumnDouble operator -(double value, ColumnDouble column)
        {
            return column - value;
        }
        public static ColumnDouble operator *(ColumnDouble column, double value)
        {
            ColumnDouble result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] * value;
            return result;
        }

        public static ColumnDouble operator *(double value, ColumnDouble column)
        {
            return column * value;
        }

        public static ColumnDouble operator /(ColumnDouble column, double value)
        {
            ColumnDouble result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] / value;
            return result;
        }

        public static ColumnDouble operator /(double value, ColumnDouble column)
        {
            ColumnDouble result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = value / column[i];
            return result;
        }

        public static ColumnDouble operator+(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnDouble result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] + column2[i];
            return result;
        }

        public static ColumnDouble operator -(ColumnDouble a, ColumnDouble b)
        {
            if (a.Values!.Length != a.Values!.Length) throw new ArithmeticException();
            ColumnDouble result = new("", a.Length);
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] - b[i];
            return result;
        }

        public static ColumnDouble operator *(ColumnDouble a, ColumnDouble b)
        {
            if (a.Values!.Length != a.Values!.Length) throw new ArithmeticException();
            ColumnDouble result = new("", a.Length);
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] * b[i];
            return result;
        }
        public static ColumnDouble operator /(ColumnDouble numerator, ColumnDouble denominator)
        {
            if (numerator.Length != denominator.Length) throw new ArithmeticException();
            ColumnDouble result = new("", numerator.Length);
            for (int i = 0; i < numerator.Length; i++)
                result[i] = numerator[i] / denominator[i];
            return result;
        }
        public static ColumnBool operator>(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] > column2[i];
            return result;
        }
        public static ColumnBool operator >=(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] >= column2[i];
            return result;
        }
        public static ColumnBool operator <(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] < column2[i];
            return result;
        }
        public static ColumnBool operator <=(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] <= column2[i];
            return result;
        }
        public static ColumnBool operator >(ColumnDouble column, double value)
        {
            ColumnBool result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] > value;
            return result;
        }
        public static ColumnBool operator >=(ColumnDouble column1, double value)
        {
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] >= value;
            return result;
        }
        public static ColumnBool operator <(ColumnDouble column, double value)
        {
            ColumnBool result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] < value;
            return result;
        }
        public static ColumnBool operator <=(ColumnDouble column, double value)
        {
            ColumnBool result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] <= value;
            return result;
        }
        public static ColumnBool operator ==(ColumnDouble column, double value)
        {
            ColumnBool result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] == value;
            return result;
        }
        public static ColumnBool operator !=(ColumnDouble column, double value)
        {
            ColumnBool result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] != value;
            return result;
        }
        public static ColumnBool operator >(double value, ColumnDouble column)
        {
            return column < value;
        }
        public static ColumnBool operator >=(double value, ColumnDouble column)
        {
            return column <= value;
        }
        public static ColumnBool operator <(double value, ColumnDouble column)
        {
            return column > value;
        }
        public static ColumnBool operator <=(double value, ColumnDouble column)
        {
            return column >= value;
        }
        public static ColumnBool operator ==(double value, ColumnDouble column)
        {
            return column == value;
        }
        public static ColumnBool operator !=(double value, ColumnDouble column)
        {
            return column != value;
        }
        public static ColumnBool operator ==(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] == column2[i];
            return result;
        }
        public static ColumnBool operator !=(ColumnDouble column1, ColumnDouble column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            ColumnBool result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] != column2[i];
            return result;
        }
        public ColumnBool CrossesAbove(ColumnDouble column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            ColumnBool result = new($"{Name}CrossesAbove{column.Name}", Length);
            result[0] = false;
            bool prevIsBelow = Values![0] <= column[0];
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values![i] > column[i];
                result[i] = prevIsBelow && currentIsAbove;
                prevIsBelow = !currentIsAbove;
            }
            return result;
        }
        public ColumnBool CrossesBelow(ColumnDouble column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            ColumnBool result = new($"{Name}CrossesBelow{column.Name}", Length);
            result[0] = false;
            bool prevIsAbove = Values![0] >= column[0];
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values![i] < column[i];
                result[i] = prevIsAbove && currentIsBelow;
                prevIsAbove = !currentIsBelow;
            }
            return result;
        }
        public ColumnBool CrossesAbove(double value)
        {
            ColumnBool result = new($"{Name}CrossesAbove{value}", Length);
            result[0] = false;
            bool prevIsBelow = Values![0] <= value;
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values![i] > value;
                result[i] = prevIsBelow && currentIsAbove;
                prevIsBelow = !currentIsAbove;
            }
            return result;
        }
        public ColumnBool CrossesBelow(double value)
        {
            ColumnBool result = new($"{Name}CrossesBelow{value}", Length);
            result[0] = false;
            bool prevIsAbove = Values![0] >= value;
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values![i] < value;
                result[i] = prevIsAbove && currentIsBelow;
                prevIsAbove = !currentIsBelow;
            }
            return result;
        }
        public void ToCSV(string filename)
        {
            using StreamWriter writer = new(filename);
            writer.Write(string.Join(',', Values!));
        }
    }
    public class TableDouble
    {
        public ColumnDouble[] Columns { get; set; }
        public ColumnBool[]? BooleanColumns { get; set; }
        public TableDouble(int nColumns)
        {
            Columns = new ColumnDouble[nColumns];
        }
        public TableDouble(ColumnDouble[] columns)
        {
            Columns = columns;
        }
        public ColumnDouble this[int index]
        {
            get { return Columns[index]; }
            set { Columns[index] = value;}
        }
        public ColumnDouble this[string columnName]
        {
            get
            {
                for (int i = 0; i < Columns.Length; i++)
                    if (Columns[i].Name == columnName) return Columns[i];
                throw new IndexOutOfRangeException();
            }
            set
            {
                for (int i = 0; i < Columns.Length; i++)
                    if (Columns[i].Name == columnName) Columns[i] = value;
                throw new IndexOutOfRangeException();
            }
        }
        public static TableDouble FromCSV(string filename, int leadingNaNs = 0)
        {
            using StreamReader reader = new(filename);
            string[] lines = reader.ReadToEnd().Split('\n');
            string[] vals = lines[0].Split(',');
            ColumnDouble[] columns = new ColumnDouble[vals.Length];
            int i = 0;
            for (; i < vals.Length; i++)
            {
                columns[i] = new ColumnDouble(vals[i], lines.Length - 1 + leadingNaNs - 1);
                for (int j = 0; j < leadingNaNs; j++)
                    columns[i][j] = double.NaN;
            }
            int currentIndex = leadingNaNs;
            i = 1;
            int stop = lines.Length - 1;
            for (; i < stop; i++)
            {
                vals = lines[i].Split(',');
                for (int j = 0; j < vals.Length; j++)
                    columns[j][currentIndex] = Convert.ToDouble(vals[j]);
                currentIndex++;
            }
            return new TableDouble(columns);
        }
        public void SetLeadingNaNs(int leadingNaNs)
        {
            for (int i = 0; i < Columns.Length; i++)
                for (int j = 0; j < leadingNaNs; j++)
                    Columns[i][j] = double.NaN;
        }
    }
    public class OhlcData
    {
        public OhlcData(string filename)
        {
            Length = File.ReadLines(filename).Count() - 1;
            Date = new Column<DateTime>("Date", Length);
            Open = new ColumnDouble("Open", Length);
            High = new ColumnDouble("High", Length);
            Low = new ColumnDouble("Low", Length);
            Close = new ColumnDouble("Close", Length);
            AdjustedClose = new ColumnDouble("Adj. Close", Length);
            Volume = new Column<long>("Volume", Length);

            using StreamReader sr = new(filename);
            int index = 0;
            string line;
            line = sr.ReadLine()!;
            string[] values;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine()!;
                values = line.Split(',');
                Date[index] = DateTime.Parse(values[0]);
                Open[index] = Convert.ToDouble(values[1]);
                High[index] = Convert.ToDouble(values[2]);
                Low[index] = Convert.ToDouble(values[3]);
                Close[index] = Convert.ToDouble(values[4]);
                if (values.Length > 6)
                    AdjustedClose[index] = Convert.ToDouble(values[5]);
                Volume[index] = long.Parse(values[6]);
                index++;
            }
        }
        public int Length { get; private set; }
        public ColumnDouble Close { get; private set; }
        public Column<DateTime> Date { get; private set; }
        public ColumnDouble High { get; private set; }
        public ColumnDouble Low { get; private set; }
        public ColumnDouble Open { get; private set; }
        public ColumnDouble AdjustedClose { get; private set; }
        public Column<long> Volume { get; private set; }
    }
}
