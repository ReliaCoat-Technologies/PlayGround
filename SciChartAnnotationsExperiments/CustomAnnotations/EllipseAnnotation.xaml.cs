using System;
using System.Linq;
using System.Windows;
using SciChart.Charting.Visuals.Annotations;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class EllipseAnnotation : BoxAnnotation
    {
        #region
        // Untranslated annotation dimensions.
        private double _x1;
        private double _x2;
        private double _y1;
        private double _y2;

        private ModifiableEllipse _modifiableEllipse;
        private readonly AnnotationTransformContext _transformContext;
        #endregion

        #region Constructor
        public EllipseAnnotation()
        {
            InitializeComponent();
            Tag = "Drawn Annotation";
            ClipToBounds = false;
            IsEditable = true;
            IsResizable = true;

            _transformContext = new AnnotationTransformContext();
        }
        #endregion

        #region Methods
        private void onEllipseLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // Sets modifiable ellipse property from XAML template upon runtime loading.
            _modifiableEllipse = sender as ModifiableEllipse;

            if (_modifiableEllipse == null) return;

            SizeChanged += (s, e) => _modifiableEllipse.onParentSizeChanged(e.NewSize);
            
            DragStarted += onDragStart;
            DragEnded += onDragEnded;

            _modifiableEllipse.rotationStarted += revertBoxSize;
            _modifiableEllipse.rotationCompleted += renderModifiedEllipse;
        }

        private void onDragStart(object sender, EventArgs e)
        {
            // Occurs original dimensions have not yet been set.
            if (new[] { _x1, _y1, _x2, _y2 }.All(x => x == 0))
            {
                _x1 = Convert.ToDouble(X1);
                _x2 = Convert.ToDouble(X2);
                _y1 = Convert.ToDouble(Y1);
                _y2 = Convert.ToDouble(Y2);
            }

            // Takes down bounds before transforming.
            _transformContext.setBeforeTransform(X1, X2, Y1, Y2);
        }

        private void onDragEnded(object sender, EventArgs e)
        {
            // Takes down bounds after transforming.
            _transformContext.setAfterTransform(X1, X2, Y1, Y2);

            // Calculates transformation parameters.
            _transformContext.calculateTransformValues();

            Console.WriteLine($"Shift: Horizontal:{_transformContext.deltaXMidpoint}, Vertical:{_transformContext.deltaYMidpoint}");
            Console.WriteLine($"Scale: Horizontal:{_transformContext.XScale}, Vertical:{_transformContext.YScale}");

            var adjustedOriginDimensions = _transformContext.getAdjustedWidthHeight(_x2 - _x1, _y2 - _y1);

            var adjustedWidth = adjustedOriginDimensions[0];
            var adjustedHeight = adjustedOriginDimensions[1];

            var _xMidpoint = (_x1 + _x2 + 2 * _transformContext.deltaXMidpoint) / 2;
            var _yMidpoint = (_y1 + _y2 + 2 * _transformContext.deltaYMidpoint) / 2;

            _x1 = _xMidpoint - adjustedWidth / 2;
            _x2 = _xMidpoint + adjustedWidth / 2;
            _y1 = _yMidpoint - adjustedHeight / 2;
            _y2 = _yMidpoint + adjustedHeight / 2;

            Console.WriteLine($"Original Transform: [X1:{_x1} X2:{_x2} Y1:{_y1} Y2:{_y2}]");
        }

        private void revertBoxSize()
        {
            if (new[] { _x1, _y1, _x2, _y2 }.All(x => x == 0)) return;

            // Sets box size to un-transformed ellipse size for rotation.
            X1 = _x1;
            X2 = _x2;
            Y1 = _y1;
            Y2 = _y2;
        }

        private void renderModifiedEllipse(double widthRatio, double heightRatio)
        {
            // Stores the un-transformed ellipses bounds
            _x1 = Convert.ToDouble(X1);
            _x2 = Convert.ToDouble(X2);
            _y1 = Convert.ToDouble(Y1);
            _y2 = Convert.ToDouble(Y2);

            // Transformation is centered around the ellipse's midpoint
            var xMidpoint = (_x2 + _x1) / 2;
            var yMidpoint = (_y2 + _y1) / 2;
            
            var xHalfRange = Math.Abs((_x2 - _x1) / 2);
            var yHalfRange = Math.Abs((_y2 - _y1) / 2);

            var adjustedXHalfRange = xHalfRange * widthRatio;
            var adjustedYHalfRange = yHalfRange * heightRatio;

            // Applying the adjusted ellipse's bound corners.
            X1 = xMidpoint - adjustedXHalfRange;
            X2 = xMidpoint + adjustedXHalfRange;
            Y1 = yMidpoint - adjustedYHalfRange;
            Y2 = yMidpoint + adjustedYHalfRange;
        }
        #endregion
    }
}
