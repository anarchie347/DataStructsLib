using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public struct TableCoord
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public TableCoord(int row, int column)
        {
            Row = row;
            Column = column;
        }
        public TableCoord Reverse()
        {
            return new TableCoord(this.Column, this.Row);
        }
        public bool Validate()
        {
            return this.Row >= 0
                && this.Column >= 0;
        }
        public bool Validate(int rowMax, int colMax)
        {
            return this.Row < rowMax
                && this.Row >= 0
                && this.Column < colMax
                && this.Column >= 0;
        }
        public override string ToString()
        {
            return $"({this.Row}, {this.Column})";
        }
    }
}
