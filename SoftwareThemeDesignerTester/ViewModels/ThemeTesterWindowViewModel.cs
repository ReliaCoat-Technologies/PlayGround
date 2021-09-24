using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Mvvm;
using ReliaCoat.Common;
using ReliaCoat.Common.UI.Controls.ViewModels;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class ThemeTesterWindowViewModel : ViewModelBase
	{
		#region Fields
		private ObservableCollection<Country> _countryRepository;
		private ObservableCollection<object> _countryList;
		private ICollectionView _countryCollectionView;
		#endregion

		#region Properties
		public ObservableCollection<Country> countryRepository
		{
			get { return _countryRepository; }
			set { _countryRepository = value; RaisePropertyChanged(() => countryRepository); }
		}
		public ICollectionView countryCollectionView
		{
			get { return _countryCollectionView; }
			set { _countryCollectionView = value; RaisePropertyChanged(() => countryCollectionView); }
		}
		public ObservableCollection<object> countryList
		{
			get { return _countryList; }
			set { _countryList = value; RaisePropertyChanged(() => countryList); }
		}
		#endregion

		#region Constructor
		public ThemeTesterWindowViewModel()
		{
			countryRepository = new ObservableCollection<Country>();
			countryList = new ObservableCollection<object>();
			countryCollectionView = CollectionViewSource.GetDefaultView(countryRepository);
		}
		#endregion

		#region Methods
		public async Task initializeAsync()
		{
			var countries = await HelperFunctions.getCountriesAsync();

			countryRepository.addRange(countries);
		}
		#endregion
	}
}