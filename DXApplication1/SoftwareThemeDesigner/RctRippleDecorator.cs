using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SoftwareThemeDesigner
{
	public class RctRippleDecorator : ContentControl
	{
		#region Fields
		private Ellipse _ellipse;
		private Grid _grid;
		private Storyboard _storyBoard;
		#endregion

		#region DependencyProperties
		public static readonly DependencyProperty highlightBackgroundProperty = DependencyProperty.Register(nameof(highlightBackground), typeof(Brush), typeof(RctRippleDecorator),
			new FrameworkPropertyMetadata(Brushes.White));
		#endregion

		#region Properites
		public Brush highlightBackground
		{
			get { return (Brush)GetValue(highlightBackgroundProperty); }
			set { SetValue(highlightBackgroundProperty, value); }
		}
		#endregion

		#region Constructor
		static RctRippleDecorator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctRippleDecorator), new FrameworkPropertyMetadata(typeof(RctRippleDecorator)));
		}
		#endregion

		#region Methods
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_ellipse = GetTemplateChild("PART_Ellipse") as Ellipse;
			_grid = GetTemplateChild("PART_Grid") as Grid;
			_storyBoard = _grid.FindResource("PART_Storyboard") as Storyboard;
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);

			Console.WriteLine("Beginning Storyboard");

			var targetWidth = Math.Max(ActualWidth, ActualHeight) * 2;
			var mousePosition = (e as MouseButtonEventArgs).GetPosition(this);
			var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
			//set initial margin to mouse position
			_ellipse.Margin = startMargin;
			//set the to value of the animation that animates the width to the target width
			(_storyBoard.Children[0] as DoubleAnimation).To = targetWidth;
			//set the to and from values of the animation that animates the distance relative to the container (grid)
			(_storyBoard.Children[1] as ThicknessAnimation).From = startMargin;
			(_storyBoard.Children[1] as ThicknessAnimation).To =
				new Thickness(mousePosition.X - targetWidth / 2, mousePosition.Y - targetWidth / 2, 0, 0);
			_ellipse.BeginStoryboard(_storyBoard);
		}
		#endregion
	}
}
