using System;

namespace SmallestEnclosedEllipse
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var points1 = new[]
            {
                new Point(5, -2),
                new Point(-3, -2),
                new Point(-2, 5),
                new Point(1, 6),
                new Point(0, 2),
            };

            // var result = new MinimumAreaEnclosingCircle().getWelzlCircleLazy(points1);
            var result = new MinimumAreaEnclosingCircle().getWelzlCircle(points1);

            Console.ReadLine();
        }
    }
}
