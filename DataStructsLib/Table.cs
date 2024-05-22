using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public class Table<T> : IStringableAndCloneable<T>
    {
        private readonly ShiftingResizingArray<ShiftingResizingArray<T>> tableRows;

        public int Length1D { get { return tableRows.Length; } }
        public ResizingArray<T> this[int row]
        {
           get
            {
                if (row < 0 || row >= tableRows.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(row));
                }
                return tableRows[row];
            }
        }
        public T this[int row, int col]
        {
            get { return this[row][col]; }
            set { this[row][col] = value; }
        }
        public T this[TableCoord coord]
        {
            get
            {
                if (!coord.Validate(Length1D, Length1D))
                {
                    throw new ArgumentOutOfRangeException(nameof(coord));
                }
                return tableRows[coord.Row][coord.Column];
            }
            set
            {
                if (!coord.Validate(Length1D, Length1D))
                {
                    throw new ArgumentOutOfRangeException(nameof(coord));
                }
                tableRows[coord.Row][coord.Column] = value;
            }
        }

        public Table()
        {
            tableRows = new ShiftingResizingArray<ShiftingResizingArray<T>>(0);
        }
        public Table(int size)
        {
            tableRows = new ShiftingResizingArray<ShiftingResizingArray<T>>(size);
            for (int i = 0; i < size; i++)
            {
                tableRows[i] = new ShiftingResizingArray<T>(i);
            }
        }

        public void AddRowColumn(T? fillerValue = default(T))
        {
            if (fillerValue != null)
            {
                tableRows.IncreaseSizeAdditive(1, new ShiftingResizingArray<T>(Enumerable.Repeat(fillerValue, Length1D).ToArray()));
            } else
            {
                tableRows.IncreaseSizeAdditive(1);
            }
          
            for (int i = 0; i < tableRows.Length; i++)
            {
                tableRows[i].IncreaseSizeAdditive(1, fillerValue);
            }
        }
        public void AddRowColumn(Func<T> constructor)
        {
            tableRows.IncreaseSizeAdditive(1, new ShiftingResizingArray<T>(Array.ConvertAll(new int[Length1D], i => constructor())));

            for (int i = 0; i < tableRows.Length; i++)
            {
                tableRows[i].IncreaseSizeAdditive(1, constructor());
            }
        }

        public void RemoveRowColumn(int rowColumn)
        {
            if (rowColumn < 0 || rowColumn >= tableRows.Length)
            {
                return;
            }
            tableRows.RemoveAt(rowColumn);
            for (int i = 0; i < tableRows.Length; i++)
            {
                tableRows[i].RemoveAt(rowColumn);
            }
        }


        public string Stringify(Func<T, string> transform)
        {
            if (Length1D == 0)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < tableRows.Length - 1; i++)
            {
                sb.Append(tableRows[i].Stringify(false, transform) + ", ");
            }
            sb.Append(tableRows[tableRows.Length - 1].Stringify(false, transform) + "]");
            return sb.ToString();
        }
        public string Stringify(bool withNewLines, Func<T, string> transform)
        {
            if (!withNewLines)
            {
                return this.Stringify(transform);
            }
            if (Length1D == 0)
            {
                return "[\n]";
            }
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < tableRows.Length - 1; i++)
            {
                sb.Append("\n\t" + tableRows[i].Stringify(false, transform) + ",");
            }
            sb.Append("\n\t" + tableRows[tableRows.Length - 1].Stringify(false, transform) + "\n]");
            return sb.ToString();
        }
        public override string ToString()
        {
            return this.Stringify(true);
        }
        public object Clone()
        {
            Table<T> copy = new Table<T>(this.Length1D);
            for (int i = 0; i < this.Length1D; i++)
            {
                copy.tableRows[i] = new ShiftingResizingArray<T>(this.tableRows[i].ToArray());
            }
            return copy;
        }

    }
}

