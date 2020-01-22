using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class ModifiableEllipse : UserControl
    {
        #region Fields
        private bool _dragAction;
        private Point _ellipseOrigin;
        private Vector _originalVector;
        private double _originalAngle;
        private RotateTransform _rotateTransform;
        private bool _isTransformed => ellipse.Visibility != Visibility.Visible;
        #endregion

        #region Delegates
        public event Action revertEllipseSize;
        public event Action<double, double> ellipseRendered;
        #endregion

        #region Constructor
        public ModifiableEllipse()
        {
            InitializeComponent();

            ellipse.MouseRightButtonDown += onRightClickEllipse;
            renderedPath.MouseRightButtonDown += onRightClickEllipse;

            _rotateTransform = new RotateTransform(0);

            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            ellipse.RenderTransform = _rotateTransform;

            ellipse.Visibility = Visibility.Visible;
            renderedPath.Visibility = Visibility.Hidden;
            angleLine.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Methods
        public void onParentSizeChanged(Size e)
        {
            _ellipseOrigin = new Point(e.Width / 2, e.Height / 2);
        }

        private void onRightClickEllipse(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MouseMove += updateEllipseAngle;
            MouseRightButtonUp += onRightButtonUp;

            revertEllipseSize?.Invoke();

            ellipse.Visibility = Visibility.Visible;
            renderedPath.Visibility = Visibility.Hidden;
            angleLine.Visibility = Visibility.Visible;

            _dragAction = true;
        }

        private void updateEllipseAngle(object sender, MouseEventArgs e)
        {
            if (!_dragAction) return;

            if (Mouse.RightButton != MouseButtonState.Pressed) return;

            var mousePosition = e.GetPosition(border);

            angleLine.X1 = _ellipseOrigin.X;
            angleLine.Y1 = _ellipseOrigin.Y;
            angleLine.X2 = mousePosition.X;
            angleLine.Y2 = mousePosition.Y;

            var deltaX = mousePosition.X - _ellipseOrigin.X;
            var deltaY = mousePosition.Y - _ellipseOrigin.Y;

            var angle = Math.Atan(deltaY / deltaX) * 180 / Math.PI;

            _rotateTransform.Angle = angle;
        }

        private void onRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragAction = false;

            MouseMove -= updateEllipseAngle;
            MouseRightButtonUp -= onRightButtonUp;

            if (_isTransformed) return;

            var matrix = _rotateTransform.Value;

            // Does a true transform of the shape.
            var ellipseGeometry = ellipse.RenderedGeometry;

            var originalBounds = ellipseGeometry.Bounds;

            ellipseGeometry.Transform = new MatrixTransform(matrix);
            var transformedEllipsePath = ellipseGeometry.GetOutlinedPathGeometry();

            ellipse.Visibility = Visibility.Hidden;
            renderedPath.Data = transformedEllipsePath;
            renderedPath.Visibility = Visibility.Visible;
            angleLine.Visibility = Visibility.Hidden;

            var renderedBounds = renderedPath.Data.Bounds;

            var widthRatio = renderedBounds.Width / originalBounds.Width;
            var heightRatio = renderedBounds.Height / originalBounds.Height;

            ellipseRendered?.Invoke(widthRatio, heightRatio);
        }
        #endregion


    }
}
