using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SoftwareThemeDesigner
{
	public class RctRippleEffectDecorator : ContentControl
	{
		#region Fields
		private Ellipse _ellipse;
		private Grid _grid;
		private Storyboard _storyBoard;
		#endregion

		#region Dependency Properties
		public static readonly DependencyProperty highlightBackgroundProperty = DependencyProperty.Register(nameof(highlightBackground), typeof(Brush), typeof(RctRippleEffectDecorator),
			new PropertyMetadata(Brushes.White));
		#endregion

		#region Properties

		public Brush highlightBackground
		{
			get { return (Brush)GetValue(highlightBackgroundProperty); }
			set { SetValue(highlightBackgroundProperty, value); }
		}
		#endregion

		#region Constructor
		static RctRippleEffectDecorator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RctRippleEffectDecorator), new FrameworkPropertyMetadata(typeof(RctRippleEffectDecorator)));
		}
		#endregion

		#region Methods

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_ellipse = GetTemplateChild("PART_Ellipse") as Ellipse;
			_grid = GetTemplateChild("PART_Grid") as Grid;
			_storyBoard = _grid.FindResource("PART_Animation") as Storyboard;

			AddHandler(MouseDownEvent, new RoutedEventHandler(onMouseDown), true);
		}

		private void onMouseDown(object sender, RoutedEventArgs e)
		{
			var targetWidth = Math.Max(ActualWidth, ActualHeight) * 2;
			var mousePosition = (e as MouseButtonEventArgs).GetPosition(this);
			var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);

			_ellipse.Margin = startMargin;

			(_storyBoard.Children[0] as DoubleAnimation).To = targetWidth;
			(_storyBoard.Children[1] as ThicknessAnimation).From = startMargin;
			(_storyBoard.Children[1] as ThicknessAnimation).To = new Thickness(mousePosition.X - targetWidth/2, mousePosition.Y - targetWidth/2, 0, 0);
			_ellipse.BeginStoryboard(_storyBoard);
		}
		#endregion
	}
}
