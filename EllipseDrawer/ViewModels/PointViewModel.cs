using EllipseDrawer.Utilities;
using ReactiveUI;

namespace EllipseDrawer.ViewModels
{
    public class PointViewModel : ReactiveObject
    {
        #region Fields
        private double _x;
        private double _y;
        #endregion

        #region Properties
        public double X
        {
            get => _x;
            set => this.RaiseAndSetIfChanged(ref _x, value);
        }
        public double Y
        {
            get => _y;
            set => this.RaiseAndSetIfChanged(ref _y, value);
        }
        #endregion

        #region Methods
        public DoublePoint2D getPoint()
        {
            return new DoublePoint2D(X, Y);
        }
        #endregion
    }
}