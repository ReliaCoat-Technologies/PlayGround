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
		private bool _isNextPointEnabled;
		private XyDataSeries<double> _hullDataSeries;
		#endregion

		#region Properties
		public NumericAxis xAxis { get; }
		public NumericAxis yAxis { get; }
		public ObservableCollection<IRenderableSeriesViewModel> renderableSeriesList { get; }
		public bool isNextPointEnabled
		{
			get { return _isNextPointEnabled; }
			set { this.RaiseAndSetIfChanged(ref _isNextPointEnabled, value); }
		}
		#endregion

		#region Commands
		public ICommand populateDataCommand { get; }
		public ICommand nextPointCommand { get; }
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
			nextPointCommand = ReactiveCommand.Create(getNextPoint);

			isNextPointEnabled = false;
		}
		#endregion

		#region Methods
		private void populateData()
		{
			renderableSeriesList.Clear();

			var dataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			var points = new List<XyPoint>();

			for (var i = 0; i < 20; i++)
			{
				var x = RandomSingleton.instance.NextDouble();
				var y = RandomSingleton.instance.NextDouble();

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

			_hullDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			var hullRenderableSeries = new LineRenderableSeriesViewModel
			{
				DataSeries = _hullDataSeries,
				Tag = "Hull Series",
				Stroke = Colors.Yellow,
				StrokeThickness = 2,
			};

			renderableSeriesList.Add(hullRenderableSeries);

			isNextPointEnabled = true;
		}

		private void getNextPoint()
		{
			var previousTraceLineSeries = renderableSeriesList
				.OfType<LineRenderableSeriesViewModel>()
				.FirstOrDefault(x => x.Tag == "Trace Line");

			if (previousTraceLineSeries != null)
			{
				renderableSeriesList.Remove(previousTraceLineSeries);
			}

			_convexHullCalculator.nextPoint();

			var points = _convexHullCalculator.getLast3Points();

			if (points == null)
			{
				return;
			}

			var traceDataSeries = new XyDataSeries<double> { AcceptsUnsortedData = true };

			traceDataSeries.Append(points[0].X, points[0].Y);
			traceDataSeries.Append(points[1].X, points[1].Y);
			traceDataSeries.Append(points[2].X, points[2].Y);

			var traceRenderableSeries = new LineRenderableSeriesViewModel
			{
				DataSeries = traceDataSeries,
				Tag = "Trace Line",
				Stroke = Colors.Red,
				StrokeThickness = 2,
			};

			renderableSeriesList.Add(traceRenderableSeries);

			_hullDataSeries.Clear();

			foreach (var hullPoint in _convexHullCalculator.hullPoints)
			{
				_hullDataSeries.Append(hullPoint.X, hullPoint.Y);
			}

			if (_convexHullCalculator.isCompleted)
			{
				var lastTracelineSeries = renderableSeriesList
					.OfType<LineRenderableSeriesViewModel>()
					.FirstOrDefault(x => x.Tag == "Trace Line");

				if (lastTracelineSeries != null)
				{
					renderableSeriesList.Remove(lastTracelineSeries);
				}

				isNextPointEnabled = false;
			}
		}
		#endregion
	}
}
