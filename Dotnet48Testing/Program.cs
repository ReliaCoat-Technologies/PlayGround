using System;
using System.Diagnostics;
using ILGPU;
using ILGPU.Runtime;

namespace Dotnet48Testing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting GPU Acceleration Testing");

            using var context = Context.CreateDefault();

            foreach (var device in context)
            {
                using var accelerator = device.CreateAccelerator(context);

                Console.WriteLine(accelerator.Name);
                Console.WriteLine(accelerator.AcceleratorType);

                var sw = new Stopwatch();

                sw.Restart();

                var kernelAction = accelerator.LoadAutoGroupedStreamKernel<Index2D, int, ArrayView2D<int, Stride2D.DenseX>>(Kernel);

                sw.Stop();
                Console.WriteLine($"Kernel Build Time: {sw.ElapsedMilliseconds} ms");

                for (var i = 0; i < 20; i++)
                {
                    Console.WriteLine($"Iteration {i + 1}");

                    sw.Restart();

                    var array = new int[400, 800];

                    using var buffer = accelerator.Allocate2DDenseX(array);
                    buffer.CopyFromCPU(array);

                    sw.Stop();
                    Console.WriteLine($"Memory Allocation Time: {sw.ElapsedMilliseconds} ms");

                    sw.Restart();
                
                    kernelAction?.Invoke(new Index2D(400, 800), 20, buffer);
                    accelerator.Synchronize();
                    buffer.CopyToCPU(array);

                    sw.Stop();
                    Console.WriteLine($"GPU Calc Time: {sw.ElapsedMilliseconds} ms");
                }

                Console.WriteLine();
            }

            Console.ReadKey();
        }

        static void Kernel(Index2D i, int threshold, ArrayView2D<int, Stride2D.DenseX> arrayView)
        {
            if (arrayView[i] == 0)
            {
                arrayView[i] = i.X + i.Y;
            }
        }
    }
}
