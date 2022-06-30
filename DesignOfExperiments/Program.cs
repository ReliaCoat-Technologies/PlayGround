using System;
using System.Linq;
using ReliaCoat.Statistics;

namespace DesignOfExperiments
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var numParameters = 6;
            var sizeOfFraction = 2;

            var parameters = Enumerable.Range(0, numParameters)
                .Select(x => new DesignParameter<double>(x.ToString(), -1, 1))
                .ToArray();

            var doe = FactorialDesignOfExperimentsCreator.designExperiment(sizeOfFraction, false, parameters);

            Console.WriteLine($"Number of Parameters: {doe.numParameters}");
            Console.WriteLine($"Size of Fraction: {doe.sizeOfFraction}");
            Console.WriteLine($"Number of Experiments: {doe.numExperiments}");
            Console.WriteLine($"Resolution of DOE: {doe.resolution}");

            foreach (var experiment in doe.experimentConditions)
            {
                Console.WriteLine(experiment.ToString());
            }

            Console.ReadKey();
        }
    }
}
