using System;
using System.Linq;

namespace EllipseDrawer.Utilities
{
    public static class MatrixGenerator
    {
        public static TValue[,] createMatrixFromColumns<TValue>(params TValue[][] columns)
        {
            var numColumns = columns.Length;

            if (numColumns == 0)
            {
                throw new ArgumentException("Matrix must have at least 1 column.");
            }

            int numRows;

            try
            {
                numRows = columns
                    .Select(x => x.Length)
                    .Distinct()
                    .Single();
            }
            catch (InvalidOperationException ioe)
            {
                throw new ArgumentException("Array lengths of columns must be equal.", ioe);
            }

            if (numRows == 0)
            {
                throw new ArgumentException("Number of rows must be greater than 0.");
            }

            var matrix = new TValue[numRows, numColumns];

            for (var j = 0; j < numColumns; j++)
            {
                var column = columns[j];

                for (var i = 0; i < numRows; i++)
                {
                    matrix[i, j] = column[i];
                }
            }

            return matrix;
        }
    }
}