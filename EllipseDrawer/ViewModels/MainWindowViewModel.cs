using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using EllipseDrawer.Utilities;
using ReactiveUI;
using ReliaCoat.Common.UI.Extensions.SciChartExtensions;
using ReliaCoat.Numerics.CartesianMath;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Data.Model;

namespace EllipseDrawer.ViewModels
{
    public class MainWindowViewModel : SciChartSingleAxisViewModelBase
    {
        #region Fields
        private readonly XyDataSeries<double> _pointSeries;
        private readonly XyDataSeries<double> _centerSeries;
        private readonly XyDataSeries<double> _ellipseSeries;
        private readonly XyScatterRenderableSeriesViewModel _centerRenderableSeriesViewModel;
        private DoubleEllipse2D _currentEllipse;

        private double _testX;
        private double _testY;
        #endregion

        #region Properties
        public ObservableCollection<PointViewModel> points { get; }

        public double testX
        {
            get => _testX;
            set => this.RaiseAndSetIfChanged(ref _testX, value);
        }
        public double testY
        {
            get => _testY;
            set => this.RaiseAndSetIfChanged(ref _testY, value);
        }
        #endregion

        #region Commands
        public ICommand addPointCommand { get; }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            points = new ObservableCollection<PointViewModel>();
#if DEBUG
            addPoint(1, 1);
            addPoint(-1, 1);
            addPoint(-1, -1);
            addPoint(1, -1);

            testX = 0;
            testY = 0;
#endif
            xAxis.AxisTitle = "X";
            xAxis.GrowBy = new DoubleRange(0.2, 0.2);
            xAxis.AutoRange = AutoRange.Always;

            yAxis.AxisTitle = "Y";
            yAxis.GrowBy = new DoubleRange(0.2, 0.2);
            yAxis.AutoRange = AutoRange.Always;

            _pointSeries = new XyDataSeries<double>
            {
                AcceptsUnsortedData = true
            };

            _centerSeries = new XyDataSeries<double>
            {
                AcceptsUnsortedData = true
            };

            _ellipseSeries = new XyDataSeries<double>
            {
                AcceptsUnsortedData = true
            };

            var pointRenderableSeries = new XyScatterRenderableSeriesViewModel
            {
                PointMarker = new EllipsePointMarker
                {
                    Fill = Colors.DodgerBlue,
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    Width = 20,
                    Height = 20,
                },
                DataSeries = _pointSeries
            };

            renderableSeriesList.Add(pointRenderableSeries);

            _centerRenderableSeriesViewModel = new XyScatterRenderableSeriesViewModel
            {
                PointMarker = new EllipsePointMarker
                {
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    Width = 20,
                    Height = 20,
                },
                DataSeries = _centerSeries
            };

            renderableSeriesList.Add(_centerRenderableSeriesViewModel);

            var ellipseRenderableSeries = new MountainRenderableSeriesViewModel()
            {
                Fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)),
                Stroke = Colors.DarkOrange,
                DataSeries = _ellipseSeries,
                StrokeThickness = 2
            };

            renderableSeriesList.Add(ellipseRenderableSeries);

            updateChart();

            points.CollectionChanged += (s, e) => updateChart();

            this.WhenAnyValue(x => x.testX, x => x.testY)
                .Subscribe(testPoint);

            addPointCommand = ReactiveCommand.Create(() => addPoint());
        }
        #endregion

        #region Methods
        public void addPoint(double X = 0, double Y = 0)
        {
            var pointToAdd = new PointViewModel { X = X, Y = Y };

            pointToAdd
                .WhenAnyValue(x => x.X, x => x.Y)
                .Subscribe(x => updateChart());

            points.Add(pointToAdd);
        }

        private void updateChart()
        {
            if (_pointSeries == null)
            {
                return;
            }

            _pointSeries.Clear();

            var doublePoints = points
                .Select(x => x.getPoint())
                .ToArray();

            foreach (var point in doublePoints)
            {
                _pointSeries.Append(point.X, point.Y);
            }

            var sw = new Stopwatch();
            sw.Start();
            
            _currentEllipse = MinimumAreaEnclosingEllipse.getAllEncompassingEllipseLazy(doublePoints);

            sw.Stop();
            Debug.Write($"Calculation Time: {sw.Elapsed.TotalMilliseconds} ms");

            var intervals = 500;

            _ellipseSeries.Clear();

            for (var i = 0; i < intervals + 1; i++)
            {
                var angleRadians = 2 * Math.PI * i / intervals;

                var (x, y) = _currentEllipse.getCartesianCoordinatesForAngle(angleRadians);

                _ellipseSeries.Append(x, y);
            }

            testPoint((testX, testY));
        }

        private void testPoint((double, double) valueTuple)
        {
            if (_centerSeries == null || _centerRenderableSeriesViewModel == null)
            {
                return;
            }

            var point = new XyPoint(valueTuple.Item1, valueTuple.Item2);

            var isWithinEllipse = _currentEllipse.isPointWithinEllipse(point);

            _centerRenderableSeriesViewModel.PointMarker.Fill = isWithinEllipse
                ? Colors.DarkGreen
                : Colors.DarkRed;

            _centerSeries.Clear();
            _centerSeries.Append(_currentEllipse.centerX, _currentEllipse.centerY);
            _centerSeries.Append(testX, testY);
        }

        protected override void zoomExtentsOverride()
        {
            // Do nothing for now.
        }
        #endregion
    }
}