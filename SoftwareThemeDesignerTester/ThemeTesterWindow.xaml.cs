using System;
using System.Threading.Tasks;
using System.Windows;
using SoftwareThemeDesignerTester.ViewModels;
using SoftwareThemeDesignerTester.Views;

namespace SoftwareThemeDesignerTester
{
	public partial class ThemeTesterWindow : Window
	{
		#region Fields
		private readonly ThemeTesterWindowViewModel _viewModel;
		private Random _random;
		#endregion

		#region Constructor
		public ThemeTesterWindow()
		{
			InitializeComponent();

			_viewModel = new ThemeTesterWindowViewModel();
			DataContext = _viewModel;

			_random = new Random();

			Loaded += async (s,e) => await onLoadedAsync();
		}
		#endregion

		#region Methods
		private async Task onLoadedAsync()
		{
			await _viewModel.initializeAsync();
		}
		#endregion

		private void addNewItem(object sender, RoutedEventArgs e)
		{
			var value = _random.Next(0, _viewModel.countryList.Count - 1);

			var viewModel = new CountryInfoViewModel();
			viewModel.country = _viewModel.countryList[value];
			var view = new CountryInfoView();
			view.setViewModel(viewModel);

			gridStack.children.Add(view);
		}
	}
}
