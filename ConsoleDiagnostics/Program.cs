using System;
using System.Diagnostics;
using EllipseDrawer.Utilities;
using ReliaCoat.Numerics.CartesianMath;

namespace ConsoleDiagnostics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var points = new[]
            {
                new XyPoint(1, 1),
                new XyPoint(1, -1),
                new XyPoint(-1, -1),
                new XyPoint(-1, 1),
                new XyPoint(-2, 0),
            };

            var points2 = new[]
            {
                new XyPoint(-1, 1),
                new XyPoint(1, 1),
                new XyPoint(-1, -1),
                new XyPoint(-2, 0),
                new XyPoint(1, -1),
            };

            var sw = new Stopwatch();
            sw.Start();

            var ellipse = DoubleEllipse2D.generateEllipse(points);

            sw.Stop();

            var sameVector = XyPoint.doArraysHaveSamePoints(points, points2);

            Console.WriteLine($"4-point ellipse Calculation Time: {sw.Elapsed.TotalMilliseconds} ms");

            Console.ReadKey();
        }
    }
}
