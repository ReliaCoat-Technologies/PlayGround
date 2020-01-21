using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class ModifiableEllipse : UserControl
    {
        public ModifiableEllipse()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseEventArgs mouseEventArgs)
        {
            ellipse.Fill = ellipse.Fill == Brushes.Blue 
                ? Brushes.Red
                : Brushes.Blue;
        }
    }
}
