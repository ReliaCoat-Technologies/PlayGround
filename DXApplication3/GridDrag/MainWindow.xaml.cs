using System;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using DevExpress.Xpf.Core.Native;

namespace GridDrag
{
    public partial class MainWindow : DXWindow
    {
        private bool _isDragging;
        private Point lastPoint;
        private SimpleAdorner adorner;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void activateAdorners(object sender, MouseEventArgs e)
        {
            clearAdorners();

            var border = sender as Border;

            if (border == null) return;

            var adornerLayers = AdornerLayer.GetAdornerLayer(border);
            var adorner = new SimpleAdorner(border);

            adorner.dragging += onDragging;
            adorner.dragCompleted += onDragCompleted;

            adornerLayers?.Add(adorner);
        }

        private void clearAdorners()
        {
            var borders = parentGrid.VisualChildren()
                .OfType<Border>()
                .Where(x =>
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(x);

                    if (adornerLayer?.GetAdorners(x) == null)
                        return false;

                    return adornerLayer.GetAdorners(x).OfType<SimpleAdorner>().Any();
                })
                .ToList();

            foreach (var borderWithAdorner in borders)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(borderWithAdorner);
                var simpleAdorners = adornerLayer.GetAdorners(borderWithAdorner).OfType<SimpleAdorner>();

                foreach (var simpleAdorner in simpleAdorners)
                    adornerLayer.Remove(simpleAdorner);
            }
        }

        private void onDragging(object sender, DragDeltaEventArgs e)
        {
            var adorner = sender as Adorner;

            if (adorner == null) return;

            var border = getAdornerParentBorder(adorner);

            if (border == null) return;

            if (traceBorder.Visibility == Visibility.Hidden)
                traceBorder.Visibility = Visibility.Visible;

            var column = Grid.GetColumn(border);
            var row = Grid.GetRow(border);

            Grid.SetColumn(traceBorder, column);
            Grid.SetRow(traceBorder, row);

            var newColumnSpan = getAdjustedSpan(border.ActualWidth,
                e.HorizontalChange,
                column,
                parentGrid.ColumnDefinitions.Count,
                i => parentGrid.ColumnDefinitions[i].ActualWidth);

            Grid.SetColumnSpan(traceBorder, newColumnSpan);

            var newRowSpan = getAdjustedSpan(border.ActualHeight,
                e.VerticalChange,
                row,
                parentGrid.RowDefinitions.Count,
                i => parentGrid.RowDefinitions[i].ActualHeight);

            Grid.SetRowSpan(traceBorder, newRowSpan);
        }

        private static int getAdjustedSpan(double actualCellDimensionValue,
            double deltaCellDimensionValue,
            int cellDimensionNumber,
            int dimensionDefinitionCount,
            Func<int, double> getCellDimensionValue
            )
        {
            var adjustedElementValue = actualCellDimensionValue + deltaCellDimensionValue;

            var remainingValue = adjustedElementValue;
            var currentCell = cellDimensionNumber;
            var newSpanValue = 0;

            while (remainingValue > 10)
            {
                if (currentCell >= dimensionDefinitionCount) break;

                var currentCellDimensionValue = getCellDimensionValue?.Invoke(cellDimensionNumber);

                if (!currentCellDimensionValue.HasValue)
                    throw new Exception("Could not get cell dimension width/height");

                remainingValue -= currentCellDimensionValue.Value;

                newSpanValue++;
            }

            return newSpanValue == 0 ? 1 : newSpanValue;
        }

        private Border getAdornerParentBorder(object sender)
        {
            var adorner = sender as SimpleAdorner;

            if (adorner == null) return null;

            var border = parentGrid.VisualChildren()
                .OfType<Border>()
                .FirstOrDefault(x =>
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(x);
                    var adornerArray = adornerLayer?.GetAdorners(x);
                    return adornerArray != null && adornerArray.Contains(adorner);
                });

            return border;
        }

        private void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (traceBorder.Visibility == Visibility.Visible)
                traceBorder.Visibility = Visibility.Hidden;

            var border = getAdornerParentBorder(sender as Adorner);

            if (border == null) return;

            Grid.SetColumn(border, Grid.GetColumn(traceBorder));
            Grid.SetRow(border, Grid.GetRow(traceBorder));
            Grid.SetColumnSpan(border, Grid.GetColumnSpan(traceBorder));
            Grid.SetRowSpan(border, Grid.GetRowSpan(traceBorder));;

            clearAdorners();
        }
    }
}
