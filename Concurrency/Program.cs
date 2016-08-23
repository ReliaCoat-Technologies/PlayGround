using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrency
{
    class Program
    {
        private static ConcurrentCircularBufferInt circularBufferInt = new ConcurrentCircularBufferInt(5);
        private static int i;

        static void Main(string[] args)
        {
            circularBufferInt.ValueChanged += WriteResult;

            var ints = new List<int> {1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };
            var tasks = new List<Task>();

            foreach (var item in ints)
                    circularBufferInt.enqueue(item);

            Task.WaitAll(tasks.ToArray());

            Console.ReadLine();
        }

        public static void WriteResult(int input)
        {
            i++;
            Console.WriteLine($"{i}: {input}");
        }
    }
}
