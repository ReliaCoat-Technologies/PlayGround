using System;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Solvers;
using MathNet.Numerics.Providers.LinearAlgebra;
using MathNet.Symbolics;

namespace ANTLRWorkspace
{
    public class Program
    {
        static void Main(string[] args)
        {
            var x = Expression.Symbol("x");
            var y = Expression.Symbol("y");

            Console.WriteLine(Infix.Format(x = 4 * y));

            Console.ReadKey();
        }
    }
}
