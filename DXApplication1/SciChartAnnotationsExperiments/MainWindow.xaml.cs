using System;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using SciChartAnnotationsExperiments.ViewModels;
using SciChart.Charting.Visuals.Annotations;

namespace SciChartAnnotationsExperiments
{
    public partial class MainWindow : ThemedWindow
    {
        #region Fields
        private readonly MainWindowViewModel _viewModel;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }
        #endregion

        #region Methods
        private void onBoxDrawClicked(object sender, ItemClickEventArgs e)
        {
            if (boxCheckItem.IsChecked == true)
            {
                enableBoxDraw();
            }
            else
            {
                disableBoxDraw();
            }
        }

        private void enableBoxDraw()
        {
            _viewModel.rubberBandModifier.IsEnabled = false;
            _viewModel.annotationCreationModifier.IsEnabled = true;
            _viewModel.annotationCreationModifier.AnnotationCreated += onAnnotationCreated;
        }

        private void disableBoxDraw()
        {
            _viewModel.annotationCreationModifier.IsEnabled = false;
            _viewModel.annotationCreationModifier.AnnotationCreated -= onAnnotationCreated;
            _viewModel.rubberBandModifier.IsEnabled = true;
        }

        private void onAnnotationCreated(object sender, SciChart.Charting.ChartModifiers.AnnotationCreationArgs e)
        {
            var annotation = (BoxAnnotation)_viewModel.annotationCreationModifier.Annotation;

            annotation.IsEditable = true;
            annotation.LayoutTransform= new RotateTransform(-45, 0.5, 0.5);

            disableBoxDraw();
            boxCheckItem.IsChecked = false;
        }
        #endregion
    }
}
