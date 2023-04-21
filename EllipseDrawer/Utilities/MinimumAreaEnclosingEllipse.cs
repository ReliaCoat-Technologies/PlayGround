using System;
using System.Collections.Generic;
using System.Linq;
using ReliaCoat.Numerics.CartesianMath;

namespace EllipseDrawer.Utilities
{
    public static class MinimumAreaEnclosingEllipse
    {
        // "Lazy" O(N^3) method.
        public static DoubleEllipse2D getAllEncompassingEllipseLazy(IEnumerable<XyPoint> pointsInput)
        {
            var points = pointsInput.ToArray();

            var hullPoints = ConvexHullCalculator.getConvexHull(points);

            if (hullPoints.Count == 3)
            {
                return DoubleEllipse2D.generateFromThreePoints(hullPoints[0], hullPoints[1], hullPoints[2]);
            }

            if (hullPoints.Count == 4)
            {
                return DoubleEllipse2D.generateFromFourPoints(hullPoints[0], hullPoints[1], hullPoints[2], hullPoints[3]);
            }

            var fivePointEllipses = getDistinctEllipses(hullPoints, 5).ToArray();

            if (!fivePointEllipses.Any())
            {
                return new DoubleEllipse2D(0, 0, 0, 0, 0);
            }

            var minArea = fivePointEllipses.Min(x => x.area);

            return fivePointEllipses.FirstOrDefault(x => x.area == minArea);
        }

        private static IEnumerable<DoubleEllipse2D> getDistinctEllipses(this IEnumerable<XyPoint> pointsInput, int numberOfPoints)
        {
            var points = pointsInput.ToArray();

            if (numberOfPoints < 3)
            {
                throw new ArgumentException("Number of points per ellipse must exceed 3");
            }
            if (numberOfPoints > 5)
            {
                throw new ArgumentException("Number of points per ellipse must not exceed 5");
            }

            var permutedPoints = new List<IList<XyPoint>>();

            for (var i = 0; i < points.Length; i++)
            {
                for (var j = i; j < points.Length; j++)
                {
                    if (points[i] == points[j])
                    {
                        continue;
                    }

                    var pointSet = new List<XyPoint> { points[i], points[j] };

                    permutedPoints.Add(pointSet);
                }
            }

            var ellipsesToReturn = new List<DoubleEllipse2D>();

            for (var t = 3; t <= numberOfPoints; t++)
            {
                var newPermutedPoints = new List<IList<XyPoint>>();

                foreach (var pointSet in permutedPoints)
                {
                    foreach (var point in points)
                    {
                        if (pointSet.Contains(point))
                        {
                            continue;
                        }

                        var newPointSet = pointSet.ToList();
                        newPointSet.Add(point);

                        if (t == numberOfPoints)
                        {
                            var ellipse = DoubleEllipse2D.generateEllipse(newPointSet);
                            
                            if (ellipse.arePointsWithinEllipse(points))
                            {
                                ellipsesToReturn.Add(ellipse);
                            }
                        }
                        else
                        {
                            newPermutedPoints.Add(newPointSet);
                        }
                    }
                }

                permutedPoints = newPermutedPoints;
            }

            return ellipsesToReturn;
        }
    }
}