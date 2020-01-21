using System;
using System.Windows;
using SciChart.Charting.Visuals.Annotations;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class EllipseAnnotation : BoxAnnotation
    {
        #region
        private ModifiableEllipse _modifiableEllipse;
        #endregion

        #region Constructor
        public EllipseAnnotation()
        {
            InitializeComponent();
            Tag = "Drawn Annotation";
            IsEditable = true;
        }
        #endregion

        #region Methods
        private void onEllipseLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _modifiableEllipse = sender as ModifiableEllipse;

            if (_modifiableEllipse == null) return;

            _modifiableEllipse.ellipseRendered += renderModifiedEllipse;
        }

        private void renderModifiedEllipse(double widthRatio, double heightRatio)
        {
            var x1 = Convert.ToDouble(X1);
            var x2 = Convert.ToDouble(X2);
            var y1 = Convert.ToDouble(Y1);
            var y2 = Convert.ToDouble(Y2);

            var xMidpoint = (x2 + x1) / 2;
            var yMidpoint = (y2 + y1) / 2;

            var xHalfRange = Math.Abs((x2 - x1) / 2);
            var yHalfRange = Math.Abs((y2 - y1) / 2);

            var adjustedXHalfRange = xHalfRange * widthRatio;
            var adjustedYHalfRange = yHalfRange * heightRatio;

            X1 = xMidpoint - adjustedXHalfRange;
            X2 = xMidpoint + adjustedXHalfRange;
            Y1 = yMidpoint - adjustedYHalfRange;
            Y2 = yMidpoint + adjustedYHalfRange;
        }
        #endregion
    }
}
