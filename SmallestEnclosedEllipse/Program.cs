using System;

namespace SmallestEnclosedEllipse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var points = new[]
            {
                new DoublePoint2D(-6, -2),
                new DoublePoint2D(6, -2),
                new DoublePoint2D(3, 7),
                new DoublePoint2D(-1, -5),
                new DoublePoint2D(-4, 4),
            };

            var ellipse = new DoubleConic2D(points[0], points[1], points[2], points[3], points[4]);

            // new MinimumAreaEnclosingCircle().getEllipseFrom(3, 4, 5);

            /*
            var matrix = new [,]
            {
                { 3.0, 2.0, 0.0, 2.0 }, 
                { 1.0, 2.0, 5.0, 3.0 }, 
                { 1.0, 0.0, 6.0, 4.0 }, 
                { 5.0, 6.0, 7.0, 8.0 }, 
            };

            var result = DeterminantFunctions.getDeterminant(matrix);

            Console.WriteLine("Determinant Result: {0}", result);
            */

            Console.ReadKey();
        }
    }
}
