using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWcfConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TestWcfService.Service1();
            Console.WriteLine(client.sum(1, 2));
            Console.ReadLine();
        }
    }
}
