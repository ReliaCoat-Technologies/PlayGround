using System.Threading.Tasks;
using System.Windows;
using ReliaCoat.Common.UI.Controls.Utilities;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester
{
	public partial class ThemeTesterWindow : Window
	{
		#region Fields
		private readonly ThemeTesterWindowViewModel _viewModel;
		private readonly CollapsibleGridManager _collapsibleGridManager;
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
		}
		#endregion
	}
}
