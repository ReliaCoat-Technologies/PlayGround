using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ReliaCoat.Common.UI;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester
{
	public partial class ThemeTesterWindow : Window
	{
		#region Fields
		private readonly ThemeTesterWindowViewModel _viewModel;
		private Country _dragItem;
		#endregion

		#region Constructor
		public ThemeTesterWindow()
		{
			InitializeComponent();

			_viewModel = new ThemeTesterWindowViewModel();
			DataContext = _viewModel;

			Loaded += async (s,e) => await onLoadedAsync();
		}
		#endregion

		#region Methods
		private async Task onLoadedAsync()
		{
			await _viewModel.initializeAsync();

			gridStack.children.Add(new Rectangle
			{
				Fill = Brushes.DodgerBlue,
				Stroke = Brushes.White,
				StrokeThickness = 2,
			});
		}
		#endregion
	}
}
