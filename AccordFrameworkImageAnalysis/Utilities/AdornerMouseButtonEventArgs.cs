using System;
using System.Windows.Input;

namespace AccordFrameworkImageAnalysis.Utilities
{
	public class AdornerMouseButtonEventArgs : MouseButtonEventArgs
	{
		public ResizePosition position { get; }

		public AdornerMouseButtonEventArgs(ResizePosition position, MouseButtonEventArgs args) : base(args.MouseDevice, args.Timestamp, args.ChangedButton, args.StylusDevice)
		{
			this.position = position;
		}
	}

	public class AdornerMouseMoveEventArgs : MouseEventArgs
	{
		public ResizePosition position { get; }

		public AdornerMouseMoveEventArgs(ResizePosition position, MouseEventArgs args) : base(args.MouseDevice, args.Timestamp, args.StylusDevice)
		{
			this.position = position;
		}
	}
}