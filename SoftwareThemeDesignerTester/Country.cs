using System.Collections.Generic;

namespace SoftwareThemeDesignerTester
{
	public class Country
	{
		#region Properties
		public string name { get; set; }
		public string capital { get; set; }
		public List<string> topLevelDomain { get; set; }
		public string alpha2Code { get; set; }
		public string alpha3Code { get; set; }
		public List<string> callingCodes { get; set; }
		public List<string> altSpellings { get; set; }
		public string region { get; set; }
		public string subregion { get; set; }
		public int population { get; set; }
		public List<double> latlng { get; set; }
		public string demonym { get; set; }
		public double area { get; set; }
		public double gini { get; set; }
		public List<string> timeZones { get; set; }
		public List<string> borders { get; set; }
		public string numericCode { get; set; }
		#endregion
	}
}