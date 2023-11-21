using System.Numerics;

namespace CandleStickPlotter.DataTypes
{
    public abstract class Column
    {
        public static Column<byte> FromTrueIndices(int[] indices, int length)
        {
            Column<byte> result = new(length);
            for (int i = 0; i < indices.Length; i++)
                result[indices[i]] = 1;
            return result;
        }
        public static int CountLeadingNaNs(Column<double> column)
        {
            for (int i = column.Length - 1; i >= 0; i--)
                if (!double.IsNaN(column[i])) return column.Length - i - 1;
            return column.Length;
        }
        public static int CountSupersedingNaNs(Column<double> column)
        {
            for (int i = 0; i < column.Length; i++)
                if (!double.IsNaN(column[i])) return i;
            return column.Length;
        }
        public static void SetLeadingNaNs(Column<double> column, int amount)
        {
            for (int i = 0; i < amount; i++)
                column[i] = double.NaN;
        }
    }
    public class Column<T> where T : struct, INumber<T>
    {
        public enum DivByZeroPolicy : byte { NONE, SET_TO_ZERO }
        public int Length { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public T[] Values { get; private set; } = Array.Empty<T>();
        public T MaxValue => Values.Max();
        public T MinValue => Values.Min();
        public Column() { }
        public Column(Column<T> column)
        {
            Length = column.Length;
            Values = new T[Length];
            for (int i = 0; i < Length; i++)
                Values[i] = column[i];
        }
        public Column(int length)
        {
            Length = length;
            Values = new T[length];
        }
        public Column(int length, T value)
        {
            Values = new T[length];
            Array.Fill(Values, value);
        }
        public Column(string name, int length)
        {
            Length = length;
            Name = name;
            Values = new T[length];
        }
        public Column(T[] array)
        {
            Length = array.Length;
            Values = array;
        }
        public Column(string name, T[] array)
        {
            Name = name;
            Length = array.Length;
            Values = array;
        }
        public T this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }
        public override bool Equals(object? obj) => Equals(obj as Column<T>);
        public bool Equals(Column<T>? column)
        {
            if (column is null) return false;
            if (ReferenceEquals(this, column)) return true;
            if (GetType() != column.GetType()) return false;
            if (Length != column.Length) return false;
            if (Name != column.Name) return false;
            for (int i = 0; i < Length; i++)
                if (Values[i] != column[i]) return false;
            return true;
        }
        public override int GetHashCode() => HashCode.Combine(Name, Length, Values);
        public void Displace(int amount)
        {
            if (Values == Array.Empty<T>()) return;
            if (amount == 0) return;
            if (amount > 0)
            {
                int i = Length - 1;
                for (; i >= amount; i--)
                    Values[i] = Values[i - amount];
                for (; i >= 0; i--)
                    Values[i] = default;
            }
            else
            {
                int i = 0;
                int stop = Length - amount;
                for (; i < stop; i++)
                    Values[i] = Values[i + amount];
                for (; i < Length; i++)
                    Values[i] = default;
            }
        }
        public int[] GetTrueIndices()
        {
            List<int> indices = new();
            for (int i = 0; i < Length; i++)
                if (!double.IsNaN(Convert.ToDouble(Values[i])))
                    if (Convert.ToBoolean(Values[i])) indices.Add(i);
            return indices.ToArray();
        }
        public int[] GetFalseIndices()
        {
            List<int> indices = new();
            for (int i = 0; i < Length; i++)
                if (!double.IsNaN(Convert.ToDouble(Values[i])))
                    if (!Convert.ToBoolean(Values[i])) indices.Add(i);
            return indices.ToArray();
        }
        public Column<T> Diff(int periodsAgo)
        {
            Column<T> result = new($"{Name}.Diff({periodsAgo})", Length);
            for (int i = periodsAgo; i < Length; i++)
                result[i] = Values[i] - Values[i - periodsAgo];
            return result;
        }
        public Column<T> Abs()
        {
            Column<T> result = new($"{Name}.Abs()", Length);
            for (int i = 0; i < Length; i++)
                result[i] = T.Abs(Values[i]);
            return result;
        }
        public Column<T> Min(int length)
        {
            Column<T> result = new($"{Name}.Min({length})", Length);
            Deque<T> deque = new();
            int i = 0;
            int stop = length;
            int tempIndex = 0;
            for (; i < stop; i++)
            {
                while (deque.Count > 0 && Values[i] < deque.Rear) deque.PopBack();
                deque.PushBack(Values[i]);
            }
            result[tempIndex] = deque.Front;
            if (Values[tempIndex++] == deque.Front) deque.PopFront();
            stop = Length;
            for (; i < stop; i++)
            {
                while (deque.Count > 0 && Values[i] < deque.Rear) deque.PopBack();
                deque.PushBack(Values[i]);
                result[tempIndex] = deque.Front;
                if (Values[tempIndex++] == deque.Front) deque.PopFront();
            }
            return result;
        }
        public Column<T> Max(int length)
        {
            Column<T> result = new($"{Name}.Max({length})", Length);
            Deque<T> deque = new();
            int i = 0;
            int stop = length;
            int tempIndex = 0;
            for (; i < stop; i++)
            {
                while (deque.Count > 0 && Values[i] > deque.Rear) deque.PopBack();
                deque.PushBack(Values[i]);
            }
            result[tempIndex] = deque.Front;
            if (Values[tempIndex++] == deque.Front) deque.PopFront();
            stop = Length;
            for (; i < stop; i++)
            {
                while (deque.Count > 0 && Values[i] > deque.Rear) deque.PopBack();
                deque.PushBack(Values[i]);
                result[tempIndex] = deque.Front;
                if (Values[tempIndex++] == deque.Front) deque.PopFront();
            }
            return result;
        }
        public Column<T> Div(Column<T> denominator, DivByZeroPolicy divByZeroPolicy = DivByZeroPolicy.NONE)
        {
            if (Values.Length != denominator.Values.Length) throw new ArithmeticException();
            Column<T> result = new("", Length);
            for (int i = 0; i < Length; i++)
            {
                if (denominator[i] == T.Zero && divByZeroPolicy == DivByZeroPolicy.SET_TO_ZERO)
                    result[i] = T.Zero;
                else
                    result[i] = Values[i] / denominator[i];
            }
            return result;
        }
        public static Column<T> operator +(Column<T> column, T value)
        {
            Column<T> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] + value;
            return result;
        }
        public static Column<T> operator +(T value, Column<T> column) => column + value;
        public static Column<T> operator -(Column<T> column, T value)
        {
            Column<T> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] - value;
            return result;
        }
        public static Column<T> operator -(T value, Column<T> column) => column - value;
        public static Column<T> operator *(Column<T> column, T value)
        {
            Column<T> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] * value;
            return result;
        }

        public static Column<T> operator *(T value, Column<T> column) => column * value;

        public static Column<T> operator /(Column<T> column, T value)
        {
            Column<T> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] / value;
            return result;
        }

        public static Column<T> operator /(T value, Column<T> column)
        {
            Column<T> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = value / column[i];
            return result;
        }

        public static Column<T> operator +(Column<T> column1, Column<T> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<T> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] + column2[i];
            return result;
        }

        public static Column<T> operator -(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            Column<T> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] - column2[i];
            return result;
        }

        public static Column<T> operator *(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            Column<T> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] * column2[i];
            return result;
        }
        public static Column<T> operator /(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            Column<T> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] / column2[i];
            return result;
        }
        public static int[] operator >(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] > column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator >=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] >= column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator <(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] < column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator <=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] <= column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator >(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] > value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator >=(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] >= value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator <(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] < value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator <=(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] <= value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator ==(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] == value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator !=(Column<T> column, T value)
        {
            List<int> indices = new();
            for (int i = 0; i < column.Length; i++)
                if (column[i] != value) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator >(T value, Column<T> column) => column < value;
        public static int[] operator >=(T value, Column<T> column) => column <= value;
        public static int[] operator <(T value, Column<T> column) => column > value;
        public static int[] operator <=(T value, Column<T> column) => column >= value;
        public static int[] operator ==(T value, Column<T> column) => column == value;
        public static int[] operator !=(T value, Column<T> column) => column != value;
        public static int[] operator ==(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] == column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static int[] operator !=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = new();
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] != column2[i]) indices.Add(i);
            return indices.ToArray();
        }
        public static Column<byte> operator &(Column<T> column1, Column<T> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = Convert.ToByte(column1[i] != T.Zero && column2[i] != T.Zero);
            return result;
        }
        public static Column<byte> operator |(Column<T> column1, Column<T> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = Convert.ToByte(column1[i] != T.Zero || column2[i] != T.Zero);
            return result;
        }
        public int[] CrossesAbove(Column<T> column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            List<int> indices = new();
            bool prevIsBelow = Values[0] <= column[0];
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > column[i];
                if (prevIsBelow && currentIsAbove) indices.Add(i);
                prevIsBelow = !currentIsAbove;
            }
            return indices.ToArray();
        }
        public int[] CrossesBelow(Column<T> column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            List<int> indices = new();
            bool prevIsAbove = Values[0] >= column[0];
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < column[i];
                if(prevIsAbove && currentIsBelow) indices.Add(i);
                prevIsAbove = !currentIsBelow;
            }
            return indices.ToArray();
        }
        public int[] CrossesAbove(T value)
        {
            List<int> indices = new();
            bool prevIsBelow = Values[0] <= value;
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > value;
                if (prevIsBelow && currentIsAbove) indices.Add(i);
                prevIsBelow = !currentIsAbove;
            }
            return indices.ToArray();
        }
        public int[] CrossesBelow(T value)
        {
            List<int> indices = new();
            bool prevIsAbove = Values[0] >= value;
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < value;
                if (prevIsAbove && currentIsBelow) indices.Add(i);
                prevIsAbove = !currentIsBelow;
            }
            return indices.ToArray();
        }
    }
    public abstract class Table
    {
        public static void SetLeadingNaNs(Table<double> table, int amount)
        {
            for (int i = 0; i < table.ColumnCount; i++)
                Column.SetLeadingNaNs(table[i], amount);
        }
    }
    public class Table<T> where T : struct, INumber<T>
    {
        private Dictionary<string, int> TableNameIndex { get; set; }
        public Column<byte>[] BooleanSignals { get; set; }
        public Column<T>[] Columns { get; set; }
        public int RowCount
        {
            get
            {
                if (Columns == Array.Empty<Column<T>>()) return 0;
                else return Columns[0].Length;
            }
        }
        public int ColumnCount => Columns.Length;
        public Table()
        {
            TableNameIndex = new Dictionary<string, int>();
            Columns = Array.Empty<Column<T>>();
            BooleanSignals = Array.Empty<Column<byte>>();
        }
        public Table(int rows, string[] columnNames)
        {
            Columns = new Column<T>[columnNames.Length];
            for (int i = 0; i < Columns.Length; i++)
                Columns[i] = new Column<T>(columnNames[i], rows);
            TableNameIndex = new Dictionary<string, int>();
            SetNameDictionary();
            BooleanSignals = Array.Empty<Column<byte>>();
        }
        public Table(Column<T>[] columns)
        {
            if (!SameLengthColumns(columns)) throw new ArgumentException("Columns must have the same length.");
            Columns = columns;
            TableNameIndex = new Dictionary<string, int>();
            SetNameDictionary();
            BooleanSignals = Array.Empty<Column<byte>>();
        }
        private void SetNameDictionary()
        {            
            for (int i = 0; i < Columns.Length; i++)
                TableNameIndex.Add(Columns[i].Name, i);
        }
        public Column<T> this[int index]
        {
            get => Columns[index];
            set => Columns[index] = value;
        }
        public Column<T> this[string columnName]
        {
            get => Columns[TableNameIndex[columnName]];
            set => Columns[TableNameIndex[columnName]] = value;
        }

        private static bool SameLengthColumns(Column<T>[] columns)
        {
            if (columns == Array.Empty<Column<T>>()) return true;
            int count = columns[0].Length;
            for (int i = 1; i < columns.Length; i++)
                if (columns[i].Length != count) return false;
            return true;
        }
        public void Displace(int amount)
        {
            foreach (Column<T> column in Columns)
                column.Displace(amount);
        }
    }
}
    /*
    public class Column<byte>
    {
        public int TrueValuesCount { get; private set; }
        public int Length { get; set; }
        public string Name { get; set; }
        protected bool[]? Values { get; set; }
        public Column<byte>()
        {
            Length = 0;
            Name = "";
            Values = null;
            TrueValuesCount = 0;
        }
        public Column<byte>(string name, int length)
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
        public override bool Equals(object? obj) => Equals(obj as Column<byte>);
        public bool Equals(Column<byte>? column)
        {
            if (column is null) return false;
            if (ReferenceEquals(this, column)) return true;
            if (GetType() != column.GetType()) return false;
            if (Length != column.Length) return false;
            if (Name !=  column.Name) return false;
            for (int i = 0; i < Length; i++)
            {
                if (Values[i] != column[i]) return false;
            }
            return true;
        }
        public override int GetHashCode() => HashCode.Combine(Name, Length, Values);
        public static Column<byte> operator ==(Column<byte> column1, Column<byte> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] == column2[i];
            return result;
        }
        public static Column<byte> operator !=(Column<byte> column1, Column<byte> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] != column2[i];
            return result;
        }
        public static Column<byte> operator &(Column<byte> column1, Column<byte> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] && column2[i]; 
            return result;
        }
        public static Column<byte> operator |(Column<byte> column1, Column<byte> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] || column2[i];
            return result;
        }
        public bool this[int index]
        {
            get => Values[index];
            set
            {
                // If the current value is true and the new value is false, then decrement the count of true values
                if (Values[index] && !value) TrueValuesCount--;
                // If the current value if false and the new value is true, then increment the count of true values
                else if (!Values[index] && value) TrueValuesCount++;
                Values[index] = value;
            }
        }

        public int[] GetTrueIndices()
        {
            List<int> indices = new();
            if (Values == null || Length == 0) return Array.Empty<int>();
            for (int i = 0; i < Length; i++)
            {
                if (Values[i]) indices.Add(i);
            }
            return indices.ToArray();
        }

        public int[] GetFalseIndices()
        {
            List<int> indices = new();
            if (Values == null || Length == 0) return Array.Empty<int>();
            for (int i = 0; i < Length; i++)
            {
                if (!Values[i]) indices.Add(i);
            }
            return indices.ToArray();
        }
    }
    public class Column<double> : Column<double>
    {
        public Column<double>()
        {
            Name = string.Empty;
            Length = 0;
            Values = Array.Empty<double>();
        }
        public Column<double>(string name, int length) : base(name, length) { }
        public Column<double>(Column<long> column) : base(column.Name, column.Length)
        {
            for (int i = 0; i < Length; i++)
                Values[i] = column[i];
        }
        public Column<double>(double[] array)
        {
            Name = "";
            Length = array.Length;
            Values = array;
        }
        public override bool Equals(object? obj) => Equals(obj as Column<double>);
        public bool Equals(Column<double>? column)
        {
            if (column is null) return false;
            if (ReferenceEquals(this, column)) return true;
            if (GetType() != column.GetType()) return false;
            if (Length != column.Length) return false;
            if (Name != column.Name) return false;
            for (int i = 0; i < Length; i++)
            {
                if (Values[i] != column[i]) return false;
            }
            return true;
        }
        public override int GetHashCode() => HashCode.Combine(Name, Length, Values);
        public double Max() => Values.Max()!;
        public double Min() => Values.Min()!;
        public int CountLeadingNaNs()
        {
            int count = 0;
            for (int i = 0; i < Length; i++)
            {
                if (!double.IsNaN(Values[i])) return count;
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
        public Column<double> Diff(int periodsAgo)
        {
            Column<double> result = new($"{Name}.Diff({periodsAgo})", Length);
            for (int i = periodsAgo; i <  Length; i++)
                result[i] = Values[i] - Values[i - periodsAgo];
            return result;
        }
        public Column<double> Abs()
        {
            Column<double> result = new($"{Name}.Abs()", Length);
            for (int i = 0; i < Length; i++)
                result[i] = Math.Abs(Values[i]);
            return result;
        }
        public Column<double> Min(int length)
        {
            Column<double> result = new($"Min{length}", Length);
            int i = 0;
            int alreadySet = 0;
            KeyValuePair<int, double>[] pairArray = new KeyValuePair<int, double>[Length];
            i = length - 1;
            for (; i < Length; i++)
                result[i] = double.NaN;
            i = 0;
            for (; i < Length; i++)
                pairArray[i] = new KeyValuePair<int, double>(i, Values[i]);
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
        public Column<double> Max(int length)
        {
            Column<double> result = new($"Min{length}", Length);
            int i = 0;
            int alreadySet = 0;
            KeyValuePair<int, double>[] pairArray = new KeyValuePair<int, double>[Length];
            i = length - 1;
            for (; i < Length; i++)
                result[i] = double.NaN;
            i = 0;
            for (; i < Length; i++)
                pairArray[i] = new KeyValuePair<int, double>(i, Values[i]);
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
        public Column<double> Div(Column<double> denominator, DivByZeroPolicy divByZeroPolicy = DivByZeroPolicy.NONE)
        {
            if (Values.Length != denominator.Values.Length) throw new ArithmeticException();
            double policy = 0;
            if (divByZeroPolicy == DivByZeroPolicy.SET_TO_NAN)
                policy = double.NaN;
            Column<double> result = new("", Length);
            for (int i = 0; i < Length; i++)
            {
                if (denominator[i] == 0.0 && divByZeroPolicy != DivByZeroPolicy.NONE)
                    result[i] = policy;
                else
                    result[i] = Values[i] / denominator[i];
            }
            return result;
        }
        public static explicit operator Column<double>(Column<long> columnLong) => new(columnLong);
        public static Column<double> operator+(Column<double> column, double value)
        {
            Column<double> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] + value;
            return result;
        }
        public static Column<double> operator +(double value, Column<double> column)
        {
            return column + value;
        }
        public static Column<double> operator -(Column<double> column, double value)
        {
            Column<double> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] - value;
            return result;
        }
        public static Column<double> operator -(double value, Column<double> column)
        {
            return column - value;
        }
        public static Column<double> operator *(Column<double> column, double value)
        {
            Column<double> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] * value;
            return result;
        }

        public static Column<double> operator *(double value, Column<double> column)
        {
            return column * value;
        }

        public static Column<double> operator /(Column<double> column, double value)
        {
            Column<double> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] / value;
            return result;
        }

        public static Column<double> operator /(double value, Column<double> column)
        {
            Column<double> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = value / column[i];
            return result;
        }

        public static Column<double> operator+(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<double> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] + column2[i];
            return result;
        }

        public static Column<double> operator -(Column<double> a, Column<double> b)
        {
            if (a.Values.Length != a.Values.Length) throw new ArithmeticException();
            Column<double> result = new("", a.Length);
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] - b[i];
            return result;
        }

        public static Column<double> operator *(Column<double> a, Column<double> b)
        {
            if (a.Values.Length != a.Values.Length) throw new ArithmeticException();
            Column<double> result = new("", a.Length);
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] * b[i];
            return result;
        }
        public static Column<double> operator /(Column<double> numerator, Column<double> denominator)
        {
            if (numerator.Length != denominator.Length) throw new ArithmeticException();
            Column<double> result = new("", numerator.Length);
            for (int i = 0; i < numerator.Length; i++)
                result[i] = numerator[i] / denominator[i];
            return result;
        }
        public static Column<byte> operator>(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] > column2[i];
            return result;
        }
        public static Column<byte> operator >=(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] >= column2[i];
            return result;
        }
        public static Column<byte> operator <(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] < column2[i];
            return result;
        }
        public static Column<byte> operator <=(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] <= column2[i];
            return result;
        }
        public static Column<byte> operator >(Column<double> column, double value)
        {
            Column<byte> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] > value;
            return result;
        }
        public static Column<byte> operator >=(Column<double> column1, double value)
        {
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] >= value;
            return result;
        }
        public static Column<byte> operator <(Column<double> column, double value)
        {
            Column<byte> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] < value;
            return result;
        }
        public static Column<byte> operator <=(Column<double> column, double value)
        {
            Column<byte> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] <= value;
            return result;
        }
        public static Column<byte> operator ==(Column<double> column, double value)
        {
            Column<byte> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] == value;
            return result;
        }
        public static Column<byte> operator !=(Column<double> column, double value)
        {
            Column<byte> result = new("", column.Length);
            for (int i = 0; i < column.Length; i++)
                result[i] = column[i] != value;
            return result;
        }
        public static Column<byte> operator >(double value, Column<double> column)
        {
            return column < value;
        }
        public static Column<byte> operator >=(double value, Column<double> column)
        {
            return column <= value;
        }
        public static Column<byte> operator <(double value, Column<double> column)
        {
            return column > value;
        }
        public static Column<byte> operator <=(double value, Column<double> column)
        {
            return column >= value;
        }
        public static Column<byte> operator ==(double value, Column<double> column)
        {
            return column == value;
        }
        public static Column<byte> operator !=(double value, Column<double> column)
        {
            return column != value;
        }
        public static Column<byte> operator ==(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] == column2[i];
            return result;
        }
        public static Column<byte> operator !=(Column<double> column1, Column<double> column2)
        {
            if (column1.Length != column2.Length) throw new ArithmeticException();
            Column<byte> result = new("", column1.Length);
            for (int i = 0; i < column1.Length; i++)
                result[i] = column1[i] != column2[i];
            return result;
        }
        public Column<byte> CrossesAbove(Column<double> column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            Column<byte> result = new($"{Name}CrossesAbove{column.Name}", Length);
            result[0] = false;
            bool prevIsBelow = Values[0] <= column[0];
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > column[i];
                result[i] = prevIsBelow && currentIsAbove;
                prevIsBelow = !currentIsAbove;
            }
            return result;
        }
        public Column<byte> CrossesBelow(Column<double> column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            Column<byte> result = new($"{Name}CrossesBelow{column.Name}", Length);
            result[0] = false;
            bool prevIsAbove = Values[0] >= column[0];
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < column[i];
                result[i] = prevIsAbove && currentIsBelow;
                prevIsAbove = !currentIsBelow;
            }
            return result;
        }
        public Column<byte> CrossesAbove(double value)
        {
            Column<byte> result = new($"{Name}CrossesAbove{value}", Length);
            result[0] = false;
            bool prevIsBelow = Values[0] <= value;
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > value;
                result[i] = prevIsBelow && currentIsAbove;
                prevIsBelow = !currentIsAbove;
            }
            return result;
        }
        public Column<byte> CrossesBelow(double value)
        {
            Column<byte> result = new($"{Name}CrossesBelow{value}", Length);
            result[0] = false;
            bool prevIsAbove = Values[0] >= value;
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < value;
                result[i] = prevIsAbove && currentIsBelow;
                prevIsAbove = !currentIsBelow;
            }
            return result;
        }
        public void ToCSV(string filename)
        {
            using StreamWriter writer = new(filename);
            writer.Write(string.Join(',', Values));
        }
    }
    public class Table<double>
    {
        public Column<double>[] Columns { get; set; }
        public Column<byte>[]? BooleanColumns { get; set; }
        public Table<double>(int nColumns)
        {
            Columns = new Column<double>[nColumns];
        }
        public Table<double>(Column<double>[] columns)
        {
            Columns = columns;
        }
        public Column<double> this[int index]
        {
            get { return Columns[index]; }
            set { Columns[index] = value;}
        }
        public Column<double> this[string columnName]
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
        public static Table<double> FromCSV(string filename, int leadingNaNs = 0)
        {
            using StreamReader reader = new(filename);
            string[] lines = reader.ReadToEnd().Split('\n');
            string[] vals = lines[0].Split(',');
            Column<double>[] columns = new Column<double>[vals.Length];
            int i = 0;
            for (; i < vals.Length; i++)
            {
                columns[i] = new Column<double>(vals[i], lines.Length - 1 + leadingNaNs - 1);
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
            return new Table<double>(columns);
        }
        public void SetLeadingNaNs(int leadingNaNs)
        {
            for (int i = 0; i < Columns.Length; i++)
                for (int j = 0; j < leadingNaNs; j++)
                    Columns[i][j] = double.NaN;
        }
    }
    public class Table<double>
    {
        public Table<double>(string filename)
        {
            Length = File.ReadLines(filename).Count() - 1;
            Timestamp = new Column<long>("Timestamp", Length);
            Open = new Column<double>("Open", Length);
            High = new Column<double>("High", Length);
            Low = new Column<double>("Low", Length);
            Close = new Column<double>("Close", Length);
            AdjustedClose = new Column<double>("Adj. Close", Length);
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
                Timestamp[index] = Convert.ToInt64(values[0]);
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
        public Column<double> Close { get; private set; }
        public Column<long> Timestamp { get; private set; }
        public Column<double> High { get; private set; }
        public Column<double> Low { get; private set; }
        public Column<double> Open { get; private set; }
        public Column<double> AdjustedClose { get; private set; }
        public Column<long> Volume { get; private set; }
    }*/