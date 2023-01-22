using System;
using System.Linq;
using System.Security.Cryptography;

namespace SmallestEnclosedEllipse
{
    public struct DoubleConic2D
    {
        public double A { get; }
        public double B { get; }
        public double C { get; }
        public double D { get; }
        public double E { get; }
        public double F { get; }

        public double radiusMajor { get; }
        public double radiusMinor { get; }
        public double centerX { get; }
        public double centerY { get; }
        public double angle { get; }

        public DoubleConic2D(DoublePoint2D point1, DoublePoint2D point2, DoublePoint2D point3, DoublePoint2D point4, DoublePoint2D point5)
        {
            var xVector = new[]
            {
                point1.X,
                point2.X,
                point3.X,
                point4.X,
                point5.X,
            };

            var yVector = new[]
            {
                point1.Y,
                point2.Y,
                point3.Y,
                point4.Y,
                point5.Y,
            };

            var x2Vector = xVector
                .Zip(xVector, (x1, x2) => x1 * x2)
                .ToArray();

            var y2Vector = yVector
                .Zip(yVector, (y1, y2) => y1 * y2)
                .ToArray();

            var xyVector = xVector
                .Zip(yVector, (x, y) => x * y)
                .ToArray();

            var cVector = new[]
            {
                1.0,
                1.0,
                1.0,
                1.0,
                1.0
            };

            // Get the ellipse formula coefficients.
            var AMatrix = MatrixGenerator.createMatrixFromColumns(xyVector, y2Vector, xVector, yVector, cVector);

            A = DeterminantFunctions.getDeterminant(AMatrix);

            var BMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, y2Vector, xVector, yVector, cVector);

            B = -1 * DeterminantFunctions.getDeterminant(BMatrix);

            var CMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, xVector, yVector, cVector);

            C = DeterminantFunctions.getDeterminant(CMatrix);

            var DMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, yVector, cVector);

            D = -1 * DeterminantFunctions.getDeterminant(DMatrix);

            var EMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, xVector, cVector);

            E = DeterminantFunctions.getDeterminant(EMatrix);

            var FMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, xVector, yVector);

            F = -1 * DeterminantFunctions.getDeterminant(FMatrix);

            // Get the canoncial form parameters.
            var conicDeterminant = B * B - 4 * A * C;

            var a = A * E * E;
            var b = C * D * D;
            var c = B * D * E;
            var d = conicDeterminant * F;
            var e = a + b - c + d;

            var f = A + C;
            var g = (A - C) * (A - C) + B * B;
            var h = Math.Sqrt(g);
            var i1 = f + h;
            var i2 = f - h;

            var j1 = 2 * e * i1;
            var k1 = -1 * Math.Sqrt(j1);
            var l1 = k1 / conicDeterminant;

            var j2 = 2 * e * i2;
            var k2 = -1 * Math.Sqrt(j2);
            var l2 = k2 / conicDeterminant;

            radiusMajor = Math.Max(l1, l2);
            radiusMinor = Math.Min(l1, l2);

            var m = 2 * C * D - B * E;
            centerX = m / conicDeterminant;

            var n = 2 * A * E - B * D;
            centerY = n / conicDeterminant;

            var o = C - A - h;
            var p = o / B;
            angle = Math.Atan(p) * 180 / Math.PI;
        }
    }
}