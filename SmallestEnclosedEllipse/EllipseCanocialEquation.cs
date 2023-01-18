using System.Collections;
using System.Collections.Generic;

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
    }

    public class EllipseCanocialEquation
    {
        // Formula: r*x^2 + s*y^2 + t*x*y + u*x + v*y + w = 0; 
        public void getEllipseFormula(IEnumerable<Point> pointsInput)
        {
            double r, s, t, u, v, w;
            

        }
    }
}