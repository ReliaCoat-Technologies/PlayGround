using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using ReliaCoat.Common.UI.Controls.Interfaces;

namespace SoftwareThemeDesignerTester.ViewModels
{
	public class CountryInfoViewModel : ViewModelBase, IGridStackItem
	{
		#region Delegates
		public event EventHandler itemDeleteClicked;
		#endregion

		#region Properties
		public Country country { get; set; }
		public Int32Rect region => new Int32Rect(column, row, columnSpan, rowSpan);
		public int column { get; set; }
		public int row { get; set; }
		public int columnSpan { get; set; }
		public int rowSpan { get; set; }
		#endregion

		#region Commands
		public ICommand deleteItemCommand { get; }
		#endregion

		#region Constructor
		public CountryInfoViewModel()
		{
			deleteItemCommand = new DelegateCommand(deleteItem);
		}
		#endregion

		#region Methods
		private void deleteItem()
		{
			itemDeleteClicked?.Invoke(this, EventArgs.Empty);
		}
		#endregion
	}
}