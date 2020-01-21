using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using SciChart.Charting.Visuals.Annotations;
using SciChartAnnotationsExperiments.CustomAnnotations;
using SciChartAnnotationsExperiments.ViewModels;

namespace SciChartAnnotationsExperiments
{
    public partial class MainWindow : ThemedWindow
    {
        #region Fields
        private readonly MainWindowViewModel _viewModel;
        private readonly IDictionary<BarCheckItem, Type> _labelTypeDictionary;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _labelTypeDictionary = new Dictionary<BarCheckItem, Type>
            {
                { ellipseCheckItem, typeof(EllipseAnnotation) },
                { boxCheckItem, typeof(BoxAnnotation) },
            };
        }
        #endregion

        #region Methods
        private void onDrawClicked(object sender, ItemClickEventArgs e)
        {
            var barItem = sender as BarCheckItem;

            if (barItem == null) return;

            _viewModel.annotationCreationModifier.AnnotationType = _labelTypeDictionary[barItem];

            if (barItem.IsChecked == true)
            {
                enableAnnotationDraw();
            }
            else
            {
                uncheckAll();
                disableAnnotationDraw();
            }
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
            uncheckAll();
        }

        private void uncheckAll()
        {
            foreach (var barCheckItem in _labelTypeDictionary.Keys)
                barCheckItem.IsChecked = false;
        }
        #endregion
    }
}
