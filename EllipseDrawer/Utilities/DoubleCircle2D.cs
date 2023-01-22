namespace EllipseDrawer.Utilities
{
    public struct DoubleCircle2D
    {
        public DoublePoint2D center { get; set; }
        public double radius { get; set; }

        public DoubleCircle2D(DoublePoint2D center, double radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public override string ToString()
        {
            return $"Center: {center.ToString()}, R = {radius}";
        }
    }
}