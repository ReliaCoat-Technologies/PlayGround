using System.AddIn.Pipeline;

namespace MAFAddInView
{
    /// <summary>
    /// The add-in view, has AddInBase() as an attribute
    /// </summary>
    [AddInBase()]
    public interface IAddInView
    {
        double add(double a, double b);
        double subtract(double a, double b);
        double multiply(double a, double b);
        double divide(double a, double b);
    }
}
