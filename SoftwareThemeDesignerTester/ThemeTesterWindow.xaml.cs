using System.Threading.Tasks;
using System.Windows;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester
{
	public partial class ThemeTesterWindow : Window
	{
		#region Fields
		private readonly ThemeTesterWindowViewModel _viewModel;
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
			
		}
		#endregion
	}
}
