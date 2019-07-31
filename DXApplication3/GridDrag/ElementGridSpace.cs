using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GridDrag
{
    public class ElementGridSpace
    {
        #region Properties
        public UIElement element { get; }
        public int leftColumn { get; }
        public int topRow { get; }
        public int rightColumn { get; }
        public int bottomRow { get; }
        public IList<int[]> occupiedGridSpaces { get; }
        #endregion

        #region Constructors
        public ElementGridSpace(UIElement element) : this(Grid.GetColumn(element),
            Grid.GetRow(element),
            Grid.GetColumnSpan(element),
            Grid.GetRowSpan(element))
        {
            this.element = element;
        }

        public ElementGridSpace(int column, int row, int columnSpan, int rowSpan)
        {
            leftColumn = column;
            topRow = row;
            rightColumn = leftColumn + columnSpan - 1;
            bottomRow = topRow + rowSpan - 1;

            occupiedGridSpaces = new List<int[]>();

            for (var i = leftColumn; i <= rightColumn; i++)
                for (var j = topRow; j <= bottomRow; j++)
                    occupiedGridSpaces.Add(new[] { i, j });
        }
        #endregion

        #region Methods
        public bool intersects(ElementGridSpace elementGridSpace)
        {
            // For efficiency:
            // This is an O(N^2) complexity operation
            // Reduce complexity by iterating over smaller grid space
            return occupiedGridSpaces.Count >= elementGridSpace.occupiedGridSpaces.Count 
                ? isOverlaid(elementGridSpace, this) 
                : isOverlaid(this, elementGridSpace);
        }

        private bool hasCoordinates(int i, int j)
        {
            return occupiedGridSpaces
                .Where(x => x[0] == i)
                .Any(x => x[1] == j);
        }
        #endregion

        #region Static Methods
        private static bool isOverlaid(ElementGridSpace smallerTestElement, ElementGridSpace largerTestElement)
        {
            for(var i = smallerTestElement.leftColumn; i <= smallerTestElement.rightColumn; i++)
                for (var j = smallerTestElement.topRow; j <= smallerTestElement.bottomRow; j++)
                    if (largerTestElement.hasCoordinates(i, j)) return true;

            return false;
        }
        #endregion
    }
}