using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SciChart.Charting.Visuals.Annotations;
using SciChartAnnotationsExperiments.CustomAnnotations;
using SciChartAnnotationsExperiments.ViewModels;

namespace SciChartAnnotationsExperiments
{
    public partial class MainWindow : Window
    {
        #region Fields
        private readonly MainWindowViewModel _viewModel;
        private readonly IDictionary<Button, Type> _labelTypeDictionary;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _labelTypeDictionary = new Dictionary<Button, Type>
            {
                { ellipseButton, typeof(EllipseAnnotation) }
            };
        }
        #endregion

        #region Methods
        private void onDrawClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            var barItem = sender as Button;

            if (barItem == null) return;

            _viewModel.annotationCreationModifier.AnnotationType = _labelTypeDictionary[barItem];

            enableAnnotationDraw();
        }

        private void enableAnnotationDraw()
        {
            _viewModel.rubberBandModifier.IsEnabled = false;
            _viewModel.annotationCreationModifier.IsEnabled = true;
            _viewModel.annotationCreationModifier.AnnotationCreated += onAnnotationCreated;
        }

        private void disableAnnotationDraw()
        {
            _viewModel.annotationCreationModifier.IsEnabled = false;
            _viewModel.annotationCreationModifier.AnnotationCreated -= onAnnotationCreated;
            _viewModel.rubberBandModifier.IsEnabled = true;
        }

        private void onAnnotationCreated(object sender, SciChart.Charting.ChartModifiers.AnnotationCreationArgs e)
        {
            var abase = _viewModel.annotationCreationModifier.Annotation as AnnotationBase;

            abase.IsEditable = true;

            disableAnnotationDraw();
        }
        #endregion
    }
}
