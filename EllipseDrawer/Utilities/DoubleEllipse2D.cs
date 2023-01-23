using System;
using System.Diagnostics;
using System.Linq;

namespace EllipseDrawer.Utilities
{
    public struct DoubleEllipse2D
    {
        #region Properties
        /// <summary>
        /// Longer radius of the ellipse. (Unitless)
        /// </summary>
        public double radiusMajor { get; }
        /// <summary>
        /// Shorter radius of the ellipse. (Unitless)
        /// </summary>
        public double radiusMinor { get; }
        /// <summary>
        /// Central X coordinate on cartesian plane.
        /// </summary>
        public double centerX { get; }
        /// <summary>
        /// Central Y coordinate on cartesian plane.
        /// </summary>
        public double centerY { get; }
        /// <summary>
        /// Rotation, counter-clockwise from X-Axis in positive direction, of ellipse, in radians.
        /// </summary>
        public double angleRadians { get; }
        /// <summary>
        /// Rotation, counter-clockwise from X-Axis in positive direction, of ellipse, in degrees.
        /// </summary>
        public double angleDegrees => angleRadians * 180 / Math.PI;
        /// <summary>
        /// Unitless area of the ellipse.
        /// </summary>
        public double area => Math.PI * radiusMajor * radiusMinor;
        #endregion

        #region Constructors
        public DoubleEllipse2D(double radiusMajor, double radiusMinor, double centerX, double centerY, double angleRadians)
        {
            this.radiusMajor = radiusMajor;
            this.radiusMinor = radiusMinor;
            this.centerX = centerX;
            this.centerY = centerY;
            this.angleRadians = angleRadians;
        }
        #endregion

        #region Methods
        public bool isPointWithinEllipse(DoublePoint2D testPoint)
        {
            // Get distance of test point from center of ellipse.
            var centeredX = testPoint.X - centerX;
            var centeredY = testPoint.Y - centerY;

            // Rotate point by ellipse's angle (in reverse direction, CW).
            var rotatedX = centeredX * Math.Cos(-angleRadians) - centeredY * Math.Sin(-angleRadians);
            var rotatedY = centeredX * Math.Sin(-angleRadians) + centeredY * Math.Cos(-angleRadians);
            
            // Scale point to ellipse's major and minor radii.
            var normalizedX = rotatedX / radiusMajor;
            var normalizedY = rotatedY / radiusMinor;
            
            // Check if pythagorean normalized distance is greater than the radius of the normalized circle.
            var distanceFromCenter = Math.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
            return distanceFromCenter <= 1;
        }

        public (double, double) getCartesianCoordinatesForAngle(double inputAngleRadians)
        {
            // Get the cartesian coordinates for an ellipses
            // Based on the angle on the transformed coordinates.
            var x = centerX
                    + radiusMajor * Math.Cos(inputAngleRadians) * Math.Cos(angleRadians)
                    - radiusMinor * Math.Sin(inputAngleRadians) * Math.Sin(angleRadians);

            var y = centerY
                    + radiusMajor * Math.Cos(inputAngleRadians) * Math.Sin(angleRadians)
                    + radiusMinor * Math.Sin(inputAngleRadians) * Math.Cos(angleRadians);

            return (x, y);
        }

        public DoublePoint2D getCartesianPointForAngle(double inputAngleRadians)
        {
            var (x, y) = getCartesianCoordinatesForAngle(inputAngleRadians);

            return new DoublePoint2D(x, y);
        }
        #endregion

        #region Static Generators
        public static DoubleEllipse2D generateFromGeneralCoefficients(double A, double B, double C, double D, double E, double F)
        {
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

            // Get Major and Minor Radii
            var j1 = 2 * e * i1;
            var k1 = -1 * Math.Sqrt(j1);
            var l1 = k1 / conicDeterminant;

            var j2 = 2 * e * i2;
            var k2 = -1 * Math.Sqrt(j2);
            var l2 = k2 / conicDeterminant;

            var radiusMajor = Math.Max(l1, l2);
            var radiusMinor = Math.Min(l1, l2);

            // Get Center Coordinates
            var m = 2 * C * D - B * E;
            var centerX = m / conicDeterminant;

            var n = 2 * A * E - B * D;
            var centerY = n / conicDeterminant;

            var o = C - A - h;
            var p = o / B;
            var angleRadians = Math.Atan(p);

            return new DoubleEllipse2D(radiusMajor, radiusMinor, centerX, centerY, angleRadians);
        }

        public static DoubleEllipse2D generateFromFivePoints(DoublePoint2D point1, DoublePoint2D point2, DoublePoint2D point3, DoublePoint2D point4, DoublePoint2D point5)
        {
            // Initialize vectors based on points.
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

            // Initialize secondary vectors.
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
            var A = DeterminantFunctions.getDeterminant(AMatrix);

            // A must always be positive. If not, multiply each coefficient by -1
            // Unproven, only determined by experimentation.
            var multiplier = A > 0 ? 1 : -1;
            A *= multiplier;

            var BMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, y2Vector, xVector, yVector, cVector);
            var B = -1 * multiplier * DeterminantFunctions.getDeterminant(BMatrix);

            var CMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, xVector, yVector, cVector);
            var C = multiplier * DeterminantFunctions.getDeterminant(CMatrix);

            var DMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, yVector, cVector);
            var D = -1 * multiplier * DeterminantFunctions.getDeterminant(DMatrix);

            var EMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, xVector, cVector);
            var E = multiplier * DeterminantFunctions.getDeterminant(EMatrix);

            var FMatrix = MatrixGenerator.createMatrixFromColumns(x2Vector, xyVector, y2Vector, xVector, yVector);
            var F = -1 * multiplier *  DeterminantFunctions.getDeterminant(FMatrix);

            Debug.WriteLine($"A = {A}, B = {B}, C = {C}, D = {D}, E = {E}, F = {F}");

            return generateFromGeneralCoefficients(A, B, C, D, E, F);
        }

        public static DoubleEllipse2D generateSteinerCircumellipse(DoublePoint2D point1, DoublePoint2D point2, DoublePoint2D point3)
        {
            var centerX = (point1.X + point2.X + point3.X) / 3;
            var centerY = (point1.Y + point2.Y + point3.Y) / 3;

            var point1DeltaX = point1.X - centerX;
            var point1DeltaY = point1.Y - centerY;

            var point4 = new DoublePoint2D(centerX - point1DeltaX, centerY - point1DeltaY);

            var point2DeltaX = point2.X - centerX;
            var point2DeltaY = point2.Y - centerY;

            var point5 = new DoublePoint2D(centerX - point2DeltaX, centerY - point2DeltaY);

            return generateFromFivePoints(point1, point2, point3, point4, point5);
        }
        #endregion    
    }
}