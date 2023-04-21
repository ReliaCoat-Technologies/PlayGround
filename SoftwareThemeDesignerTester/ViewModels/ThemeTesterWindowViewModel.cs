using ReactiveUI;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class ThemeTesterWindowViewModel : ReactiveObject
	{
		#region Fields
        private double _myValue;
		#endregion

		#region Properties
		public double myValue
		{
			get => _myValue;
			set => this.RaiseAndSetIfChanged(ref _myValue, value);
		}
		#endregion

		#region Constructor
		public ThemeTesterWindowViewModel()
        {
            myValue = 1.5;
        }
		#endregion
	}
}