using System.Windows.Controls;
using SoftwareThemeDesignerTester.ViewModels;

namespace SoftwareThemeDesignerTester.Views
{
	public partial class CountryInfoView : UserControl
	{
		#region ViewModels
		public CountryInfoViewModel viewModel => DataContext as CountryInfoViewModel;
		#endregion

		#region Constructor
		public CountryInfoView()
		{
			InitializeComponent();
		}
		#endregion
	}
}
