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
        private bool _allowMove;
        private Point _ellipseOrigin;
        private Vector _originalVector;
        private double _originalAngle;
        private RotateTransform _rotateTransform;
        #endregion

        #region Delegates
        public event Action<double, double> ellipseRendered;
        #endregion

        #region Constructor
        public ModifiableEllipse()
        {
            InitializeComponent();

            ellipse.MouseRightButtonDown += onRightClickEllipse;
            ellipse.MouseMove += updateEllipseAngle;
            border.MouseRightButtonUp += onRightButtonUp;

            _rotateTransform = new RotateTransform(0);

            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            ellipse.RenderTransform = _rotateTransform;

            ellipse.Visibility = Visibility.Visible;
            renderedPath.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Methods
        private void onRightClickEllipse(object sender, MouseEventArgs mouseEventArgs)
        {
            _ellipseOrigin = border.GetPosition();

            // Sets to center of ellipse position.
            _ellipseOrigin.X += border.ActualWidth / 2;
            _ellipseOrigin.Y += border.ActualHeight / 2;

            _allowMove = true;
        }

        private void updateEllipseAngle(object sender, MouseEventArgs e)
        {
            if (!_allowMove) return;

            var mousePosition = e.GetPosition(border);

            var deltaX = mousePosition.X - _ellipseOrigin.X;
            var deltaY = mousePosition.Y - _ellipseOrigin.Y;

            var angle = Math.Atan(deltaY / deltaX) * 180 / Math.PI;

            Console.WriteLine(angle);

            _rotateTransform.Angle = angle;
        }

        private void onRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _allowMove = false;

            var matrix = _rotateTransform.Value;

            // Does a true transform of the shape.
            var ellipseGeometry = ellipse.RenderedGeometry;

            var originalBounds = ellipseGeometry.Bounds;

            ellipseGeometry.Transform = new MatrixTransform(matrix);
            var transformedEllipsePath = ellipseGeometry.GetOutlinedPathGeometry();

            ellipse.Visibility = Visibility.Collapsed;
            renderedPath.Data = transformedEllipsePath;
            renderedPath.Visibility = Visibility.Visible;

            var renderedBounds = renderedPath.Data.Bounds;

            var widthRatio = renderedBounds.Width / originalBounds.Width;
            var heightRatio = renderedBounds.Height / originalBounds.Height;

            ellipseRendered?.Invoke(widthRatio, heightRatio);
        }
        #endregion
    }
}
