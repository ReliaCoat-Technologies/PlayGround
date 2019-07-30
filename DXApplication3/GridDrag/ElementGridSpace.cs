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
        public ElementGridSpace(UIElement element)
        {
            this.element = element;
            leftColumn = Grid.GetColumn(element);
            topRow = Grid.GetRow(element);
            rightColumn = leftColumn + Grid.GetColumnSpan(element) - 1;
            bottomRow = topRow + Grid.GetRowSpan(element) - 1;

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
            // Test the corners of the element with more coordinates
            // Against all points of the element with fewer points
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
        private static bool isOverlaid(ElementGridSpace allTestElement, ElementGridSpace cornerTestElement)
        {
            return new[]
            {
                allTestElement.hasCoordinates(cornerTestElement.leftColumn, cornerTestElement.topRow),
                allTestElement.hasCoordinates(cornerTestElement.leftColumn, cornerTestElement.rightColumn),
                allTestElement.hasCoordinates(cornerTestElement.rightColumn, cornerTestElement.topRow),
                allTestElement.hasCoordinates(cornerTestElement.rightColumn, cornerTestElement.rightColumn),
            }.Any(x => x);
        }
        #endregion
    }
}