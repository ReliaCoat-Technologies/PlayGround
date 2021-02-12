using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Mvvm;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class ThemeTesterWindowViewModel : ViewModelBase
	{
		#region Fields
		private IList<object> _selectedCountry;
		private ObservableCollection<Country> _countryList;
		private ICollectionView _collectionView;
		private ObservableCollection<Country> _draggedCountryList;
		private Color _selectedColor;
		#endregion

		#region Properties
		public ObservableCollection<Country> countryList
		{
			get { return _countryList; }
			set
			{
				_countryList = value;
				RaisePropertyChanged(() => countryList);
				RaisePropertyChanged(() => shortList);

				draggedCountryList = new ObservableCollection<Country>();
			}
		}
		public List<Country> shortList => countryList.Take(10).ToList();
		public ObservableCollection<Country> draggedCountryList
		{
			get { return _draggedCountryList; }
			set { _draggedCountryList = value; RaisePropertyChanged(() => draggedCountryList); }
		}
		public Color selectedColor
		{
			get { return _selectedColor; }
			set
			{
				_selectedColor = value;
				RaisePropertyChanged(nameof(selectedColor));
			}
		}
		#endregion

		#region Methods
		public async Task initializeAsync()
		{
			countryList = new ObservableCollection<Country>(await HelperFunctions.getCountriesAsync());
		}
		#endregion
	}
}