 using System;
 using System.Diagnostics;
 using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using ManagedCuda;
 using ManagedCuda.BasicTypes;
 using ManagedCuda.VectorTypes;

namespace CsProject
{
    class Program
    {
        private static CudaKernel cudaKernel;

        static void initKernels()
        {
            var deviceID = 0;
            var context = new CudaContext(deviceID);
            cudaKernel = context.LoadKernel(@"D:\GitHub\PlayGround\CppProject\Debug\kernel.ptx", "addKernel");
        }

        static void Main(string[] args)
        {
            initKernels();

            Console.WriteLine(cudaKernel.MaxThreadsPerBlock);

            var arraySize = 180000;

            cudaKernel.GridDimensions = 600;
            cudaKernel.BlockDimensions = 300;

            var array1 = Enumerable.Range(1, arraySize).Select(x => x * 100).ToArray();
            var array2 = Enumerable.Range(1, arraySize).Select(x => x * 10).ToArray();

            var b = Stopwatch.StartNew();

            var sumArray2 = new int[arraySize];
            for (int i = 0; i < arraySize; i++)
                sumArray2[i] = array1[i] + array2[i];

            b.Stop();
            Console.WriteLine(b.ElapsedTicks);

            var a = Stopwatch.StartNew();

            CudaDeviceVariable<int> cudaArray1 = array1;
            CudaDeviceVariable<int> cudaArray2 = array2;
            var cudaSumArray = new CudaDeviceVariable<int>(arraySize);

            cudaKernel.Run(cudaArray1.DevicePointer, cudaArray2.DevicePointer, cudaSumArray.DevicePointer);

            int[] sumArray = cudaSumArray;

            a.Stop();
            Console.WriteLine(a.ElapsedTicks);

            var c = Stopwatch.StartNew();

            var sumArray3 = array1.Zip(array2, (x, y) => x + y).ToArray();

            c.Stop();
            Console.WriteLine(c.ElapsedTicks);

            Console.ReadLine();
        }
    }
}
