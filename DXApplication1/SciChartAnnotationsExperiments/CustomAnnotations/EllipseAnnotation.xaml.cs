using System;
using System.Windows;
using SciChart.Charting.Visuals.Annotations;

namespace SciChartAnnotationsExperiments.CustomAnnotations
{
    public partial class EllipseAnnotation : BoxAnnotation
    {
        #region
        private ModifiableEllipse _modifiableEllipse;
        #endregion

        #region Constructor
        public EllipseAnnotation()
        {
            InitializeComponent();
        }
        #endregion

        #region methods
        private void onEllipseLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _modifiableEllipse = sender as ModifiableEllipse;
        }
        #endregion
    }
}
