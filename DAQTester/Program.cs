using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAQTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var daqSomething = new DAQSomething();
            daqSomething.startMeasurement();
            Console.ReadLine();
        }
    }
}
