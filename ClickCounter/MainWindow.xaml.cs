using System;
using System.Windows;
using Gma.System.MouseKeyHook;

namespace ClickCounter
{
	public partial class MainWindow : Window
	{
		#region ViewModels
		private readonly MainWindowViewModel _viewModel;
		#endregion

		#region Constructor
		public MainWindow()
		{
			InitializeComponent();

			_viewModel = new MainWindowViewModel();
			DataContext = _viewModel;

			Hook.GlobalEvents().MouseDown += _viewModel.OnMouseDown;
		}
		#endregion
	}
}
