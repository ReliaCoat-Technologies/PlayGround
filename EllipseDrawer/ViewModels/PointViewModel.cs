using ReactiveUI;
using ReliaCoat.Numerics.CartesianMath;

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
        public XyPoint getPoint()
        {
            return new XyPoint(X, Y);
        }
        #endregion
    }
}