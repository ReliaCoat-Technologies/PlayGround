using System;
using System.Windows;
using DevExpress.XtraExport.Xls;
using SciChart.Charting.Visuals.Annotations;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class EllipseAnnotation : BoxAnnotation
    {
        #region
        private ModifiableEllipse _modifiableEllipse;
        private double _x1;
        private double _x2;
        private double _y1;
        private double _y2;
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

            SizeChanged += onSizeChanged;
            _modifiableEllipse.revertEllipseSize += revertBoxSize;
            _modifiableEllipse.ellipseRendered += renderModifiedEllipse;
        }

        private void onSizeChanged(object s, SizeChangedEventArgs e)
        {
            _modifiableEllipse?.onParentSizeChanged(e.NewSize);
        }

        private void revertBoxSize()
        {
            if (_x1 == 0) return;

            X1 = _x1;
            X2 = _x2;
            Y1 = _y1;
            Y2 = _y2;
        }

        private void renderModifiedEllipse(double widthRatio, double heightRatio)
        {
            _x1 = Convert.ToDouble(X1);
            _x2 = Convert.ToDouble(X2);
            _y1 = Convert.ToDouble(Y1);
            _y2 = Convert.ToDouble(Y2);

            var xMidpoint = (_x2 + _x1) / 2;
            var yMidpoint = (_y2 + _y1) / 2;

            var xHalfRange = Math.Abs((_x2 - _x1) / 2);
            var yHalfRange = Math.Abs((_y2 - _y1) / 2);

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
