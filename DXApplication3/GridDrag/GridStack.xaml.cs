using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;

namespace GridDrag
{
    public partial class GridStack : UserControl
    {
        #region Fields
        private bool _isDragging;
        #endregion

        #region Constructor
        public GridStack()
        {
            InitializeComponent();

            addRowDefinition(8);
        }
        #endregion

        #region Methods
        public void addRowDefinition(int numRows = 1)
        {
            if (numRows < 1) return;

            for (var i = 0; i < numRows; i++)
            {
                var rowDefinition = new RowDefinition { MinHeight = 80 };
                parentGrid.RowDefinitions.Add(rowDefinition);
            }
        }

        public void addGridItem(int column, int row, int colSpan, int rowSpan)
        {
            var borderToAdd = new Border
            {
                Margin = new Thickness(5),
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(2),
                Background = Brushes.Black
            };

            borderToAdd.MouseEnter += activateAdorners;

            Grid.SetColumn(borderToAdd, column);
            Grid.SetRow(borderToAdd, row);
            Grid.SetColumnSpan(borderToAdd, colSpan);
            Grid.SetRowSpan(borderToAdd, rowSpan);

            parentGrid.Children.Add(borderToAdd);
        }

        private IEnumerable<ElementGridSpace> getPanelGridSpaces()
        {
            return parentGrid.Children
                .OfType<Border>()
                .Where(x => x != traceBorder)
                .Select(x => new ElementGridSpace(x));
        }

        private void activateAdorners(object sender, MouseEventArgs e)
        {
            if (_isDragging) return;

            clearAdorners();

            var border = sender as Border;

            if (border == null) return;

            var adornerLayers = AdornerLayer.GetAdornerLayer(border);
            var adorner = new SimpleAdorner(border);

            adorner.dragStarted += onDragStarted;
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

        private void onDragStarted(object sender, DragStartedEventArgs e)
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
        }

        private void onCenterAdornerDragging(object sender, DragDeltaEventArgs e)
        {
            var adorner = sender as Adorner;

            if (adorner == null) return;

            var border = getAdornerParentBorder(adorner);

            if (border == null) return;

            var column = Grid.GetColumn(border);
            var row = Grid.GetRow(border);
            var columnSpan = Grid.GetColumnSpan(border);
            var rowSpan = Grid.GetRowSpan(border);

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

            while (remainingValue > getCellDimensionValue?.Invoke(actualCellDimensionValue) / 2)
            {
                var currentCellDimensionValue = getCellDimensionValue?.Invoke(actualCellDimensionValue);

                remainingValue -= currentCellDimensionValue.Value;
                newCellDimensionValue++;
            }

            return newCellDimensionValue > dimensionDefinitionCount - cellDimensionSpan ?
                dimensionDefinitionCount - cellDimensionSpan
                : newCellDimensionValue;
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

            while (remainingValue > getCellDimensionValue?.Invoke(cellDimensionNumber) / 2)
            {
                if (currentCell >= dimensionDefinitionCount) break;

                var currentCellDimensionValue = getCellDimensionValue?.Invoke(cellDimensionNumber);

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

            var traceBorderColumn = Grid.GetColumn(traceBorder);
            var traceBorderRow = Grid.GetRow(traceBorder);
            var traceBorderColumnSpan = Grid.GetColumnSpan(traceBorder);
            var traceBorderRowSpan = Grid.GetRowSpan(traceBorder);

            moveTraceBorderOverlaidElements(border);

            Grid.SetColumn(border, traceBorderColumn);
            Grid.SetRow(border, traceBorderRow);
            Grid.SetColumnSpan(border, traceBorderColumnSpan);
            Grid.SetRowSpan(border, traceBorderRowSpan); ;

            clearAdorners();
        }

        public void moveTraceBorderOverlaidElements(UIElement originalElement)
        {
            var traceBorderOverlayElements = findOverlayingElements(traceBorder, originalElement);

            if (!traceBorderOverlayElements.Any()) return;

            var newRow = new ElementGridSpace(traceBorder).bottomRow + 1;

            foreach (var overlayingElement in traceBorderOverlayElements)
                moveOverlaidElementsDown(overlayingElement.element, newRow, originalElement);
        }

        public void moveOverlaidElementsDown(UIElement overlayElementInput, int newRow, params UIElement[] dontMoveElements)
        {
            Grid.SetRow(overlayElementInput, newRow);
            
            var inputOverlayBottomRow = new ElementGridSpace(overlayElementInput).bottomRow;

            // Recursive!
            foreach (var overlayingElement in findOverlayingElements(overlayElementInput, dontMoveElements))
            {
                var overlayElementNewRow = inputOverlayBottomRow + 1;    
                moveOverlaidElementsDown(overlayingElement.element, overlayElementNewRow);
            }
        }

        private IList<ElementGridSpace> findOverlayingElements(UIElement element, params UIElement[] excludeElements)
        {
            var elementGridSpace = new ElementGridSpace(element);

            return getPanelGridSpaces()
                .Where(x => x.element != element)
                .Where(x => !excludeElements.Contains(x.element))
                .Where(x => elementGridSpace.intersects(x))
                .ToList();
        }
        #endregion
    }
}
