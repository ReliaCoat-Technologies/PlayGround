using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReliaCoat.Common;

namespace SmallestEnclosedEllipse
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"X = {X}, Y = {Y}";
        }
    }

    public struct Circle
    {
        public Point center { get; set; }
        public double radius { get; set; }

        public Circle(Point center, double radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public override string ToString()
        {
            return $"Center: {center.ToString()}, R = {radius}";
        }
    }

    public class MinimumAreaEnclosingCircle
    {
        private Random _random;

        public MinimumAreaEnclosingCircle()
        {
            _random = new Random();
        }

        // Returns Pythagorean distance between two points.
        public double getDistance(Point pointA, Point pointB)
        {
            return Math.Sqrt(Math.Pow(pointA.X - pointB.X, 2) + Math.Pow(pointA.Y - pointB.Y, 2));
        }

        // Determine if a point is inside the circle.
        public bool isPointInsideCircle(Circle inputCircle, Point testPoint)
        {
            return getDistance(inputCircle.center, testPoint) <= inputCircle.radius;
        }

        public Point getCircleCenter(double deltaABX, double deltaABY, double deltaACX, double deltaACY)
        {
            var B = deltaABX * deltaABX + deltaABY * deltaABY;
            var C = deltaACX * deltaACX + deltaACY * deltaACY;
            var D = deltaABX * deltaACY - deltaABY * deltaACX;

            var centerX = (deltaACY * B - deltaABY * C) / (2 * D);
            var centerY = (deltaABX * C - deltaACX * B) / (2 * D);

            return new Point(centerX, centerY);
        }

        public Circle getCircleFrom(Point pointA, Point pointB, Point pointC)
        {
            var deltaABX = pointB.X - pointA.X;
            var deltaABY = pointB.Y - pointA.Y;
            var deltaACX = pointC.X - pointA.X;
            var deltaACY = pointC.Y - pointA.Y;

            var circleCenterPoint = getCircleCenter(deltaABX, deltaABY, deltaACX, deltaACY);

            circleCenterPoint.X += pointA.X;
            circleCenterPoint.Y += pointA.Y;

            var radius = getDistance(circleCenterPoint, pointA);

            return new Circle(circleCenterPoint, radius);
        }

        public Circle getCircleFrom(Point pointA, Point pointB)
        {
            var centerPoint = new Point((pointA.X + pointB.X) / 2, (pointA.Y + pointB.Y) / 2);

            var radius = getDistance(pointA, pointB) / 2;

            return new Circle(centerPoint, radius);
        }

        public bool isValidCircle(Circle testCircle, Point[] points)
        {
            foreach (var point in points)
            {
                if (!isPointInsideCircle(testCircle, point))
                {
                    return false;
                }
            }

            return true;
        }

        // "Lazy" O(N^3) method.
        public Circle getWelzlCircleLazy(IEnumerable<Point> pointsInput)
        {
            var points = pointsInput.ToArray();

            var currentCircle = new Circle(new Point(0, 0), double.PositiveInfinity);

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

        public Circle getMinimumCircle(Point[] points)
        {
            if (points.Length == 0)
            {
                return new Circle(new Point(0, 0), 0);
            }
            if (points.Length == 1)
            {
                return new Circle(points[0], 1);
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

        public Circle getWelzlCircleHelper(Point[] inputPoints, IList<Point> boundaryPoints, int numberOfPointsLeft)
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

        public Circle getWelzlCircle(IEnumerable<Point> inputPoints)
        {
            var points = inputPoints.shuffle().ToArray();

            return getWelzlCircleHelper(points, new List<Point>(), points.Length);
        }
    }
}