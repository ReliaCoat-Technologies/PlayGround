using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AccordFrameworkImageAnalysis.Utilities
{
	public class RectangleResizeAdorner : Adorner
	{
		#region Constants
		private const double adornerSize = 20;
		#endregion

		#region Fields
		private readonly VisualCollection _children;
		private readonly Thumb _topLeft, _top, _topRight, _right, _bottomRight, _bottom, _bottomLeft, _left, _center;
		#endregion

		#region Delegates
		public event EventHandler<AdornerMouseButtonEventArgs> dragStarted;
		public event EventHandler<AdornerMouseMoveEventArgs> dragMove;
		public event EventHandler<AdornerMouseButtonEventArgs> dragCompleted;
		#endregion

		#region Properties
		protected override int VisualChildrenCount => _children.Count;
		#endregion
		
		#region Constructor
		public RectangleResizeAdorner(UIElement adornedElement) : base(adornedElement)
		{
			_children = new VisualCollection(this);

			buildAdorner(ref _center, ResizePosition.Center);

			buildAdorner(ref _top, ResizePosition.Top);
			buildAdorner(ref _bottom, ResizePosition.Bottom);
			buildAdorner(ref _left, ResizePosition.Left);
			buildAdorner(ref _right, ResizePosition.Right);

			buildAdorner(ref _topLeft, ResizePosition.TopLeft);
			buildAdorner(ref _topRight, ResizePosition.TopRight);
			buildAdorner(ref _bottomRight, ResizePosition.BottomRight);
			buildAdorner(ref _bottomLeft, ResizePosition.BottomLeft);
		}
		#endregion

		#region Methods
		protected override void OnRender(DrawingContext drawingContext)
		{
			var rect = new Rect(AdornedElement.DesiredSize);

			var paddedX0 = getMaxDouble(rect.X + adornerSize, rect.Height);
			var paddedY0 = getMaxDouble(rect.Y + adornerSize, rect.Width);
			var paddedX1 = getMinDouble(rect.Width - adornerSize, 0);
			var paddedY1 = getMinDouble(rect.Height - adornerSize, 0);

			var paddedRect = new Rect(new Point(paddedX0, paddedY0), new Point(paddedX1, paddedY1));

			_topLeft.Arrange(new Rect(0, 0, adornerSize, adornerSize));
			_topRight.Arrange(new Rect(paddedRect.Right, 0, adornerSize, adornerSize));
			_bottomLeft.Arrange(new Rect(0, paddedRect.Bottom, adornerSize, adornerSize));
			_bottomRight.Arrange(new Rect(paddedRect.Right, paddedRect.Bottom, adornerSize, adornerSize));

			_top.Arrange(new Rect(paddedRect.X, 0, paddedRect.Width, adornerSize));
			_left.Arrange(new Rect(0, paddedRect.Y, adornerSize, paddedRect.Height));
			_bottom.Arrange(new Rect(adornerSize, paddedRect.Bottom, paddedRect.Width, adornerSize));
			_right.Arrange(new Rect(paddedRect.Right, adornerSize, adornerSize, paddedRect.Height));

			_center.Arrange(paddedRect);
		}

		private double getMaxDouble(double value, double max)
		{
			return value > max ? max : value;
		}

		private double getMinDouble(double value, double min)
		{
			return value < min ? min : value;
		}

		private void buildAdorner(ref Thumb thumb, ResizePosition position)
		{
			if (thumb != null) return;

			Cursor cursor;
			SolidColorBrush bgBrush;

			switch(position)
			{
				case ResizePosition.Center:
					cursor = Cursors.SizeAll;
					bgBrush = Brushes.White;
					break;
				case ResizePosition.Top:
				case ResizePosition.Bottom:
					cursor = Cursors.SizeNS;
					bgBrush = Brushes.Black;
					break;
				case ResizePosition.Left:
				case ResizePosition.Right:
					cursor = Cursors.SizeWE;
					bgBrush = Brushes.Black;
					break;
				case ResizePosition.TopLeft:
				case ResizePosition.BottomRight:
					cursor = Cursors.SizeNWSE;
					bgBrush = Brushes.Gray;
					break;
				case ResizePosition.TopRight:
				case ResizePosition.BottomLeft:
					cursor = Cursors.SizeNESW;
					bgBrush = Brushes.Gray;
					break;
				default:
					throw new Exception("Invalid cursor assignment");
			}

			thumb = new Thumb
			{
				Cursor = cursor,
				BorderThickness = new Thickness(0),
				Opacity = 0.2,
				Background = bgBrush,
			};

			thumb.PreviewMouseDown += (s, e) =>
			{
				dragStarted?.Invoke(this, new AdornerMouseButtonEventArgs(position, e));
			};

			thumb.PreviewMouseMove += (s, e) =>
			{
				dragMove?.Invoke(this, new AdornerMouseMoveEventArgs(position, e));
			};

			thumb.PreviewMouseUp += (s, e) =>
			{
				dragCompleted?.Invoke(this, new AdornerMouseButtonEventArgs(position, e));
			};

			_children.Add(thumb);
		}

		protected override Visual GetVisualChild(int index)
		{
			return _children[index];
		}
		#endregion
	}
}
