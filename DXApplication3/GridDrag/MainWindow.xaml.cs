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
            if (_isDragging) return;

            clearAdorners();

            var border = sender as Border;

            if (border == null) return;

            var adornerLayers = AdornerLayer.GetAdornerLayer(border);
            var adorner = new SimpleAdorner(border);

            adorner.centerDragging += onCenterAdornerDragging;
            adorner.edgeDragging += onEdgeAdornerDragging;
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

        private void onCenterAdornerDragging(object sender, DragDeltaEventArgs e)
        {
            var adorner = sender as Adorner;

            if (adorner == null) return;

            var border = getAdornerParentBorder(adorner);

            if (border == null) return;

            _isDragging = true;

            if (traceBorder.Visibility == Visibility.Hidden)
                traceBorder.Visibility = Visibility.Visible;

            var column = Grid.GetColumn(border);
            var row = Grid.GetRow(border);
            var columnSpan = Grid.GetColumnSpan(border);
            var rowSpan = Grid.GetRowSpan(border);

            Grid.SetColumn(traceBorder, column);
            Grid.SetRow(traceBorder, row);
            Grid.SetColumnSpan(traceBorder, columnSpan);
            Grid.SetRowSpan(traceBorder, rowSpan);

            var newColumn = getAdjustedCellValue(column,
                columnSpan,
                parentGrid.ColumnDefinitions.Count,
                e.HorizontalChange,
                i => parentGrid.ColumnDefinitions[i].ActualWidth);

            Grid.SetColumn(traceBorder, newColumn);

            var newRow = getAdjustedCellValue(row,
                rowSpan,
                parentGrid.RowDefinitions.Count,
                e.VerticalChange,
                i => parentGrid.RowDefinitions[i].ActualHeight);

            Grid.SetRow(traceBorder, newRow);
        }

        private void onEdgeAdornerDragging(object sender, DragDeltaEventArgs e)
        {
            var adorner = sender as Adorner;

            if (adorner == null) return;

            var border = getAdornerParentBorder(adorner);

            if (border == null) return;

            _isDragging = true;

            if (traceBorder.Visibility == Visibility.Hidden)
                traceBorder.Visibility = Visibility.Visible;

            var column = Grid.GetColumn(border);
            var row = Grid.GetRow(border);
            var columnSpan = Grid.GetColumnSpan(border);
            var rowSpan = Grid.GetRowSpan(border);

            Grid.SetColumn(traceBorder, column);
            Grid.SetRow(traceBorder, row);
            Grid.SetColumnSpan(traceBorder, columnSpan);
            Grid.SetRowSpan(traceBorder, rowSpan);

            var newColumnSpan = getAdjustedSpan(border.ActualWidth,
                column,
                parentGrid.ColumnDefinitions.Count,
                e.HorizontalChange,
                i => parentGrid.ColumnDefinitions[i].ActualWidth);

            Grid.SetColumnSpan(traceBorder, newColumnSpan);

            var newRowSpan = getAdjustedSpan(border.ActualHeight,
                row,
                parentGrid.RowDefinitions.Count,
                e.VerticalChange,
                i => parentGrid.RowDefinitions[i].ActualHeight);

            Grid.SetRowSpan(traceBorder, newRowSpan);
        }

        public static int getAdjustedCellValue(int actualCellDimensionValue,
            int cellDimensionSpan,
            int dimensionDefinitionCount,
            double deltaCellDimensionValue,
            Func<int, double> getCellDimensionValue)
        {
            var currentCellDimensionPosition = 0d;

            for (var i = 0; i < actualCellDimensionValue; i++)
            {
                var currentCellDimensionValue = getCellDimensionValue?.Invoke(actualCellDimensionValue);

                if (!currentCellDimensionValue.HasValue)
                    throw new Exception("Could not get cell dimension width/height");

                currentCellDimensionPosition += currentCellDimensionValue.Value;
            }

            var adjustedCellPosition = currentCellDimensionPosition + deltaCellDimensionValue;

            var remainingValue = adjustedCellPosition;
            var newCellDimensionValue = 0;

            while (remainingValue > 10)
            {
                var currentCellDimensionValue = getCellDimensionValue?.Invoke(actualCellDimensionValue);

                if (!currentCellDimensionValue.HasValue)
                    throw new Exception("Could not get cell dimension width/height");

                remainingValue -= currentCellDimensionValue.Value;
                newCellDimensionValue++;
            }

            return newCellDimensionValue > dimensionDefinitionCount - cellDimensionSpan ?
                dimensionDefinitionCount - cellDimensionSpan
                : newCellDimensionValue;
        }

        private static int getAdjustedSpan(double actualCellDimensionValue,
            int cellDimensionNumber,
            int dimensionDefinitionCount,
            double deltaCellDimensionValue,
            Func<int, double> getCellDimensionValue)
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

            if (newSpanValue > dimensionDefinitionCount - cellDimensionNumber)
                newSpanValue = dimensionDefinitionCount - cellDimensionNumber;
            else if (newSpanValue == 0)
                newSpanValue = 1;

            return newSpanValue;
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
            _isDragging = false;

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
