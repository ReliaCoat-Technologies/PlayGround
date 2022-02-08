using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReliaCoat.Common;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class CountrySelectorViewModel : ReactiveObject
	{
		#region Fields and Properties
		private IList<object> _selectedCountries;

		public IList<object> selectedCountries
		{
			get { return _selectedCountries; }
			set
			{
				_selectedCountries = value;
				this.RaisePropertyChanged(nameof(selectedCountries));
				onSelectedCountryChanged(_selectedCountries);
			}
		}

		public ObservableCollection<Country> countryList { get; }
		#endregion

		#region Constructor
		public CountrySelectorViewModel()
		{
			countryList = new ObservableCollection<Country>();

			selectedCountries = new List<object>();

			this.WhenAnyValue(x => x.selectedCountries)
				.Subscribe(onSelectedCountryChanged);
		}
		#endregion

		#region Methods
		public void addCountries(IEnumerable<Country> countryListInput)
		{
			countryList.addRange(countryListInput);
		}

		private void onSelectedCountryChanged(IList<object> selectedCountriesInput)
		{
			if (!selectedCountriesInput.Any())
			{
				return;
			}

			Console.WriteLine($"Countries: {string.Join(", ", selectedCountriesInput.OfType<Country>().Select(x => x.name).ToArray()) }");
		}
		#endregion
	}
}
