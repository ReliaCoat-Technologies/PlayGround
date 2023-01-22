using System;

namespace EllipseDrawer.Utilities
{
    public static class DeterminantFunctions
    {
        public static double getDeterminant(double[,] inputMatrix)
        {
            var width = inputMatrix.GetLength(1);

            validateInputMatrix(inputMatrix);

            if (width == 2)
            {
                return getTwoByTwoDeterminant(inputMatrix);
            }

            var determinantResult = 0.0;

            for (var i = 0; i < width; i++)
            {
                var determinantFactor = inputMatrix[0, i];
                
                // Cofactoring not needed if the current factor is 0;
                if (determinantFactor == 0.0)
                {
                    continue;
                }

                // Get the determinant cofactor double[width -1, width - 1]
                var determinantCoFactor = getDeterminantCofactorMatrix(inputMatrix, i);

                if (i % 2 == 1)
                {
                    determinantResult -= determinantFactor * getDeterminant(determinantCoFactor);
                }
                else
                {
                    determinantResult += determinantFactor * getDeterminant(determinantCoFactor);
                }
            }

            return determinantResult;
        }

        public static double[,] getDeterminantCofactorMatrix(double[,] inputMatrix, int xIndex)
        {
            var width = inputMatrix.GetLength(1);

            var cofactorMatrix = new double[width - 1, width - 1];

            for (var i = 0; i < width - 1; i++)
            {
                var returnMatrixXIndex = i < xIndex ? i : i + 1;

                for (var j = 0; j < width - 1; j++)
                {
                    // C# matrix notation is [vertical,horizontal]
                    cofactorMatrix[j, i] = inputMatrix[j + 1, returnMatrixXIndex];
                }
            }

            return cofactorMatrix;
        }

        public static double getTwoByTwoDeterminant(double[,] inputMatrix)
        {
            if (inputMatrix.GetLength(0) != 2 || inputMatrix.GetLength(1) != 2)
            {
                throw new ArgumentException("inputMatrix must be 2 x 2");
            }

            return inputMatrix[0, 0] * inputMatrix[1, 1] - inputMatrix[1, 0] * inputMatrix[0, 1];
        }

        public static void validateInputMatrix(double[,] inputMatrix)
        {
            var height = inputMatrix.GetLength(0);
            var width = inputMatrix.GetLength(1);

            if (height < 2)
            {
                throw new ArgumentException("Matrix height must be 2 or greater.");
            }

            if (width < 2)
            {
                throw new ArgumentException("Matrix width must be 2 or greater.");
            }

            if (width != height)
            {
                throw new ArgumentException($"Matrix input size is {width} X {height}. Dimensions must be equal.");
            }
        }
    }
}