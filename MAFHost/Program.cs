using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.Linq;
using MAFHostView;

namespace MAFHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // assumes directory is app folder, contants pipeline folder structure
            var addInRoot = @"C:\Users\Ari\Desktop\Pipeline";

            // Update cache files of pipeline segments
            var warnings = AddInStore.Update(addInRoot);

            foreach (var warning in warnings)
                Console.WriteLine(warning);

            // Search for add-ins
            var tokens = AddInStore.FindAddIns(typeof(IHostView), addInRoot);

            // Ask which add-ins to use
            var token = chooseToken(tokens);

            var calc = token.Activate<IHostView>(AddInSecurityLevel.Internet);

            runToken(calc);
        }

        private static AddInToken chooseToken(IEnumerable<AddInToken> tokenInput)
        {
            while (true)
            {
                var tokens = tokenInput.ToList();

                if (!tokens.Any())
                {
                    Console.WriteLine("No tokens available");
                    return null;
                }

                Console.WriteLine("Available tokens:");

                for (var i = 0; i < tokens.Count; i++)
                {
                    Console.WriteLine($"\t{i}: {tokens[i].Name}" + $"\t{tokens[i].AddInFullName}" + $"\t{tokens[i].AssemblyName}" + $"\t{tokens[i].Description}" + $"\t{tokens[i].Version}" + $"\t{tokens[i].Publisher}");
                }

                Console.WriteLine("Which calculator do you want to use?");

                var line = Console.ReadLine();

                int selection;

                if (int.TryParse(line, out selection))
                    if (selection < tokens.Count)
                        return tokens[selection];

                Console.WriteLine($"Invalid Selection: {selection}. Please choose again.", line);
                tokenInput = tokens;
            }
        }

        private static void runToken(IHostView token)
        {
            if (token == null)
                Console.ReadLine();

            Console.WriteLine("Available operations: +, -, *, /");
            Console.WriteLine("Request a calculation , such as: 2 + 2");
            Console.WriteLine("Type \"exit\" to exit");
            var line = Console.ReadLine();

            while (!line.Equals("exit"))
            {
                // The Parser class parses the user's input.
                try
                {
                    Parser c = new Parser(line);
                    switch (c.Action)
                    {
                        case "+":
                            Console.WriteLine(token.add(c.A, c.B));
                            break;
                        case "-":
                            Console.WriteLine(token.subtract(c.A, c.B));
                            break;
                        case "*":
                            Console.WriteLine(token.multiply(c.A, c.B));
                            break;
                        case "/":
                            Console.WriteLine(token.divide(c.A, c.B));
                            break;
                        default:
                            Console.WriteLine("{0} is an invalid command. Valid commands are +,-,*,/", c.Action);
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid command: {0}. Commands must be formated: [number] [operation] [number]", line);
                }

                line = Console.ReadLine();
            }
        }


        internal class Parser
        {
            double a;
            double b;
            string action;

            internal Parser(string line)
            {
                string[] parts = line.Split(' ');
                a = double.Parse(parts[0]);
                action = parts[1];
                b = double.Parse(parts[2]);
            }

            public double A
            {
                get { return a; }
            }

            public double B
            {
                get { return b; }
            }

            public string Action
            {
                get { return action; }
            }
        }
    }
}
