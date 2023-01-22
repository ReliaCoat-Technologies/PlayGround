using System;
using System.Collections.Generic;
using System.Linq;
using ReliaCoat.Common;

namespace EllipseDrawer.Utilities
{
    public class MinimumAreaEnclosingCircle
    {
        private Random _random;

        public MinimumAreaEnclosingCircle()
        {
            _random = new Random();
        }

        // Returns Pythagorean distance between two points.
        public double getDistance(DoublePoint2D doublePoint2DA, DoublePoint2D doublePoint2DB)
        {
            return Math.Sqrt(Math.Pow(doublePoint2DA.X - doublePoint2DB.X, 2) + Math.Pow(doublePoint2DA.Y - doublePoint2DB.Y, 2));
        }

        // Determine if a point is inside the circle.
        public bool isPointInsideCircle(DoubleCircle2D inputDoubleCircle2D, DoublePoint2D testDoublePoint2D)
        {
            return getDistance(inputDoubleCircle2D.center, testDoublePoint2D) <= inputDoubleCircle2D.radius;
        }

        public DoublePoint2D getCircleCenter(double deltaABX, double deltaABY, double deltaACX, double deltaACY)
        {
            var B = deltaABX * deltaABX + deltaABY * deltaABY;
            var C = deltaACX * deltaACX + deltaACY * deltaACY;
            var D = deltaABX * deltaACY - deltaABY * deltaACX;

            var centerX = (deltaACY * B - deltaABY * C) / (2 * D);
            var centerY = (deltaABX * C - deltaACX * B) / (2 * D);

            return new DoublePoint2D(centerX, centerY);
        }

        public DoubleCircle2D getCircleFrom(DoublePoint2D doublePoint2DA, DoublePoint2D doublePoint2DB, DoublePoint2D doublePoint2DC)
        {
            var deltaABX = doublePoint2DB.X - doublePoint2DA.X;
            var deltaABY = doublePoint2DB.Y - doublePoint2DA.Y;
            var deltaACX = doublePoint2DC.X - doublePoint2DA.X;
            var deltaACY = doublePoint2DC.Y - doublePoint2DA.Y;

            var circleCenterPoint = getCircleCenter(deltaABX, deltaABY, deltaACX, deltaACY);

            circleCenterPoint.X += doublePoint2DA.X;
            circleCenterPoint.Y += doublePoint2DA.Y;

            var radius = getDistance(circleCenterPoint, doublePoint2DA);

            return new DoubleCircle2D(circleCenterPoint, radius);
        }

        public void getEllipseFrom(DoublePoint2D doublePoint2DA, DoublePoint2D doublePoint2DB, DoublePoint2D doublePoint2DC)
        {
            var lengthA = getDistance(doublePoint2DB, doublePoint2DC);
            var lengthB = getDistance(doublePoint2DA, doublePoint2DC);
            var lengthC = getDistance(doublePoint2DA, doublePoint2DB);

            getEllipseFrom(lengthA, lengthB, lengthC);
        }

        public void getEllipseFrom(double lengthA, double lengthB, double lengthC)
        {
            var lengthASquared = lengthA * lengthA;
            var lengthBSquared = lengthB * lengthB;
            var lengthCSquared = lengthC * lengthC;

            var k1 = lengthASquared
                     + lengthBSquared
                     + lengthCSquared;

            var k2 = lengthASquared * lengthBSquared
                     + lengthBSquared * lengthCSquared
                     + lengthCSquared * lengthASquared;

            var k3 = lengthASquared * lengthASquared
                     + lengthBSquared * lengthBSquared
                     + lengthCSquared * lengthCSquared;

            var k4 = Math.Sqrt(k3 - k2);

            var k5 = k1 / Math.Sqrt(2 * k2 - k3);

            var anglePortion1 = Math.Sqrt(k5 * k5 - 3);
            var anglePortion2 = k5 - anglePortion1;
            var tanAngle = 1.0 / 3.0 * anglePortion2;

            var angle = Math.Atan(tanAngle) * 180 / Math.PI;

            var a = 1.0 / 3.0 * Math.Sqrt(k1 + 2 * k4);
            var b = 1.0 / 3.0 * Math.Sqrt(k1 - 2 * k4);
        }

