using System;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TelnetTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var tcpClient = new TcpClient { NoDelay = true })
            {
                // currently set to the IP of the hi-bay feeder
                tcpClient.Connect("169.254.1.20", 10001);

                using (var stream = tcpClient.GetStream())
                {
                    var inputString = Console.ReadLine();

                    if (stream.CanWrite)
                    {
                        var writeString = inputString + Environment.NewLine;
                        var writeBuffer = Encoding.ASCII.GetBytes(writeString);
                        stream.Write(writeBuffer, 0, writeBuffer.Length);
                    }

                    // Sometimes the response stream doesn't give a full response string,
                    // so this concatenates each fragment of the response string until the stream is empty.
                    while (true)
                    {
                        if (!stream.CanRead) continue;

                        var byteArray = new byte[tcpClient.ReceiveBufferSize];
                        stream.Read(byteArray, 0, byteArray.Length);

                        // string.Trim() doesn't seem to work, so I just got rid of all "0" bytes from the byteArray
                        var modifiedArray = byteArray.Where(x => x > 0).Select(x => x).ToArray();
                        var outputString = Encoding.ASCII.GetString(modifiedArray);
                        Console.Write(outputString);
                        if (outputString == string.Empty)
                            break;
                    }
                }
            }

            Console.ReadLine();
        }
    }
}