using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReliaCoat.Common;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class ThemeTesterWindowViewModel : ReactiveObject
	{
		private CountrySelectorViewModel _mainSelectorViewModel;

		#region Properties
		public CountrySelectorViewModel mainSelectorViewModel
		{
			get { return _mainSelectorViewModel; }
			set { this.RaiseAndSetIfChanged(ref _mainSelectorViewModel, value); }
		}

		public ObservableCollection<CountrySelectorViewModel> selectorViewModels { get; }
		#endregion

		#region Constructor
		public ThemeTesterWindowViewModel()
		{
			selectorViewModels = new ObservableCollection<CountrySelectorViewModel>();
		}
		#endregion

		#region Methods
		public async Task initializeAsync()
		{
			var countries = await HelperFunctions.getCountriesAsync();

			mainSelectorViewModel = new CountrySelectorViewModel();

			mainSelectorViewModel.addCountries(countries);

			var viewModelsToAdd = Enumerable.Range(0, 1)
				.Select(x =>
				{
					var viewModel = new CountrySelectorViewModel();
					viewModel.addCountries(countries);
					return viewModel;
				})
				.ToList();

			selectorViewModels.addRange(viewModelsToAdd);
		}
		#endregion
	}
}