        public DoubleCircle2D getCircleFrom(DoublePoint2D doublePoint2DA, DoublePoint2D doublePoint2DB)
        {
            var centerPoint = new DoublePoint2D((doublePoint2DA.X + doublePoint2DB.X) / 2, (doublePoint2DA.Y + doublePoint2DB.Y) / 2);

            var radius = getDistance(doublePoint2DA, doublePoint2DB) / 2;

            return new DoubleCircle2D(centerPoint, radius);
        }

        public bool isValidCircle(DoubleCircle2D testDoubleCircle2D, DoublePoint2D[] points)
        {
            foreach (var point in points)
            {
                if (!isPointInsideCircle(testDoubleCircle2D, point))
                {
                    return false;
                }
            }

            return true;
        }

        // "Lazy" O(N^3) method.
        public DoubleCircle2D getWelzlCircleLazy(IEnumerable<DoublePoint2D> pointsInput)
        {
            var points = pointsInput.ToArray();

            var currentCircle = new DoubleCircle2D(new DoublePoint2D(0, 0), double.PositiveInfinity);

            var pointsToTry = points
                .permute(points)
                .permute(points)
                .Where(x => x.Distinct().Count() == 3)
                .ToArray();

            foreach (var pointToTry in pointsToTry)
            {
                var pointArray = pointToTry.ToArray();

                var circle = getCircleFrom(pointArray[0], pointArray[1], pointArray[2]);

                if (isValidCircle(circle, points) && currentCircle.radius > circle.radius)
                {
                    currentCircle = circle;
                }
            }

            return currentCircle;
        }

        public DoubleCircle2D getMinimumCircle(DoublePoint2D[] points)
        {
            if (points.Length == 0)
            {
                return new DoubleCircle2D(new DoublePoint2D(0, 0), 0);
            }
            if (points.Length == 1)
            {
                return new DoubleCircle2D(points[0], 1);
            }
            if (points.Length == 2)
            {
                return getCircleFrom(points[0], points[1]);
            }

            for (var i = 0; i < 3; i++)
            {
                for (var j = i + 1; j < 3; j++)
                {
                    var circle = getCircleFrom(points[i], points[j]);

                    if (isValidCircle(circle, points))
                    {
                        return circle;
                    }
                }
            }

            return getCircleFrom(points[0], points[1], points[2]);
        }

        public DoubleCircle2D getWelzlCircleHelper(DoublePoint2D[] inputPoints, IList<DoublePoint2D> boundaryPoints, int numberOfPointsLeft)
        {
            if (numberOfPointsLeft == 0 || boundaryPoints.Count == 3)
            {
                return getMinimumCircle(boundaryPoints.ToArray());
            }

            var index = _random.Next() % numberOfPointsLeft;
            var point = inputPoints[index];

            (inputPoints[index], inputPoints[numberOfPointsLeft - 1]) = (inputPoints[numberOfPointsLeft - 1], inputPoints[index]);

            var circle = getWelzlCircleHelper(inputPoints, boundaryPoints, numberOfPointsLeft - 1);

            if (isPointInsideCircle(circle, point))
            {
                return circle;
            }

            boundaryPoints.Add(point);

            return getWelzlCircleHelper(inputPoints, boundaryPoints, numberOfPointsLeft - 1);
        }

        public DoubleCircle2D getWelzlCircle(IEnumerable<DoublePoint2D> inputPoints)
        {
            var points = inputPoints.shuffle().ToArray();

            return getWelzlCircleHelper(points, new List<DoublePoint2D>(), points.Length);
        }
    }
}