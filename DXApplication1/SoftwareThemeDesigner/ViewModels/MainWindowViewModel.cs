using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;

namespace SoftwareThemeDesigner.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		#region Fields
		private string _textBoxString;
		private ObservableCollection<string> _comboBoxItems;
		private string _comboBoxValue;
		#endregion

		#region Properties
		public string textBoxString
		{
			get { return _textBoxString; }
			set { _textBoxString = value; RaisePropertyChanged(() => textBoxString); }
		}
		public ObservableCollection<string> comboBoxItems
		{
			get { return _comboBoxItems; }
			set { _comboBoxItems = value; RaisePropertyChanged(() => comboBoxItems); }
		}
		public string comboBoxValue
		{
			get { return _comboBoxValue; }
			set { _comboBoxValue = value; RaisePropertyChanged(() => comboBoxValue); }
		}
		#endregion

		#region Constructor
		public MainWindowViewModel()
		{
			var itemsToAdd = new List<string>
			{
				"Alabama", "Alaska", "American Samoa", "Arizona", "Arkansas", "California", "Colorado", "Connecticut",
				"Delaware", "District of Columbia", "Florida", "Georgia", "Guam",
				"Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine",
				"Maryland", "Massachusetts", "Michigan", "Minnesota", "Minor Outlying Islands",
				"Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
				"New York", "North Carolina", "North Dakota", "Northern Mariana Islands",
				"Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Puerto Rico", "Rhode Island", "South Carolina",
				"South Dakota", "Tennessee", "Texas", "U.S. Virgin Islands", "Utah", "Vermont",
				"Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
			};

			comboBoxItems = new ObservableCollection<string>(itemsToAdd);
		}
		#endregion
	}
}