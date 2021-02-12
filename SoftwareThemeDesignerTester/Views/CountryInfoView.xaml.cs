using System.Windows.Controls;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester.Views
{
	public partial class CountryInfoView : UserControl
	{
		#region ViewModels
		public CountryInfoViewModel viewModel { get; set; }
		#endregion

		#region Constructor
		public CountryInfoView()
		{
			InitializeComponent();
		}
		#endregion

		#region Methods
		public void setViewModel(CountryInfoViewModel viewModelInput)
		{
			viewModel = viewModelInput;
			DataContext = viewModel;
		}
		#endregion
	}
}
