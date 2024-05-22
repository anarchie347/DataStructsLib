using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public struct Matrix : IStringableDataStruct<double>
    {
        public const int RoundingErrorDigitCutoff = 8; //used to eliminate floating point rounding errors by approximating the results
        private double[,] matrix; //[row,column]


        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public double this[int row, int column]
        {
            get
            {
                return this[new TableCoord(row, column)];
            }
            set
            {
                this[new TableCoord(row, column)] = value;
            }
        }
        public double this[TableCoord coord]
        {
            get
            {
                if (!coord.Validate(RowCount, ColumnCount))
                {
                    throw new ArgumentException(nameof(coord));
                }
                return matrix[coord.Row,coord.Column];
            }
            set
            {
                if (!coord.Validate(RowCount, ColumnCount))
                {
                    throw new ArgumentException(nameof(coord));
                }
                matrix[coord.Row,coord.Column] = value;
            }
        }
        public Matrix(int rowCount, int columnCount)
        {
            if (rowCount < 1) {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }
            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowCount));
            }
            RowCount = rowCount;
            ColumnCount = columnCount;
            matrix = new double[rowCount, columnCount];
        }
        public Matrix(double[,] matrixAsArray)
        { 
            RowCount = matrixAsArray.GetLength(0);
            ColumnCount = matrixAsArray.GetLength(1);
            matrix = (double[,])matrixAsArray.Clone();
        }
        //Returns the identity matrix of a given size
        public static Matrix Identity(int size)
        {
            double[,] matrix = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                matrix[i, i] = 1;
            }
            return new Matrix(matrix);
        }

        public bool IsSquare()
        {
            return RowCount == ColumnCount;
        }
        public bool IsSingular(int roundingCutoff = RoundingErrorDigitCutoff)
        {
            return Math.Round(this.Determinant(), roundingCutoff) == 0;
        }
        public double Determinant()
        {
            if (!IsSquare())
            {
                throw new Exception("Non square matrices do not have a determinant");
            }
            //Matrix is square, so can just use RowCount for size

            //1x1
            //for a 1x1 matrix, the determinant is the value in the matrix
            if (RowCount == 1)
            {
                return matrix[0, 0];
            }

            //for 2x2 and bigger
            //The determinant of an upper triangular matrix is the product of the diagonal elements
            (Matrix matrix, bool DeterminantSwapSign)? result = this.ToUpperTriangular();
            if (result is null) // no upper triangular matrix exists => determinant is 0
            {
                return 0;
            }

            Matrix uppertriangular = result.Value.matrix;
            int sign = result.Value.DeterminantSwapSign ? -1 : 1;
            double total = 1;
            for (int i = 0; i < uppertriangular.RowCount; i++)
            {
                total *= uppertriangular[i, i];
            }
            return total * sign;
        }

        //swap rows and columns
        public Matrix Transpose()
        {
            double[,] newMatrix = new double[ColumnCount, RowCount];
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    newMatrix[j,i] = matrix[i,j];
                }
            }
            return new Matrix(newMatrix);
        }
        //multiply each term by either +1 or -1 based on the below checkerboard pattern, with the top left being +1
        public Matrix Cofactor()
        {
            
            double[,] newMatrix = new double[RowCount, ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                    else
                    {
                        newMatrix[i, j] = -matrix[i, j];
                    }
                }
            }
            return new Matrix(newMatrix);
        }
        //swap each matrix element with the determinant of the correspsonding minor matrix
        public Matrix Minors()
        {
            
            double[,] newMatrix = new double[this.RowCount, this.ColumnCount];
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    newMatrix[i, j] = this.GetSingleMinor(i, j).Determinant();
                }
            }
            return new Matrix(newMatrix);
        }
        public Matrix Adjugate()
        {
            return this.Minors().Cofactor().Transpose();
        }
        public Matrix Inverse()
        {
            //matrix * inverse = identity matrix.
            //for an inverse to exist, the matrix must be square and not singular
            if (!this.IsSquare())
            {
                throw new Exception("Matrix is not square, no inverse exists");
            }
            if (this.IsSingular())
            {
                throw new Exception("Matrix is singular, no inverse exists");
            }
            return this.Adjugate() * (1 / this.Determinant());
        }
        //Calculates the Moor-Penrose inverse of the matrix
        public Matrix PseudoInverse()
        {
            Matrix transpose = this.Transpose();
            Matrix multByTranspose = transpose * this;
            if (multByTranspose.IsSingular())
            {
                throw new Exception("M multiplied by transpose is singular, no pseduoinverse exists");
            }
            Matrix inverse = multByTranspose.Inverse();
            Matrix ans = inverse * transpose;
            return ans;
        }
        public Matrix GetSingleMinor(int row, int column)
        {
            return GetSingleMinor(new TableCoord(row, column));
        }
        public Matrix GetSingleMinor(TableCoord coord)
        {
            if (!coord.Validate(this.RowCount, this.ColumnCount))
            {
                throw new Exception("Coordinate was not inside the matrix");
            }
            //when a row and column are crossed out, the remaining items in the matrix form the minor matrix for the position where the crossed out row and column meet
            Matrix minor = new Matrix(this.ColumnCount - 1, this.RowCount - 1);
            for (int row = 0; row < RowCount; row++)
            {
                for (int column = 0; column < ColumnCount; column++)
                {
                    int newRow = row;
                    int newColumn = column;
                    if (row == coord.Row) //adjusts i based on where it is compared to the removed row
                    {
                        newRow = -1;
                    }
                    else if (row > coord.Row)
                    {
                        newRow--;
                    }

                    if (column == coord.Column)//adjusts j based on where it is compared to the removed column
                    {
                        newColumn = -1;
                    }
                    else if (column > coord.Column)
                    {
                        newColumn--;
                    }
                    if (newRow > -1 && newColumn > -1) //if in the removed row or column, then i or j will be -1
                    {
                        minor[newRow, newColumn] = this[row,column];
                    }
                }
            }
            return minor;
        }

        //Attempts to convert the matrix to upper triangular form and specifies if the determinant has been multiplied by -1
        public (Matrix matrix, bool determinantSwapSign)? ToUpperTriangular()
        {
            //a matrix in the form where all entries below/left of the main diagonal are 0
            if (!this.IsSquare())
            {
                throw new Exception("Only square matrices can be converted to upper triangular form");
            }
            if (this.RowCount == 1) //a 1x1 matrix is already in upper triangular form
            {
                return (new Matrix((double[,])this.matrix.Clone()), false);
            }

            double[,] uppertriangular = (double[,])this.matrix.Clone();
            bool swapSign = false; //for every row swap made, the determinant is multiplied by -1
            

            for (int row = 0; row < this.ColumnCount - 1; row++)
            {
                int maxAbsValueIndex = -1;
                double maxAbsValue = -1;
                for (int i = row; i < this.RowCount; i++)
                {
                    double currentAbsValue = Math.Abs(uppertriangular[i, row]);
                    if (currentAbsValue > maxAbsValue)
                    {
                        maxAbsValue = currentAbsValue;
                        maxAbsValueIndex = i;
                    }
                } //finds the value and index and the maximum magnitude element remaining in the column
                if (Math.Round(maxAbsValue, RoundingErrorDigitCutoff) == 0)
                {
                    return null; //determinant = 0, no upper triangular form exists;
                }
                if (maxAbsValueIndex != (row)) //swap so that the row with the largest magnitude value  in the given column is worked on
                {
                    SwapRow(ref uppertriangular, row, maxAbsValueIndex);
                    swapSign = !swapSign;
                }

                for (int i = row + 1; i < this.RowCount; i++) //foreach remaining row below the working row
                {
                    double scaleFactor = -uppertriangular[i, row] / uppertriangular[row, row];
                    RowOperation(ref uppertriangular, i, row, scaleFactor);
                }
            }
            return (new Matrix(uppertriangular), swapSign);
        }
        //adds a scalar multiple of one row to another
        private static void RowOperation(ref double[,] matrix, int editingRow, int addingRow, double scale)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[editingRow, j] += matrix[addingRow, j] * scale;
            }
        }
        private static void SwapRow(ref double[,] matrix, int row1, int row2)
        {
            for (int j = 0; j < matrix.GetLength(1); j++) //foreach column
            {
                (matrix[row1, j], matrix[row2, j]) = (matrix[row2, j], matrix[row1, j]); //swap values
            }
        }
        //attempts to eliminate rounding errors by rounding all elements in the matrix
        //due to lots of arithmetic with matrix calculations, floating point errors are common
        public Matrix EliminateRoundingErrors(int roundingCutOff = RoundingErrorDigitCutoff)
        {
            double[,] newMatrix = new double[RowCount, ColumnCount];
            for (int row = 0; row < this.RowCount; row++)
            {
                for (int column = 0; column < this.ColumnCount; column++)
                {
                    double val = matrix[row, column];
                    val = Math.Round(val, roundingCutOff);
                    if (val == 0D) //Math.Round can produce a value of -0, which should be changed to 0
                    {
                        val = 0;
                    }
                    newMatrix[row, column] = val;
                }
            }
            return new Matrix(newMatrix);
        }

        public bool CanMultiply(Matrix right)
        {
            return this.ColumnCount == right.RowCount;
        }
        public bool CanAdd(Matrix right)
        {
            return this.ColumnCount == right.ColumnCount && this.RowCount == right.RowCount;
        }
        //returns a tuple containing the size of the result matrix if two matrices were multiplied together
        public (int rowCount, int columnCount) MultiplySize(Matrix right)
        {
            if (!this.CanMultiply(right))
            {
                throw new ArgumentException($"{this.RowCount}x{this.ColumnCount} matrix cannot be multiplied by {right.RowCount}x{right.ColumnCount} matrix");
            }
            return (this.RowCount, right.ColumnCount);
        }

        public string Stringify(Func<double, string> transform)
        {
            if (RowCount == 0 || ColumnCount == 0)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.RowCount; i++)
            {
                sb.Append("[");
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    sb.Append("  ");
                    sb.Append(transform(matrix[i,j]));
                }
                sb.Append("  ]");
            }
            sb.Remove(sb.Length - 1, 1); //remove trailing newline
            return sb.ToString();

        }
        public string Stringify(bool withNewLines, Func<double, string> transform)
        {
            if (!withNewLines)
            {
                return this.Stringify(transform);
            }
            if (RowCount == 0 || ColumnCount == 0)
            {
                return "[\n]";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.RowCount; i++)
            {
                sb.Append("[");
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    sb.Append("  ");
                    sb.Append(transform(matrix[i,j]));
                }
                sb.Append("  ]\n");
            }      
            sb.Remove(sb.Length - 1, 1); //remove trailing newline
            return sb.ToString();
        }
        public override string ToString()
        {
            return this.Stringify(true);
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            if (!(left.RowCount == right.RowCount && left.ColumnCount == right.ColumnCount))
            {
                return false;
            }
            for (int i = 0; i < left.RowCount; i++)
            {
                for (int j = 0; j < left.RowCount; j++)
                {
                    if (left[i,j] != right[i,j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool operator !=(Matrix left, Matrix right)
        {
            return !(left == right);
        }
        public static Matrix operator *(double scalar, Matrix m)
        {
            return m * scalar;
        }
        public static Matrix operator *(Matrix m, double scalar)
        {
            //matrix scalar multiplication
            double[,] newMatrix = new double[m.RowCount, m.ColumnCount];
            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    newMatrix[i,j] = scalar * m[i,j];
                }
            }
            return new Matrix(newMatrix);
        }
        public static Matrix operator *(Matrix left, Matrix right)
        {

            (int resultRowCount, int resultColumnCount) = left.MultiplySize(right);
            double[,] result = new double[resultRowCount, resultColumnCount];

            double tempTotal;
            for (int i = 0; i < left.RowCount; i++)
            {
                for (int j = 0; j < right.ColumnCount; j++) //calculates the dot product of the vector from the row of the left matrix and the column of the second matrix
                {
                    //for each cell at [row = i][col = j] in the resulting matrix
                    tempTotal = 0;
                    for (int k = 0; k < left.ColumnCount; k++) //would be the same using right.RowCount as these must be equal
                    {
                        tempTotal += left[i, k] * right[k, j];
                    }
                    result[i, j] = tempTotal; //store result
                }
            }
            return new Matrix(result);
        }

        public static double[] SolveSystemOfEquations(double[,] expressions, double[] equalTo, bool eliminateRoundingErrors) // [equationNo, value]
        {
            Matrix expressionMatrix = new(expressions);
            Matrix equalToMatrix = new Matrix(SpreadAcross2D(equalTo));
            Matrix inverse = expressionMatrix.IsSquare() ?
                expressionMatrix.Inverse() : expressionMatrix.PseudoInverse(); //finds the inverse or Moore-Penrose inverse, depending on if the system is overloaded or not
            Matrix resultsMatrix =  inverse * equalToMatrix;
            if (eliminateRoundingErrors)
            {
                resultsMatrix = resultsMatrix.EliminateRoundingErrors();
            }
            return CollapseTo1D(resultsMatrix);

        }
        //converts a 1d array to a 2d array with value [i] stored in [i,0]
        private static double[,] SpreadAcross2D(double[] array)
        {
            double[,] toReturn = new double[array.Length, 1];
            for (int i = 0; i < array.Length; i++)
            {
                toReturn[i,0] = array[i];
            }
            return toReturn;
        }
        //converts a single column matrix to a 1d array
        private static double[] CollapseTo1D(Matrix m)
        {
            double[] toReturn = new double[m.RowCount];
            for (int i = 0; i < m.RowCount; i++)
            {
                toReturn[i] = m[i, 0];
            }
            return toReturn;
        }

    }
}
