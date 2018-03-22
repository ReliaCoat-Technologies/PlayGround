using System.AddIn.Pipeline;
using MAFAddInView;
using MAFContract;

namespace MAFAddInSideAdapter
{
    [AddInAdapter]
    public class AddInSideAdapter : ContractBase, IContractTest
    {
        private readonly IAddInView _view;

        public AddInSideAdapter(IAddInView view)
        {
            // the add-in adapter has its view as a property
            _view = view;
        }

        public virtual double add(double a, double b) => _view.add(a, b);
        public virtual double subtract(double a, double b) => _view.subtract(a, b);
        public virtual double multiply(double a, double b) => _view.multiply(a, b);
        public virtual double divide(double a, double b) => _view.divide(a, b);
    }
}
