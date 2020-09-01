using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AccordFrameworkImageAnalysis.Utilities;
using AccordFrameworkImageAnalysis.ViewModels;

namespace AccordFrameworkImageAnalysis.Views
{
	public partial class ImageCropDialogView : UserControl
	{
		#region Fields
		private bool _isDragging;
		private Point _startPoint;
		private Point _currentPoint;
		private double _xMin, _yMin, _xMax, _yMax;
		private double _x0, _x1, _y0, _y1;
		private double _scaledX0, _scaledX1, _scaledY0, _scaledY1;
		private Rect _shapeRect;
		private RectangleResizeAdorner _adorner;
		#endregion

		#region Properties
		public ImageCropDialogViewModel viewModel { get; private set; }
		#endregion

		#region Constructor
		public ImageCropDialogView()
		{
			Focusable = true;

			InitializeComponent();

			grid.PreviewMouseDown += onImageMouseDown;
			grid.PreviewMouseMove += onImageMouseMove;
			grid.PreviewMouseLeftButtonUp += onImageMouseUp;

			PreviewKeyDown += OnPreviewKeyDown;
		}
		#endregion

		#region Methods
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			var pointMin = image.TransformToAncestor(grid).Transform(new Point(0, 0));

			_xMin = 0;
			_yMin = 0;
			_xMax = _xMin + image.ActualWidth;
			_yMax = _yMin + image.ActualHeight;

			_x0 = _scaledX0 * (_xMax - _xMin);
			_y0 = _scaledY0 * (_yMax - _yMin);
			_x1 = _scaledX1 * (_xMax - _xMin);
			_y1 = _scaledY1 * (_yMax - _yMin);

			drawShapeFromPoints();
		}

		public void setViewModel(ImageCropDialogViewModel viewModelInput)
		{
			viewModel = viewModelInput;
			DataContext = viewModel;
		}

		private void onImageMouseDown(object sender, MouseButtonEventArgs e)
		{
			_isDragging = true;
			_startPoint = e.GetPosition(image);
		}

		private void onAdornerDragStarted(object sender, AdornerMouseButtonEventArgs e)
		{
			_isDragging = true;
			_startPoint = e.GetPosition(image);
		}

		private void onImageMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			if (!_isDragging) return;

			_currentPoint = e.GetPosition(image);

			_x0 = _startPoint.X < _currentPoint.X ? _startPoint.X : _currentPoint.X;
			_x1 = _startPoint.X < _currentPoint.X ? _currentPoint.X : _startPoint.X;
			_y0 = _startPoint.Y < _currentPoint.Y ? _startPoint.Y : _currentPoint.Y;
			_y1 = _startPoint.Y < _currentPoint.Y ? _currentPoint.Y : _startPoint.Y;

