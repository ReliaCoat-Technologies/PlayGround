using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using DevExpress.Mvvm;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private Country _selectedCountry;
		private ObservableCollection<Country> _countryList;
		private ICollectionView _collectionView;
		#endregion

		#region Properties
		public ObservableCollection<Country> countryList
		{
			get { return _countryList; }
			set
			{
				_countryList = value;
				RaisePropertyChanged(() => countryList);

				collectionView = CollectionViewSource.GetDefaultView(_countryList);
				collectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Country.continentName)));
			}
		}

		public Country selectedCountry
		{
			get { return _selectedCountry; }
			set
			{
				_selectedCountry = value;
				RaisePropertyChanged(() => selectedCountry);
				Console.WriteLine(_selectedCountry.countryName);
			}
		}

		public ICollectionView collectionView
		{
			get { return _collectionView; }
			set { _collectionView = value; RaisePropertyChanged(() => collectionView); }
		}
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			countryList = new ObservableCollection<Country>(HelperFunctions.getCountryList());
		}
		#endregion
	}
}