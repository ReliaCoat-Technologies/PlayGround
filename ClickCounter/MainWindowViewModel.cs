using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace ClickCounter
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private ObservableCollection<MouseClickLogEntry> _mouseClickLog;
		private DateTime _timeStart;
		private DateTime _lastClickTime;
		private TimeSpan _testDuration;
		private bool _isRecording;
		#endregion

		#region Properties
		public bool isRecording
		{
			get { return _isRecording;}
			set { _isRecording = value; RaisePropertyChanged(nameof(isRecording)); }
		}
		public ObservableCollection<MouseClickLogEntry> mouseClickLog
		{
			get { return _mouseClickLog; }
			set
			{
				_mouseClickLog = value;
				RaisePropertyChanged(nameof(mouseClickLog));
				mouseClickLog.CollectionChanged += onMouseClickLogged;
			}
		}
		public int numLeftClicks => _mouseClickLog.Count(x => x.mouseButton == MouseButtons.Left);
		public int numRightClicks => _mouseClickLog.Count(x => x.mouseButton == MouseButtons.Right);
		public int numMiddleClicks => _mouseClickLog.Count(x => x.mouseButton == MouseButtons.Middle);
		public string timeStart => _timeStart.ToLongTimeString();
		public string lastClickTime => _lastClickTime.ToLongTimeString();
		public string testDuration => _testDuration.ToString("g");
		public int numClicks => mouseClickLog.Count;
		#endregion

		#region Commands
		public ICommand resetCounterCommand { get; set; }
		public ICommand exportDataCommand { get; set; }
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			mouseClickLog = new ObservableCollection<MouseClickLogEntry>();
			resetCounterCommand = new DelegateCommand(() => mouseClickLog.Clear());
			exportDataCommand = new DelegateCommand(exportData);
		}
		#endregion

		#region Methods
		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (!_isRecording) return;

			var entry = new MouseClickLogEntry
			{
				dateTime = DateTime.Now,
				xLocation = e.X,
				yLocation = e.Y,
				mouseButton = e.Button,
			};

			mouseClickLog.Add(entry);
		}

		private void onMouseClickLogged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!_mouseClickLog.Any())
			{
				_timeStart = default(DateTime);
				_lastClickTime = default(DateTime);
				_testDuration = default(TimeSpan);
			}
			else
			{
				_timeStart = _mouseClickLog.First().dateTime;
				_lastClickTime = _mouseClickLog.Last().dateTime;
				_testDuration = _lastClickTime - _timeStart;
			}

			RaisePropertyChanged(nameof(timeStart));
			RaisePropertyChanged(nameof(lastClickTime));
			RaisePropertyChanged(nameof(testDuration));
			RaisePropertyChanged(nameof(numClicks));
			RaisePropertyChanged(nameof(numLeftClicks));
			RaisePropertyChanged(nameof(numRightClicks));
			RaisePropertyChanged(nameof(numMiddleClicks));
		}

		private void exportData()
		{
			isRecording = false;

			var saveDialog = new SaveFileDialog
			{
				CheckPathExists = true,
				DefaultExt = ".csv",
				RestoreDirectory = true,
				Filter = "Comma Separated Value|.csv"
			};

			var result = saveDialog.ShowDialog();

			if (result != DialogResult.OK) return;

			var fileName = saveDialog.FileName;

			using (var sw = new StreamWriter(fileName))
			{
				sw.WriteLine("Date/Time, X, Y, MouseButton");

				foreach(var logEntry in _mouseClickLog)
					sw.WriteLine($"{logEntry.dateTime.ToLongTimeString()},{logEntry.xLocation},{logEntry.yLocation},{logEntry.mouseButton}");
			}
		}
		#endregion
	}
}