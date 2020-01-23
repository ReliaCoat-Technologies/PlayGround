using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class ModifiableEllipse : UserControl
    {
        #region Fields
        private readonly RotateTransform _rotateTransform;

        private bool _isDragging;
        private Point _ellipseOrigin;
        #endregion

        #region Delegates
        public event Action rotationStarted;
        public event Action<double, double> rotationCompleted;
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

            // Showing the original ellipse when it's being created.
            ellipse.Visibility = Visibility.Visible;
            renderedPath.Visibility = Visibility.Hidden;
            angleLine.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Methods
        public void onParentSizeChanged(Size e)
        {
            // Resets the ellipse origin when the size has changed.
            _ellipseOrigin = new Point(e.Width / 2, e.Height / 2);
        }

        private void onRightClickEllipse(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MouseMove += updateEllipseAngle;
            MouseRightButtonUp += onRightButtonUp;

            rotationStarted?.Invoke();

            ellipse.Visibility = Visibility.Visible;
            renderedPath.Visibility = Visibility.Hidden;
            angleLine.Visibility = Visibility.Visible;

            _isDragging = true;
        }

        private void updateEllipseAngle(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            if (Mouse.RightButton != MouseButtonState.Pressed) return;

            var mousePosition = e.GetPosition(border);

            // Calculates the vector of the mouse with respect to the ellipse's origin.
            angleLine.X1 = _ellipseOrigin.X;
            angleLine.Y1 = _ellipseOrigin.Y;
            angleLine.X2 = mousePosition.X;
            angleLine.Y2 = mousePosition.Y;

            var deltaX = mousePosition.X - _ellipseOrigin.X;
            var deltaY = mousePosition.Y - _ellipseOrigin.Y;

            // Calculates the angle of the vector with respect to the X-Axis, in degrees.
            var angle = Math.Atan(deltaY / deltaX) * 180 / Math.PI;

            // Sets the Rotate Transform's angle appropriately.
            _rotateTransform.Angle = angle;
        }

        private void onRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;

            MouseMove -= updateEllipseAngle;
            MouseRightButtonUp -= onRightButtonUp;

            // Converts WPF Ellipse to WPF Path
            var matrix = _rotateTransform.Value;

            // Gets the Geometry of the un-transformed and its bounds.
            var ellipseGeometry = ellipse.RenderedGeometry;
            var originalBounds = ellipseGeometry.Bounds;

            // Performs transformation on the ellipse's geometry and creates a path based on it.
            ellipseGeometry.Transform = new MatrixTransform(matrix);
            var transformedEllipsePath = ellipseGeometry.GetOutlinedPathGeometry();
            renderedPath.Data = transformedEllipsePath;

            // Obtains the bounds of the transformed ellipse's path
            var transformedBounds = renderedPath.Data.Bounds;

            // Hides the original ellipse, shows the transformed path instead.
            ellipse.Visibility = Visibility.Hidden;
            renderedPath.Visibility = Visibility.Visible;
            angleLine.Visibility = Visibility.Hidden;

            // Obtains change in width and height of ellipse after rotation.
            var widthRatio = transformedBounds.Width / originalBounds.Width;
            var heightRatio = transformedBounds.Height / originalBounds.Height;

            rotationCompleted?.Invoke(widthRatio, heightRatio);
        }
        #endregion
    }
}
