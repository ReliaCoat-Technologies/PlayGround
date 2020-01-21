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

        #region Constructor
        public ModifiableEllipse()
        {
            InitializeComponent();

            ellipse.MouseRightButtonDown += onRightClickEllipse;
            ellipse.MouseMove += updateEllipseAngle;
            border.MouseRightButtonUp += onRightButtonUp;

            _rotateTransform = new RotateTransform(0);

            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
            ellipse.RenderTransform= _rotateTransform;

            modifiedAngleLine.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Methods
        private void onRightClickEllipse(object sender, MouseEventArgs mouseEventArgs)
        {
            _ellipseOrigin = border.GetPosition();

            // Sets to center of ellipse position.
            _ellipseOrigin.X += border.ActualWidth / 2;
            _ellipseOrigin.Y += border.ActualHeight / 2;

            var mousePosition = mouseEventArgs.GetPosition(border);

            var delX = mousePosition.X - _ellipseOrigin.X;
            var delY = mousePosition.Y - _ellipseOrigin.Y;

            modifiedAngleLine.X1 = _ellipseOrigin.X - delX;
            modifiedAngleLine.X2 = _ellipseOrigin.X + delX;
            modifiedAngleLine.Y1 = _ellipseOrigin.Y - delY;
            modifiedAngleLine.Y2 = _ellipseOrigin.Y + delY;

            _allowMove = true;

            modifiedAngleLine.Visibility = Visibility.Visible;
        }

        private void updateEllipseAngle(object sender, MouseEventArgs e)
        {
            if (!_allowMove) return;

            var mousePosition = e.GetPosition(border);

            var delX = mousePosition.X - _ellipseOrigin.X;
            var delY = mousePosition.Y - _ellipseOrigin.Y;

            modifiedAngleLine.X1 = _ellipseOrigin.X - delX;
            modifiedAngleLine.X2 = _ellipseOrigin.X + delX;
            modifiedAngleLine.Y1 = _ellipseOrigin.Y - delY;
            modifiedAngleLine.Y2 = _ellipseOrigin.Y + delY;

            var deltaX = modifiedAngleLine.X2 - modifiedAngleLine.X1;
            var deltaY = modifiedAngleLine.Y2 - modifiedAngleLine.Y1;

            var angle = Math.Atan(deltaY / deltaX) * 180 / Math.PI;

            Console.WriteLine(angle);

            _rotateTransform.Angle = angle;
        }

        private void onRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _allowMove = false;
            modifiedAngleLine.Visibility = Visibility.Hidden;
        }

        private void Ellipse_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _allowMove = false;
            modifiedAngleLine.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}
