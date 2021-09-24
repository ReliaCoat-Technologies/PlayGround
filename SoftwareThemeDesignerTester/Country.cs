using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace SoftwareThemeDesignerTester
{
	public class Country : INotifyPropertyChanged
	{
		#region Fields
		private string _objectId;
		private string _name;
		private string _capital;
		private string _native;
		private string _currency;
		private string _continent;
		#endregion

		#region Delegates
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Constructor
		public Country(JObject countryJsonObject)
		{
			objectId = countryJsonObject[nameof(objectId)].ToString();
			name = countryJsonObject[nameof(name)].ToString();
			capital = countryJsonObject[nameof(capital)].ToString();
			native = countryJsonObject[nameof(native)].ToString();
			currency = countryJsonObject[nameof(currency)].ToString();
			continent = countryJsonObject["continent"]["name"].ToString();
		}
		#endregion

		#region Properties
		public string objectId
		{
			get { return _objectId; }
			set { _objectId = value; raisePropertyChanged(nameof(objectId)); }
		}
		public string name
		{
			get { return _name; }
			set { _name = value; raisePropertyChanged(nameof(name)); }
		}
		public string capital
		{
			get { return _capital; }
			set { _capital = value; raisePropertyChanged(nameof(capital)); }
		}
		public string native
		{
			get { return _native; }
			set { _native = value; raisePropertyChanged(nameof(native)); }
		}
		public string currency
		{
			get { return _currency; }
			set { _currency = value; raisePropertyChanged(nameof(currency)); }
		}
		public string continent
		{
			get { return _continent; }
			set
			{
				_continent = value;
				raisePropertyChanged(nameof(continent));
			}
		}
		#endregion

		#region Methods
		private void raisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}