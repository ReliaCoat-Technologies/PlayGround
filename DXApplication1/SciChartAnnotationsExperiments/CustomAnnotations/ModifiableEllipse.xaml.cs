using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class ModifiableEllipse : UserControl
    {
        public ModifiableEllipse()
        {
            InitializeComponent();
            ellipse.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private void onRightClickEllipse(object sender, MouseEventArgs mouseEventArgs)
        {
            ellipse.RenderTransform = new RotateTransform(-45);
            var borderRect = LayoutInformation.GetLayoutSlot(border);
            ellipse.TransformToVisual(border).TransformBounds(borderRect);
        }
    }
}
