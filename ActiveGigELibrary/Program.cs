using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveGigELibrary
{
    class Program
    {
        private static CameraCapture cameraCapture;
        private static int i = 0;

        static void Main(string[] args)
        {
            cameraCapture = new CameraCapture();
            cameraCapture.activeGige.CaptureCompleted += ActiveGige_CaptureCompleted;
            Console.ReadLine();
        }

        private static void ActiveGige_CaptureCompleted(int frames)
        {
            i++;
            Console.WriteLine($"Acquired, {i}, {frames}");
        }

        private static void ActiveGige_FrameAcquiredX()
        {
            i++;
            Console.WriteLine($"Acquired, {i}");
        }

        private static void ActiveGigeOnFrameAcquired()
        {
            i++;
            Console.WriteLine($"Acquired, {i}");
        }
    }
}
