using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ILGPU;
using ILGPU.Runtime;

namespace ReliaCoat.GpuDetectorNetFramework
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public ObservableCollection<Device> devices { get; }
        public ObservableCollection<Accelerator> accelerators { get; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            var context = Context.Create(builder => builder.AllAccelerators());

            devices = new ObservableCollection<Device>(context.Devices);
            accelerators = new ObservableCollection<Accelerator>(devices.Select(x => x.CreateAccelerator(context)));
        }
        #endregion
    }
}