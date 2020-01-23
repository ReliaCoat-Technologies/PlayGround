using System;
using System.Collections.Generic;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public class AnnotationTransformContext
    {
        #region Properties
        // Before Values
        public double beforeX1 { get; private set; }
        public double beforeX2 { get; private set; }
        public double beforeY1 { get; private set; }
        public double beforeY2 { get; private set; }
        public double beforeXMidpoint { get; private set; }
        public double beforeYMidpoint { get; private set; }

        // After Values
        public double afterX1 { get; private set; }
        public double afterX2 { get; private set; }
        public double afterY1 { get; private set; }
        public double afterY2 { get; private set; }
        public double afterXMidpoint { get; private set; }
        public double afterYMidpoint { get; private set; }

        // Delta Values
        public double deltaX1 { get; private set; }
        public double deltaX2 { get; private set; }
        public double deltaY1 { get; private set; }
        public double deltaY2 { get; private set; }
        public double deltaXMidpoint { get; private set; }
        public double deltaYMidpoint { get; private set; }

        public double XScale { get; private set; }
        public double YScale { get; private set; }
        #endregion

        public void setBeforeTransform(IComparable X1, IComparable X2, IComparable Y1, IComparable Y2)
        {
            beforeX1 = Convert.ToDouble(X1);
            beforeX2 = Convert.ToDouble(X2);
            beforeY1 = Convert.ToDouble(Y1);
            beforeY2 = Convert.ToDouble(Y2);

            beforeXMidpoint = (beforeX2 + beforeX1) / 2;
            beforeYMidpoint = (beforeY2 + beforeY1) / 2;
        }

        public void setAfterTransform(IComparable X1, IComparable X2, IComparable Y1, IComparable Y2)
        {
            afterX1 = Convert.ToDouble(X1);
            afterX2 = Convert.ToDouble(X2);
            afterY1 = Convert.ToDouble(Y1);
            afterY2 = Convert.ToDouble(Y2);

            afterXMidpoint = (afterX1 + afterX2) / 2;
            afterYMidpoint = (afterY1 + afterY2) / 2;
        }

        public void calculateTransformValues()
        {
            deltaX1 = afterX1 - beforeX1;
            deltaX2 = afterX2 - beforeX2;
            deltaY1 = afterY1 - beforeY1;
            deltaY2 = afterY2 - beforeY2;

            deltaXMidpoint = afterXMidpoint - beforeXMidpoint;
            deltaYMidpoint = afterYMidpoint - beforeYMidpoint;

            XScale = (afterX2 - afterX1) / (beforeX2 - beforeX1);
            YScale = (afterY2 - afterY1) / (beforeY2 - beforeY1);
        }

        public IList<double> getAdjustedWidthHeight(double width, double height)
        {
            var adjustedWidth = Math.Abs(width) * XScale;
            var adjustedHeight = Math.Abs(height) * YScale;

            return new List<double> { adjustedWidth, adjustedHeight };
        }
    }
}