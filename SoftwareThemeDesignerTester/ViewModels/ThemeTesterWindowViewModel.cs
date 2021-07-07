using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using DevExpress.Mvvm;
using ReliaCoat.Common;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class ThemeTesterWindowViewModel : ViewModelBase
	{
		#region Fields
		private readonly Random _random;
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
			_random = new Random();

			countryRepository = new ObservableCollection<Country>();
			countryList = new ObservableCollection<object>();
			countryCollectionView = CollectionViewSource.GetDefaultView(countryRepository);
		}
		#endregion

		#region Methods
		public async Task initializeAsync()
		{
			countryRepository.addRange(await HelperFunctions.getCountriesAsync());
		}
		#endregion
	}
}