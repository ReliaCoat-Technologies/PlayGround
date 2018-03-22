using System.AddIn.Pipeline;
using MAFContract;
using MAFHostView;

namespace MAFHostSideAdapter
{
    /// <summary>
    /// The host side adapter, implements the host view, has the contract and the contract handle fields
    /// </summary>
    [HostAdapter]
    public class HostSideAdapter : IHostView
    {
        private IContractTest _contract;
        private ContractHandle _handle;

        public HostSideAdapter(IContractTest contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public double add(double a, double b) => _contract.add(a, b);
        public double subtract(double a, double b) => _contract.subtract(a, b);
        public double multiply(double a, double b) => _contract.multiply(a, b);
        public double divide(double a, double b) => _contract.divide(a, b);
    }
}
