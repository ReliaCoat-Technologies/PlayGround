using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using ReactiveUI;
using ReliaCoat.Common;
using ReliaCoat.Numerics.CartesianMath;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Data.Model;

namespace BubbleChartTesting
{
	public class MainWindowViewModel : ReactiveObject
	{
		#region Fields
		private ConvexHullCalculator _convexHullCalculator;
		private bool _isCalculateEnabled;
		private XyDataSeries<double> _hullDataSeries;
		#endregion

		#region Properties
		public NumericAxis xAxis { get; }
		public NumericAxis yAxis { get; }
		public ObservableCollection<IRenderableSeriesViewModel> renderableSeriesList { get; }
		public bool isCalculateEnabled
		{
			get { return _isCalculateEnabled; }
			set { this.RaiseAndSetIfChanged(ref _isCalculateEnabled, value); }
		}
		#endregion

		#region Commands
		public ICommand populateDataCommand { get; }
		public ICommand calculateHullCommand { get; }
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			xAxis = new NumericAxis
			{
				AxisTitle = "X-Axis",
				GrowBy = new DoubleRange(1, 1),
				AutoRange = AutoRange.Always,
			};

			yAxis = new NumericAxis
			{
				AxisTitle = "Y-Axis",
				GrowBy = new DoubleRange(1, 1),
				AutoRange = AutoRange.Always,
			};

			renderableSeriesList = new ObservableCollection<IRenderableSeriesViewModel>();

			populateDataCommand = ReactiveCommand.Create(populateData);
			calculateHullCommand = ReactiveCommand.Create(getConvexHull);

			isCalculateEnabled = false;
		}
		#endregion

		#region Methods
		private void populateData()
		{
			renderableSeriesList.Clear();

			var dataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			var points = new List<XyPoint>();

			for (var i = 0; i < 8; i++)
			{
				var x = RandomSingleton.instance.NextDouble();
				var y = RandomSingleton.instance.NextDouble() * 30;

				points.Add(new XyPoint(x, y));
			}

			// Populate Points onto Chart
			foreach (var point in points)
			{
				dataSeries.Append(point.X, point.Y);
			}

			var allPointsRenderableSeries = new XyScatterRenderableSeriesViewModel
			{
				DataSeries = dataSeries,
				Tag = "Source Point",
				PointMarker = new EllipsePointMarker
				{
					Height = 20,
					Width = 20,
					Fill = Colors.DodgerBlue,
					Stroke = Colors.White,
					StrokeThickness = 2
				}
			};

			renderableSeriesList.Add(allPointsRenderableSeries);

			// Populate Angle Lines
			_convexHullCalculator = new ConvexHullCalculator(points);

			_convexHullCalculator.initializeHullStack();

			foreach (var point in _convexHullCalculator.allPointVectors)
			{
				var angleLineDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

				angleLineDataSeries.Append(_convexHullCalculator.firstPoint.X, _convexHullCalculator.firstPoint.Y);
				angleLineDataSeries.Append(point.targetPoint.X, point.targetPoint.Y);

				var renderableSeries = new LineRenderableSeriesViewModel
				{
					DataSeries = angleLineDataSeries,
					Tag = "Angle Line",
					Stroke = Colors.Green,
					StrokeThickness = 2,
				};

				renderableSeriesList.Add(renderableSeries);
			}

			isCalculateEnabled = true;
		}

		private void getConvexHull()
		{
			// Get Convex Hull
			_convexHullCalculator.calculateHull();

			_hullDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			foreach (var hullPoint in _convexHullCalculator.hullPoints)
			{
				_hullDataSeries.Append(hullPoint.X, hullPoint.Y);
			}

			var hullRenderableSeries = new LineRenderableSeriesViewModel
			{
				DataSeries = _hullDataSeries,
				Tag = "Hull Series",
				Stroke = Colors.Yellow,
				StrokeThickness = 2,
			};

			renderableSeriesList.Add(hullRenderableSeries);

			// Find Midpoint
			var midpointDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			var midpoint = _convexHullCalculator.hullPoints.getCartesianMidpoint();
			midpointDataSeries.Append(midpoint.X, midpoint.Y);

			var midpointRenderableSeries = new XyScatterRenderableSeriesViewModel()
			{
				DataSeries = midpointDataSeries,
				Tag = "Source Point",
				PointMarker = new EllipsePointMarker
				{
					Height = 20,
					Width = 20,
					Fill = Colors.Orange,
					Stroke = Colors.White,
					StrokeThickness = 2
				}
			};

			renderableSeriesList.Add(midpointRenderableSeries);

			// Find Scaled Hull
			
            /*
            var scaledHull = _convexHullCalculator.hullPoints.scaleBy(1.2);

			var scaledHullDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			foreach (var point in scaledHull)
			{
				scaledHullDataSeries.Append(point.X, point.Y);
			}

			var scaledHullRenderableSeries = new LineRenderableSeriesViewModel
			{
				DataSeries = scaledHullDataSeries,
				Tag = "Source Point",
				Stroke = Colors.MediumPurple,
			};

			renderableSeriesList.Add(scaledHullRenderableSeries);

			isCalculateEnabled = false;
			*/
		}
		#endregion
	}
}
