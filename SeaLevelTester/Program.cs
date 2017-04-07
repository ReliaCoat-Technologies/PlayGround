using System;
using System.Threading;
using Sealevel;

namespace SeaLevelTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // The e150 has 4 ports. The first feeder uses port 1
            // the second uses port 2.
            // 1 turns the stirrer on, 0 turns the stirrer off.
            // Port numbers should not be higher than 1.
            int port1 = 0, port2 = 0; // both are off
            var ipAddress = "169.254.1.40";

            Console.WriteLine("-- SeaLevel eI/O Modbus Tester --");

            var device = connectToDevice("169.254.1.40");

            if (device == null)
            {
                Console.ReadLine();
                return;
            }

            port1 = 1; // Turn first stirrer on
            sendCommandToStirrer(device, port1, port2);

            Thread.Sleep(5000);

            port1 = 0; // Turn first stirrer off
            sendCommandToStirrer(device, port1, port2);

            Console.ReadLine();
        }

        public static SeaMAX connectToDevice(string ipAddress)
        {
            // Refer to the SeaLevel SeaMAX Software API Manual, Page 14 (Section 4.6)
            // on how to reference the SeaMAX dot not.dll and implement the .COM modules
            // SeaMAX.dll and ftd2xx.dll files. The 64-bit DLLs are in the program Files folder
            // (Program Files x86)\Sealevel Systems\SeaMAX\Dll\x64
            // ******** MAKE SURE TO CHANGE ftd2xx64.dll to ftd2xx.dll **********
            // (or you will get a DLL not found exception).
            var device = new SeaMAX();

            // SM_Open takes any string (it can be "COM1" if it's connected via serial port
            // For TCP you just need an ip address. All relevant functions for our purposes return
            // and Int32, which indicates whether or not an operation has succeeded.
            // If the operation has succeeded the number is positive. If not, it's negative.
            // See Page 89 (Section 10.1.2.2) for error reference.
            var verifyDeviceOpen = device.SM_Open(ipAddress);

            Console.WriteLine(verifyDeviceOpen == 0
                ? "Device Connection Successful"
                : $"Device Connection Unsucccessful - Code {verifyDeviceOpen}");

            return verifyDeviceOpen == 0 ? device : null;
        }

        private static int sendCommandToStirrer(SeaMAX device, int port1, int port2)
        {
            // Command only requires 1 byte to control all 4 ports.
            // Each byte is capable of controlling 8 ports on an eIO Device.
            var command = new byte[1];

            // Converting the command to a string representing a binary number
            // port 1 on = 1
            // port 2 on = 2
            // port 3 on = 4
            // port 4 on = 8
            var commandString = $"{port2}{port1}";

            // Converting the binary to a byte
            command[0] = Convert.ToByte(commandString, 2);

            // Refer to 10.1.2.27 on page 107 ("WriteDigitalOutputs")
            // the "start" parameter is the first port, the "number" parameter is the
            // number of ports being managed. Ports are a zero-based value where port
            // 1 represents 0, and numbers are the number of ports being managed in
            // sequential order. So SM_WriteDigitalOutputs(0, 2) implies we start at port 1
            // and we are working with 2 ports, so this command works for both port 1 and port 2.
            // SM_WriteDigitalOutputs(1, 3) implies working with ports 2, 3 and 4.
            // SM_WriteDigitalOutputs(3, 2) would cause an error because you start at port 4, but
            // there is no port 5.
            var result = device.SM_WriteDigitalOutputs(0, 2, command);

            Console.WriteLine(result >= 0
                ? "Command Successfully Performed"
                : $"Error, Code {result}");

            return result;
        }
    }
}