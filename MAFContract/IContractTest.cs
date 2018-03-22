using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace MAFContract
{
    /// <summary>
    /// The contract, implements IContract, has an AddInContract attribute
    /// </summary>
    [AddInContract]
    public interface IContractTest : IContract
    {
        double add(double a, double b);
        double subtract(double a, double b);
        double multiply(double a, double b);
        double divide(double a, double b);
    }
}
