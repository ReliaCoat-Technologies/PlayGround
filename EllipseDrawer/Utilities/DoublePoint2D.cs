namespace EllipseDrawer.Utilities
{
    public struct DoublePoint2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public DoublePoint2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"X = {X}, Y = {Y}";
        }

        public static DoublePoint2D operator +(DoublePoint2D point2DLeft, DoublePoint2D point2DRight)
        {
            return new DoublePoint2D(point2DLeft.X + point2DRight.X, point2DLeft.Y + point2DRight.Y);
        }

        public static DoublePoint2D operator -(DoublePoint2D point2DLeft, DoublePoint2D point2DRight)
        {
            return new DoublePoint2D(point2DLeft.X - point2DRight.X, point2DLeft.Y - point2DRight.Y);
        }
    }
}