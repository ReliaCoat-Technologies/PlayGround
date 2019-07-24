using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
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
            adornerLayers?.Add(adorner);
        }

        private void clearAdorners()
        {
            var borders = parentGrid.VisualChildren()
                .OfType<Border>()
                .Where(x =>
                {
                    var a = AdornerLayer.GetAdornerLayer(x);

                    if (a?.GetAdorners(x) == null)
                        return false;

                    return a.GetAdorners(x).OfType<SimpleAdorner>().Any();
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
    }
}
