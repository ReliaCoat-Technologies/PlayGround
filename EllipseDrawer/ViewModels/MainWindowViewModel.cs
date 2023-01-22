﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media;
using EllipseDrawer.Utilities;
using ReactiveUI;
using ReliaCoat.Common.UI.Extensions.SciChartExtensions;
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
        private DoubleEllipse2D _currentEllipse;

        private double _X1;
        private double _Y1;
        private double _X2;
        private double _Y2;
        private double _X3;
        private double _Y3;
        private double _X4;
        private double _Y4;
        private double _X5;
        private double _Y5;
        private double _testX;
        private double _testY;
        #endregion

        #region Properties
        public double X1
        {
            get => _X1;
            set => this.RaiseAndSetIfChanged(ref _X1, value);
        }
        public double Y1
        {
            get => _Y1;
            set => this.RaiseAndSetIfChanged(ref _Y1, value);
        }
        public double X2
        {
            get => _X2;
            set => this.RaiseAndSetIfChanged(ref _X2, value);
        }
        public double Y2
        {
            get => _Y2;
            set => this.RaiseAndSetIfChanged(ref _Y2, value);
        }
        public double X3
        {
            get => _X3;
            set => this.RaiseAndSetIfChanged(ref _X3, value);
        }
        public double Y3
        {
            get => _Y3;
            set => this.RaiseAndSetIfChanged(ref _Y3, value);
        }
        public double X4
        {
            get => _X4;
            set => this.RaiseAndSetIfChanged(ref _X4, value);
        }
        public double Y4
        {
            get => _Y4;
            set => this.RaiseAndSetIfChanged(ref _Y4, value);
        }
        public double X5
        {
            get => _X5;
            set => this.RaiseAndSetIfChanged(ref _X5, value);
        }
        public double Y5
        {
            get => _Y5;
            set => this.RaiseAndSetIfChanged(ref _Y5, value);
        }
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

        #region Constructor
        public MainWindowViewModel()
        {
            var propertyObservables = new[]
            {
                this.WhenAnyValue(x => x.X1),
                this.WhenAnyValue(x => x.Y1),
                this.WhenAnyValue(x => x.X2),
                this.WhenAnyValue(x => x.Y2),
                this.WhenAnyValue(x => x.X3),
                this.WhenAnyValue(x => x.Y3),
                this.WhenAnyValue(x => x.X4),
                this.WhenAnyValue(x => x.Y4),
                this.WhenAnyValue(x => x.X5),
                this.WhenAnyValue(x => x.Y5),
            };

            this.WhenAnyValue(x => x.testX, x => x.testY)
                .Subscribe(x => testPoint(x));

#if DEBUG
            X1 = -7;
            Y1 = -3;
            X2 = 5;
            Y2 = -3;
            X3 = 2;
            Y3 = 6;
            X4 = -2;
            Y4 = -6;
            X5 = -5;
            Y5 = 3;
#endif

            var combinedObservable = Observable.CombineLatest(propertyObservables);

            combinedObservable
                .ObserveOnDispatcher()
                .Subscribe(updateChart);

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

            var centerRenderableSeries = new XyScatterRenderableSeriesViewModel
            {
                PointMarker = new EllipsePointMarker
                {
                    Fill = Colors.DarkRed,
                    Stroke = Colors.White,
                    StrokeThickness = 2,
                    Width = 20,
                    Height = 20,
                },
                DataSeries = _centerSeries
            };

            renderableSeriesList.Add(centerRenderableSeries);

            var ellipseRenderableSeries = new MountainRenderableSeriesViewModel()
            {
                Fill = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)),
                Stroke = Colors.DarkOrange,
                DataSeries = _ellipseSeries,
                StrokeThickness = 2
            };

            renderableSeriesList.Add(ellipseRenderableSeries);
        }
        #endregion

        #region Methods
        private void updateChart(IList<double> values)
        {
            _pointSeries.Clear();

            _pointSeries.Append(values[0], values[1]);
            _pointSeries.Append(values[2], values[3]);
            _pointSeries.Append(values[4], values[5]);
            _pointSeries.Append(values[6], values[7]);
            _pointSeries.Append(values[8], values[9]);

            var point1 = new DoublePoint2D(values[0], values[1]);
            var point2 = new DoublePoint2D(values[2], values[3]);
            var point3 = new DoublePoint2D(values[4], values[5]);
            var point4 = new DoublePoint2D(values[6], values[7]);
            var point5 = new DoublePoint2D(values[8], values[9]);

            _currentEllipse = new DoubleEllipse2D(point1, point2, point3, point4, point5);

            var intervals = 500;

            _centerSeries.Clear();

            _centerSeries.Append(_currentEllipse.centerX, _currentEllipse.centerY);

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
            var point = new DoublePoint2D(valueTuple.Item1, valueTuple.Item2);

            var isWithinEllipse = _currentEllipse.isPointWithin(point);
        }

        protected override void zoomExtentsOverride()
        {
            // Do nothing for now.
        }
        #endregion
    }
}