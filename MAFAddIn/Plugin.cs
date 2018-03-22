using System.AddIn;
using MAFAddInView;

namespace MAFAddIn
{
    [AddIn("Plugin Add In", Version="1.0.0.0")]
    public class Plugin : IAddInView
    {
        public double add(double a, double b) => a + b;
        public double subtract(double a, double b) => a - b;
        public double multiply(double a, double b) => a * b;
        public double divide(double a, double b) => a / b;
    }
}
