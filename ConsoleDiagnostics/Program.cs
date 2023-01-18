using System;
using ReliaCoat.Common.UI.Extensions.Utilities;

namespace ConsoleDiagnostics
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 10; i++)
            {
                var colorContext = ColorGenerator.generateRandomColor(LightnessFilter.Bright, HueFilter.All);
            
                Console.WriteLine($"R = {colorContext.solidColor.R}, " +
                                  $"G = {colorContext.solidColor.G}, " +
                                  $"B = {colorContext.solidColor.B}");
            }

            Console.ReadKey();
        }
    }
}
