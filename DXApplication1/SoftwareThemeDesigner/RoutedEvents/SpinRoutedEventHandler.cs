using System.Windows;

namespace SoftwareThemeDesigner.RoutedEvents
{
	public delegate void SpinRoutedEventHandler(object sender, SpinRoutedEventArgs e);

	public class SpinRoutedEventArgs : RoutedEventArgs
	{
		#region Properties
		public double oldValue { get; }
		public double newValue { get; }
		#endregion

		#region Constructors

		public SpinRoutedEventArgs(double oldValue, double newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public SpinRoutedEventArgs(RoutedEvent routedEvent, double oldValue, double newValue) : base(routedEvent)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public SpinRoutedEventArgs(RoutedEvent routedEvent, object source, double oldValue, double newValue) : base(routedEvent, source)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#endregion
	}
}