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
        public static int CountNaNsAtTop(Column<double> column)
        {
            for (int i = column.Length - 1; i >= 0; i--)
                if (!double.IsNaN(column[i])) return column.Length - i - 1;
            return column.Length;
        }
        public static int CountNaNsAtBottom(Column<double> column)
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
        public int Length => Values.Length;
        public string Name { get; set; } = string.Empty;
        public T[] Values { get; private set; } = [];
        public T MaxValue => Values.Where(item => !T.IsNaN(item)).Max();
        public T MinValue => Values.Where(item => !T.IsNaN(item)).Min();
        public Column() { }
        public Column(Column<T> column)
        {
            Values = new T[Length];
            Name = column.Name;
            for (int i = 0; i < Length; i++)
                Values[i] = column[i];
        }
        public Column(int length)
        {
            Values = new T[length];
        }
        public Column(int length, T value)
        {
            Values = new T[length];
            Array.Fill(Values, value);
        }
        public Column(string name, int length)
        {
            Name = name;
            Values = new T[length];
        }
        public Column(T[] array)
        {
            Values = array;
        }
        public Column(string name, T[] array)
        {
            Name = name;
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
            List<int> indices = [];
            for (int i = 0; i < Length; i++)
                if (!double.IsNaN(Convert.ToDouble(Values[i])))
                    if (Convert.ToBoolean(Values[i])) indices.Add(i);
            return [.. indices];
        }
        public int[] GetFalseIndices()
        {
            List<int> indices = [];
            for (int i = 0; i < Length; i++)
                if (!double.IsNaN(Convert.ToDouble(Values[i])))
                    if (!Convert.ToBoolean(Values[i])) indices.Add(i);
            return [.. indices];
        }
        public Column<double> Log10()
        {
            Column<double> result = new(Length);
            for (int i = 0; i < Length; i++)
                result[i] = Math.Log10(Convert.ToDouble(Values[i]));
            return result;
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
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] > column2[i]) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator >=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] >= column2[i]) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator <(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] < column2[i]) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator <=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] <= column2[i]) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator >(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] > value) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator >=(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] >= value) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator <(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] < value) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator <=(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] <= value) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator ==(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] == value) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator !=(Column<T> column, T value)
        {
            List<int> indices = [];
            for (int i = 0; i < column.Length; i++)
                if (column[i] != value) indices.Add(i);
            return [.. indices];
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
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] == column2[i]) indices.Add(i);
            return [.. indices];
        }
        public static int[] operator !=(Column<T> column1, Column<T> column2)
        {
            if (column1.Values.Length != column2.Values.Length) throw new ArithmeticException();
            List<int> indices = [];
            for (int i = 0; i < column1.Length; i++)
                if (column1[i] != column2[i]) indices.Add(i);
            return [.. indices];
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
            List<int> indices = [];
            bool prevIsBelow = Values[0] <= column[0];
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > column[i];
                if (prevIsBelow && currentIsAbove) indices.Add(i);
                prevIsBelow = !currentIsAbove;
            }
            return [.. indices];
        }
        public int[] CrossesBelow(Column<T> column)
        {
            if (Length != column.Length) throw new ArithmeticException();
            List<int> indices = [];
            bool prevIsAbove = Values[0] >= column[0];
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < column[i];
                if(prevIsAbove && currentIsBelow) indices.Add(i);
                prevIsAbove = !currentIsBelow;
            }
            return [.. indices];
        }
        public int[] CrossesAbove(T value)
        {
            List<int> indices = [];
            bool prevIsBelow = Values[0] <= value;
            bool currentIsAbove;
            for (int i = 1; i < Length; i++)
            {
                currentIsAbove = Values[i] > value;
                if (prevIsBelow && currentIsAbove) indices.Add(i);
                prevIsBelow = !currentIsAbove;
            }
            return [.. indices];
        }
        public int[] CrossesBelow(T value)
        {
            List<int> indices = [];
            bool prevIsAbove = Values[0] >= value;
            bool currentIsBelow;
            for (int i = 1; i < Length; i++)
            {
                currentIsBelow = Values[i] < value;
                if (prevIsAbove && currentIsBelow) indices.Add(i);
                prevIsAbove = !currentIsBelow;
            }
            return [.. indices];
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
        private Dictionary<string, int> TableNameIndex { get; set; } = [];
        private Column<T>[] Columns { get; set; }
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
            Columns = [];
        }
        public Table(Table<T> table)
        {
            Columns = new Column<T>[table.ColumnCount];
            for (int i = 0; i < table.ColumnCount; i++)
                Columns[i] = new Column<T>(table[i]);
            SetNameDictionary();
        }
        public Table(int rows, string[] columnNames)
        {
            Columns = new Column<T>[columnNames.Length];
            for (int i = 0; i < Columns.Length; i++)
                Columns[i] = new Column<T>(columnNames[i], rows);
            SetNameDictionary();
        }
        public Table(Column<T>[] columns)
        {
            if (!SameLengthColumns(columns)) throw new ArgumentException("Columns must have the same length.");
            Columns = columns;
            SetNameDictionary();
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
        public Table<double> Log10()
        {
            Column<double>[] columns = new Column<double>[Columns.Length];
            for (int i = 0; i <  columns.Length; i++)
            {
                columns[i] = Columns[i].Log10();
            }
            return new Table<double>(columns);
        }
        public static Table<T> operator +(Table<T> table, T value)
        {
            Column<T>[] columns = new Column<T>[table.Columns.Length];
            for (int i = 0; i < columns.Length; i++)
                columns[i] = table[i] + value;
            return new Table<T>(columns);
        }
        public static Table<T> operator +(T value, Table<T> table)
        {
            return table + value;
        }
    }
}