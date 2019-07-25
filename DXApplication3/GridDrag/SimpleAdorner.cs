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
        private Thumb center, bottomRight;
        #endregion

        #region Delegates
        public EventHandler<DragStartedEventArgs> dragStarted;
        public EventHandler<DragDeltaEventArgs> centerDragging;
        public EventHandler<DragDeltaEventArgs> edgeDragging;
        public EventHandler<DragCompletedEventArgs> dragCompleted;
        #endregion

        #region Properties
        protected override int VisualChildrenCount => children.Count;
        #endregion

        #region Constructor
        public SimpleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            children = new VisualCollection(this);

            buildAdornerCenter(ref center);
            buildAdornerEdge(ref bottomRight, Cursors.SizeNWSE);

            center.DragStarted += (s, e) => dragStarted?.Invoke(this, e);
            bottomRight.DragStarted += (s, e) => dragStarted?.Invoke(this, e);

            center.DragDelta += (s, e) => centerDragging?.Invoke(this, e); ;
            bottomRight.DragDelta += (s, e) => edgeDragging?.Invoke(this, e); ;

            center.DragCompleted += (s, e) => dragCompleted?.Invoke(this, e);
            bottomRight.DragCompleted += (s, e) => dragCompleted?.Invoke(this, e);
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

            center.Arrange(new Rect(0, 0, renderWidth, renderHeight));
            bottomRight.Arrange(new Rect(renderWidth - adornerWidth / 2, renderHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            return finalSize;
        }

        private void buildAdornerCenter(ref Thumb centerThumb)
        {
            if (centerThumb != null) return;

            centerThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Opacity = 0.4,
                Background = new SolidColorBrush(Colors.Gray)
            };

            children.Add(centerThumb);
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

        protected override Visual GetVisualChild(int index)
        {
            return children[index];
        }
        #endregion
    }
}