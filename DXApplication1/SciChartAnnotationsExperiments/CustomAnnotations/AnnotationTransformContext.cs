using System;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public class AnnotationTransformContext
    {
        #region Properties
        public double beforeX1 { get; private set; }
        public double beforeX2 { get; private set; }
        public double beforeY1 { get; private set; }
        public double beforeY2 { get; private set; }
        public double afterX1 { get; private set; }
        public double afterX2 { get; private set; }
        public double afterY1 { get; private set; }
        public double afterY2 { get; private set; }
        public double deltaX1 { get; private set; }
        public double deltaX2 { get; private set; }
        public double deltaY1 { get; private set; }
        public double deltaY2 { get; private set; }
        public double XScale { get; private set; }
        public double YScale { get; private set; }
        #endregion

        public void setBeforeTransform(IComparable X1, IComparable X2, IComparable Y1, IComparable Y2)
        {
            beforeX1 = Convert.ToDouble(X1);
            beforeX2 = Convert.ToDouble(X2);
            beforeY1 = Convert.ToDouble(Y1);
            beforeY2 = Convert.ToDouble(Y2);
        }

        public void setAfterTransform(IComparable X1, IComparable X2, IComparable Y1, IComparable Y2)
        {
            afterX1 = Convert.ToDouble(X1);
            afterX2 = Convert.ToDouble(X2);
            afterY1 = Convert.ToDouble(Y1);
            afterY2 = Convert.ToDouble(Y2);
        }

        public void calculateTransformValues()
        {
            deltaX1 = afterX1 - beforeX1;
            deltaX2 = afterX2 - beforeX2;
            deltaY1 = afterY1 - beforeY1;
            deltaY2 = afterY2 - beforeY2;

            XScale = (afterX2 - afterX1) / (beforeX2 - beforeX1);
            YScale = (afterY2 - afterY1) / (afterX2 - afterX1);
        }
    }
}