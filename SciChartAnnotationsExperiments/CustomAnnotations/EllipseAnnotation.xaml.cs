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
		private double _beforeTransformX1;
		private double _beforeTransformX2;
		private double _beforeTransformY1;
		private double _beforeTransformY2;

		private double _widthRatio;
		private double _heightRatio;

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

			_modifiableEllipse.modifiableEllipseShown += setModifiableEllipseSize;
			_modifiableEllipse.renderedPathShown += resetOriginalBoundingBoxes;
			_modifiableEllipse.rotationCompleted += updateNewAngleBoundingBox;
		}

		private void onDragStart(object sender, EventArgs e)
		{
			// Occurs original dimensions have not yet been set.
			if (new[] { _beforeTransformX1, _beforeTransformY1, _beforeTransformX2, _beforeTransformY2 }.All(x => x == 0))
			{
				_beforeTransformX1 = Convert.ToDouble(X1);
				_beforeTransformX2 = Convert.ToDouble(X2);
				_beforeTransformY1 = Convert.ToDouble(Y1);
				_beforeTransformY2 = Convert.ToDouble(Y2);
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

			var adjustedOriginDimensions = _transformContext.getAdjustedWidthHeight(_beforeTransformX2 - _beforeTransformX1, _beforeTransformY2 - _beforeTransformY1);

			var adjustedWidth = adjustedOriginDimensions[0];
			var adjustedHeight = adjustedOriginDimensions[1];

			var _xMidpoint = (_beforeTransformX1 + _beforeTransformX2 + 2 * _transformContext.deltaXMidpoint) / 2;
			var _yMidpoint = (_beforeTransformY1 + _beforeTransformY2 + 2 * _transformContext.deltaYMidpoint) / 2;

			_beforeTransformX1 = _xMidpoint - adjustedWidth / 2;
			_beforeTransformX2 = _xMidpoint + adjustedWidth / 2;
			_beforeTransformY1 = _yMidpoint - adjustedHeight / 2;
			_beforeTransformY2 = _yMidpoint + adjustedHeight / 2;

			Console.WriteLine($"Original Transform: [X1:{_beforeTransformX1} X2:{_beforeTransformX2} Y1:{_beforeTransformY1} Y2:{_beforeTransformY2}]");
		}

		private void setModifiableEllipseSize()
		{
			if (new[] { _beforeTransformX1, _beforeTransformY1, _beforeTransformX2, _beforeTransformY2 }.All(x => x == 0)) return;

			// Sets box size to un-transformed ellipse size for rotation.
			X1 = _beforeTransformX1;
			X2 = _beforeTransformX2;
			Y1 = _beforeTransformY1;
			Y2 = _beforeTransformY2;
		}

		private void updateNewAngleBoundingBox(double widthRatio, double heightRatio)
		{
			_widthRatio = widthRatio;
			_heightRatio = heightRatio;
		}

		private void resetOriginalBoundingBoxes()
		{
			// Stores the un-transformed ellipses bounds
			_beforeTransformX1 = Convert.ToDouble(X1);
			_beforeTransformX2 = Convert.ToDouble(X2);
			_beforeTransformY1 = Convert.ToDouble(Y1);
			_beforeTransformY2 = Convert.ToDouble(Y2);

			// Transformation is centered around the ellipse's midpoint
			var xMidpoint = (_beforeTransformX2 + _beforeTransformX1) / 2;
			var yMidpoint = (_beforeTransformY2 + _beforeTransformY1) / 2;

			var xHalfRange = Math.Abs((_beforeTransformX2 - _beforeTransformX1) / 2);
			var yHalfRange = Math.Abs((_beforeTransformY2 - _beforeTransformY1) / 2);

			var adjustedXHalfRange = xHalfRange * _widthRatio;
			var adjustedYHalfRange = yHalfRange * _heightRatio;

			// Applying the adjusted ellipse's bound corners.
			X1 = xMidpoint - adjustedXHalfRange;
			X2 = xMidpoint + adjustedXHalfRange;
			Y1 = yMidpoint - adjustedYHalfRange;
			Y2 = yMidpoint + adjustedYHalfRange;
		}
		#endregion
	}
}
