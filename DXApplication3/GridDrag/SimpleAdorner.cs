using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GridDrag
{
    public class SimpleAdorner : Adorner
    {
        #region Fields
        private VisualCollection children;
        private Thumb bottom, right;
        #endregion

        #region Delegates
        public EventHandler<DragDeltaEventArgs> dragging;
        public EventHandler<DragCompletedEventArgs> dragCompleted;
        #endregion

        #region Properties
        protected override int VisualChildrenCount => children.Count;
        #endregion

        #region Constructor
        public SimpleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            children = new VisualCollection(this);

            buildAdornerEdge(ref bottom, Cursors.SizeNS);
            buildAdornerEdge(ref right, Cursors.SizeWE);

            bottom.DragDelta += bottomDrag;
            right.DragDelta += rightDrag;

            bottom.DragCompleted += (s, e) => dragCompleted?.Invoke(this, e);
            right.DragCompleted += (s, e) => dragCompleted?.Invoke(this, e);
        }
        #endregion

        #region Methods
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            var renderWidth = AdornedElement.RenderSize.Width;
            var renderHeight = AdornedElement.RenderSize.Height;

            var adornerWidth = DesiredSize.Width;
            var adornerHeight = DesiredSize.Height;

            bottom.Arrange(new Rect(renderWidth / 2 - adornerWidth / 2, renderHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            right.Arrange(new Rect(renderWidth - adornerWidth / 2, renderHeight / 2 - adornerHeight / 2, adornerWidth, adornerHeight));

            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            var pen = new Pen(new SolidColorBrush(Colors.LimeGreen), 1);

            dc.DrawRectangle(new SolidColorBrush(Colors.Transparent), pen, new Rect(new Point(0,0),
                new Point(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height)) );
        }

        private void buildAdornerEdge(ref Thumb edgeThumb, Cursor customizedCursor)
        {
            if (edgeThumb != null) return;

            edgeThumb = new Thumb
            {
                Cursor = customizedCursor,
                Height = 10,
                Width = 10,
                Opacity = 1,
                Background = new SolidColorBrush(Colors.DodgerBlue),
            };

            children.Add(edgeThumb);
        }

        private void bottomDrag(object sender, DragDeltaEventArgs e)
        {
            dragging?.Invoke(this, e);
        }

        private void rightDrag(object sender, DragDeltaEventArgs e)
        {
            dragging?.Invoke(this, e);
        }

        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }
        #endregion
    }
}