using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using DevExpress.Mvvm;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.Annotations;
using SciChart.Charting.Visuals.Axes;
using SciChart.Data.Model;
using SciChartAnnotationsExperiments.CustomAnnotations;

namespace SciChartAnnotationsExperiments.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private ObservableCollection<IRenderableSeriesViewModel> _renderableSeriesList;
        private IAxis _xAxis;
        private IAxis _yAxis;
        private AnnotationCollection _annotations;
        private ModifierGroup _modifiers;
        private RubberBandXyZoomModifier _rubberBandModifier;
        private AnnotationCreationModifier _annotationCreationModifier;
        #endregion

        #region Properties
        public ObservableCollection<IRenderableSeriesViewModel> renderableSeriesList
        {
            get { return _renderableSeriesList; }
            set { _renderableSeriesList = value; RaisePropertyChanged(() => renderableSeriesList); }
        }
        public IAxis xAxis
        {
            get { return _xAxis; }
            set { _xAxis = value; RaisePropertyChanged(() => xAxis); }
        }
        public IAxis yAxis
        {
            get { return _yAxis; }
            set { _yAxis = value; RaisePropertyChanged(() => yAxis); }
        }
        public AnnotationCollection annotations
        {
            get { return _annotations; }
            set { _annotations = value; RaisePropertyChanged(() => annotations); }
        }
        public ModifierGroup modifiers
        {
            get { return _modifiers; }
            set { _modifiers = value; RaisePropertyChanged(() => modifiers); }
        }
        public RubberBandXyZoomModifier rubberBandModifier
        {
            get { return _rubberBandModifier; }
            set { _rubberBandModifier = value; RaisePropertyChanged(() => rubberBandModifier); }
        }
        public AnnotationCreationModifier annotationCreationModifier
        {
            get { return _annotationCreationModifier; }
            set { _annotationCreationModifier = value; RaisePropertyChanged(() => annotationCreationModifier);}
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            renderableSeriesList = new ObservableCollection<IRenderableSeriesViewModel>();

            modifiers = new ModifierGroup();
            annotations = new AnnotationCollection();
            annotationCreationModifier = new AnnotationCreationModifier();

            setAxesAndModifiers();
            plotData();
        }

        private void setAxesAndModifiers()
        {
            xAxis = new NumericAxis
            {
                AxisTitle = "X",
                DrawMajorBands = false,
                AutoRange = AutoRange.Once
            };

            yAxis = new NumericAxis
            {
                AxisTitle = "Y",
                GrowBy = new DoubleRange(0.1, 0.1),
                DrawMajorBands = false,
                AutoRange = AutoRange.Once
            };

            annotationCreationModifier = new AnnotationCreationModifier
            {
                ExecuteOn = ExecuteOn.MouseLeftButton,
                AnnotationType = typeof(EllipseAnnotation),
                IsEnabled = false
            };

            rubberBandModifier = new RubberBandXyZoomModifier();

            var zoomExtentsModifier = new ZoomExtentsModifier
            {
                ExecuteOn = ExecuteOn.MouseDoubleClick
            };

            modifiers.ChildModifiers.Add(zoomExtentsModifier);
            modifiers.ChildModifiers.Add(rubberBandModifier);
            modifiers.ChildModifiers.Add(annotationCreationModifier);
        }

        public void plotData()
        {
            var xValues = Enumerable.Range(-50000, 100001)
                .Select(x => x / 5000d)
                .ToList();

            var yValues = xValues
                .Select(Math.Cos)
                .ToList();

            var dataSeries = new XyDataSeries<double, double>();
            dataSeries.Append(xValues, yValues);

            var renderableSeries = new LineRenderableSeriesViewModel
            {
                Stroke = Colors.DodgerBlue,
                DataSeries = dataSeries
            };

            renderableSeriesList.Add(renderableSeries);
        }
        #endregion

    }
}