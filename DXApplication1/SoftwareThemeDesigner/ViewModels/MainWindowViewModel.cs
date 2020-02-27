using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using DevExpress.Mvvm;

namespace SoftwareThemeDesigner.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Constants
		private const string continentNameHeader = "Continent_Name";
		private const string continentCodeHeader = "Continent_Code";
		private const string countryNameHeader = "Country_Name";
		private const string countryCodeHeader = "Three_Letter_Country_Code";
		#endregion

		#region Fields
		private string _textBoxString;
		private ObservableCollection<Country> _comboBoxItems;
		private Country _comboBoxValue;
		private double _spinBoxValue;
		private ICollectionView _comboBoxCollectionView;

		#endregion

		#region Properties
		public string textBoxString
		{
			get { return _textBoxString; }
			set
			{
				_textBoxString = value;
				RaisePropertyChanged(() => textBoxString);
				Console.WriteLine($"Text Box Binding Value Changed: {_textBoxString}");
			}
		}
		public ObservableCollection<Country> comboBoxItems
		{
			get { return _comboBoxItems; }
			set { _comboBoxItems = value; RaisePropertyChanged(() => comboBoxItems); }
		}

		public ICollectionView comboBoxCollectionView
		{
			get { return _comboBoxCollectionView; }
			set { _comboBoxCollectionView = value; RaisePropertyChanged(() => comboBoxCollectionView); }
		}
		public Country comboBoxValue
		{
			get { return _comboBoxValue; }
			set
			{
				_comboBoxValue = value;
				RaisePropertyChanged(() => comboBoxValue);
			}
		}
		public double spinBoxValue
		{
			get { return _spinBoxValue; }
			set
			{
				_spinBoxValue = value;
				RaisePropertyChanged(() => spinBoxValue);
				Console.WriteLine($"Spin Box Binding Value Changed: {_spinBoxValue}");
			}
		}
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			var countriesText = File.ReadAllLines("CountryList.csv");

			var headerRow = countriesText.First().Split(',').ToList();

			var continentNameColumn = headerRow.IndexOf(continentNameHeader);
			var continentCodeColumn = headerRow.IndexOf(continentCodeHeader);
			var countryNameColumn = headerRow.IndexOf(countryNameHeader);
			var countryCodeColumn = headerRow.IndexOf(countryCodeHeader);

			var countryList = countriesText
				.Skip(1) // Skip header row
				.Select(x => x.Split(','))
				.Select(x => new Country(x[countryNameColumn],
					x[countryCodeColumn],
					x[continentNameColumn],
					x[continentCodeColumn]));

			comboBoxItems = new ObservableCollection<Country>(countryList);
			comboBoxCollectionView = CollectionViewSource.GetDefaultView(comboBoxItems);
			comboBoxCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Country.continentName)));
		}
		#endregion
	}
}