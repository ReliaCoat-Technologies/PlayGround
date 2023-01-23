using System;
using System.Collections.Generic;
using System.Linq;
using ReliaCoat.Common;

namespace EllipseDrawer.Utilities
{
    public static class MinimumAreaEnclosingEllipse
    {
        private static readonly Random _random = new Random();

        // Determine if a point is inside the circle.
        private static bool isPointInsideEllipse(DoubleEllipse2D inputEllipse, DoublePoint2D testPoint)
        {
            return inputEllipse.isPointWithinEllipse(testPoint);
        }

        private static DoubleEllipse2D getEllipseFrom(DoublePoint2D pointA, DoublePoint2D pointB)
        {
            var deltaX = pointB.X - pointA.X;
            var deltaY = pointB.Y - pointA.Y;

            var angleRadians = Math.Atan2(deltaX, deltaY);

            var radiusMajor = Math.Sqrt(deltaX * deltaX + deltaY + deltaY);
            var radiusMinor = radiusMajor / 10;

            var midpointX = (pointB.X + pointA.X) / 2;
            var midpointY = (pointB.Y + pointA.Y) / 2;

            return new DoubleEllipse2D(radiusMajor, radiusMinor, midpointX, midpointY, angleRadians);
        }

        private static DoubleEllipse2D getEllipseFrom(DoublePoint2D pointA, DoublePoint2D pointB, DoublePoint2D pointC)
        {
            return DoubleEllipse2D.generateSteinerCircumellipse(pointA, pointB, pointC);
        }

        private static bool isValidEllipse(DoubleEllipse2D ellipse, DoublePoint2D[] points)
        {
            foreach (var point in points)
            {
                if (!ellipse.isPointWithinEllipse(point))
                {
                    return false;
                }
            }

            return true;
        }

        // "Lazy" O(N^3) method.
        public static DoubleEllipse2D getSteinerEllipseLazy(IEnumerable<DoublePoint2D> pointsInput)
        {
            var points = pointsInput.ToArray();

            var currentEllipse = new DoubleEllipse2D(double.PositiveInfinity, double.PositiveInfinity, 0, 0, 0);

            var pointsToTry = points
                .permute(points)
                .permute(points)
                .Where(x => x.Distinct().Count() == 3)
                .ToArray();

            foreach (var pointToTry in pointsToTry)
            {
                var pointArray = pointToTry.ToArray();

                var ellipse = getEllipseFrom(pointArray[0], pointArray[1], pointArray[2]);

                if (isValidEllipse(ellipse, points) && currentEllipse.area > ellipse.area)
                {
                    currentEllipse = ellipse;
                }
            }

            return currentEllipse;
        }

        private static DoubleEllipse2D getMinimumEllipse(DoublePoint2D[] points)
        {
            if (points.Length == 0)
            {
                return new DoubleEllipse2D(0, 0, 0, 0, 0);
            }
            if (points.Length == 1)
            {
                return new DoubleEllipse2D(0, 0, points[0].X, points[0].Y, 0);
            }
            if (points.Length == 2)
            {
                return getEllipseFrom(points[0], points[1]);
            }

            for (var i = 0; i < 3; i++)
            {
                for (var j = i + 1; j < 3; j++)
                {
                    var ellipse = getEllipseFrom(points[i], points[j]);

                    if (isValidEllipse(ellipse, points))
                    {
                        return ellipse;
                    }
                }
            }

            return getEllipseFrom(points[0], points[1], points[2]);
        }

        private static DoubleEllipse2D getWelzlEllipseHelper(DoublePoint2D[] inputPoints, IList<DoublePoint2D> boundaryPoints, int numberOfPointsLeft)
        {
            if (numberOfPointsLeft == 0 || boundaryPoints.Count == 3)
            {
                return getMinimumEllipse(boundaryPoints.ToArray());
            }

            var index = _random.Next() % numberOfPointsLeft;
            var point = inputPoints[index];

            (inputPoints[index], inputPoints[numberOfPointsLeft - 1]) = (inputPoints[numberOfPointsLeft - 1], inputPoints[index]);

            var circle = getWelzlEllipseHelper(inputPoints, boundaryPoints, numberOfPointsLeft - 1);

            if (isPointInsideEllipse(circle, point))
            {
                return circle;
            }

            boundaryPoints.Add(point);

            return getWelzlEllipseHelper(inputPoints, boundaryPoints, numberOfPointsLeft - 1);
        }

        public static DoubleEllipse2D getSteinerCurcumellipse(IEnumerable<DoublePoint2D> inputPoints)
        {
            var points = inputPoints.shuffle().ToArray();

            return getWelzlEllipseHelper(points, new List<DoublePoint2D>(), points.Length);
        }
    }
}