			drawShapeFromPoints();
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);

			Vector delta;

			var increment = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
				? 10
				: 1;

			switch (e.Key)
			{
				case (Key.Up):
					delta = new Vector(0, -increment);
					break;
				case (Key.Down):
					delta = new Vector(0, increment);
					break;
				case (Key.Left):
					delta = new Vector(-increment, 0);
					break;
				case (Key.Right):
					delta = new Vector(increment, 0);
					break;
				default:
					return;
			}

			shiftRectangle(delta);
			drawShapeFromPoints();

			appendPoints();
		}

		private void onAdornerDragMove(object sender, AdornerMouseMoveEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed) return;
			if (!_isDragging) return;

			_currentPoint = e.GetPosition(image);

			switch (e.position)
			{
				case ResizePosition.Center:
					var delta = _currentPoint - _startPoint;
					shiftRectangle(delta);
					_startPoint = _currentPoint;
					break;
				case ResizePosition.Top:
					_y0 = setValue(_currentPoint.Y, _yMin, _y1);
					break;
				case ResizePosition.TopRight:
					_x1 = setValue(_currentPoint.X, _x0, _xMax);
					_y0 = setValue(_currentPoint.Y, _yMin, _y1);
					break;
				case ResizePosition.Right:
					_x1 = setValue(_currentPoint.X, _x0, _xMax);
					break;
				case ResizePosition.BottomRight:
					_x1 = setValue(_currentPoint.X, _x0, _xMax);
					_y1 = setValue(_currentPoint.Y, _y0, _yMax);
					break;
				case ResizePosition.Bottom:
					_y1 = setValue(_currentPoint.Y, _y0, _yMax);
					break;
				case ResizePosition.BottomLeft:
					_x0 = setValue(_currentPoint.X, _xMin, _x1);
					_y1 = setValue(_currentPoint.Y, _y0, _yMax);
					break;
				case ResizePosition.Left:
					_x0 = setValue(_currentPoint.X, _xMin, _x1);
					break;
				case ResizePosition.TopLeft:
					_x0 = setValue(_currentPoint.X, _xMin, _x1);
					_y0 = setValue(_currentPoint.Y, _yMin, _y1);
					break;
			}

			drawShapeFromPoints();
		}

		private void shiftRectangle(Vector delta)
		{
			if (_x0 + delta.X > _xMin && _x1 + delta.X < _xMax)
			{
				_x0 = setValue(_x0 + delta.X, _xMin, _x1);
				_x1 = setValue(_x1 + delta.X, _x0, _xMax);
			}
			else if(_x0 + delta.X <= _xMin)
			{
				var deltaX = _x0 - _xMin;
				_x0 -= deltaX;
				_x1 -= deltaX;
			}
			else if (_x1 + delta.X >= _xMax)
			{
				var deltaX = _xMax - _x1;
				_x0 += deltaX;
				_x1 += deltaX;
			}

			if (_y0 + delta.Y > _yMin && _y1 + delta.Y < _yMax)
			{
				_y0 = setValue(_y0 + delta.Y, _yMin, _y1);
				_y1 = setValue(_y1 + delta.Y, _y0, _yMax);
			}
			else if(_y0 + delta.Y <= _yMin)
			{
				var deltaY = _y0 - _yMin;
				_y0 -= deltaY;
				_y1 -= deltaY;
			}
			else if (_y1 + delta.Y >= _yMax)
			{
				var deltaY = _yMax - _y1;
				_y0 += deltaY;
				_y1 += deltaY;
			}
		}

		public double setValue(double inputValue, double minValue, double maxValue)
		{
			if (inputValue > maxValue) return maxValue;
			else if (inputValue < minValue) return minValue;
			else return inputValue;
		}

		private void drawShapeFromPoints()
		{
			_shapeRect = new Rect(_x0, _y0, Math.Abs(_x1 - _x0), Math.Abs(_y1 - _y0));

			var displacementVector = _shapeRect.TopLeft - _shapeRect.BottomRight;

			if (displacementVector.Length < 5) return;

			Canvas.SetLeft(cropRect, _shapeRect.Left);
			Canvas.SetTop(cropRect, _shapeRect.Top);

			cropRect.Width = _shapeRect.Width;
			cropRect.Height = _shapeRect.Height;

			_scaledX0 = (_x0 - _xMin) / (_xMax - _xMin);
			_scaledX1 = (_x1 - _xMin) / (_xMax - _xMin);
			_scaledY0 = (_y0 - _yMin) / (_yMax - _yMin);
			_scaledY1 = (_y1 - _yMin) / (_yMax - _yMin);
		}

		private void onImageMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!_isDragging) return;

			_isDragging = false;
			addAdorner();

			appendPoints();
		}

		private void onDragCompleted(object sender, AdornerMouseButtonEventArgs e)
		{
			_isDragging = false;

			appendPoints();
		}

		private void appendPoints()
		{
			Console.WriteLine($"Points: {_x0}, {_y0}, {_x1}, {_y1}");
			Console.WriteLine($"Scales: {_scaledX0}, {_scaledY0}, {_scaledX1}, {_scaledY1}");

			viewModel.setCropDimensions(_scaledX0, _scaledY0, _scaledX1, _scaledY1);
		}

		private void addAdorner()
		{
			var adornerLayers = AdornerLayer.GetAdornerLayer(cropRect);

			if (adornerLayers.GetAdorners(cropRect)?.Any() ?? false) return;

			_adorner = new RectangleResizeAdorner(cropRect);
			subscribeAdorners();
			adornerLayers.Add(_adorner);
		}

		private void removeAdorner()
		{
			if (_adorner == null) return;

			var adornerLayers = AdornerLayer.GetAdornerLayer(cropRect);
			adornerLayers.Remove(_adorner);
		}

		private void subscribeAdorners()
		{
			_adorner.dragStarted += onAdornerDragStarted;
			_adorner.dragMove += onAdornerDragMove;
			_adorner.dragCompleted += onDragCompleted;
		}
		#endregion
	}
}
