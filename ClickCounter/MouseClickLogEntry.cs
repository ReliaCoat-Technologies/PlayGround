using System;
using System.Windows.Forms;

namespace ClickCounter
{
	public class MouseClickLogEntry
	{
		public DateTime dateTime { get; set; }
		public double xLocation { get; set; }
		public double yLocation { get; set; }
		public MouseButtons mouseButton { get; set; }
	}
}