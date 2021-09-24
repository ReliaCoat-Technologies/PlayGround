using System;
using ReliaCoat.Numerics;

namespace MathFunctionTester
{
	class Program
	{
		static void Main(string[] args)
		{
			string valueString;

			do
			{
				Console.Write("Enter Value: ");
				valueString = Console.ReadLine();

				double value;

				if (double.TryParse(valueString, out value))
				{
					Console.WriteLine($"Erf({valueString}) = {MathUtils.Erf(value)}");
					Console.WriteLine($"Erfc({valueString}) = {MathUtils.Erfc(value)}");
				}
				else
				{
					Console.WriteLine("Invalid Value Input");
				}

			} while (!string.IsNullOrWhiteSpace(valueString));

			Console.WriteLine("Completed");
		}
	}
}
