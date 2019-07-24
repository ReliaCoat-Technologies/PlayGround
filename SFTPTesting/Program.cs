using System;

namespace SFTPTesting
{
    public class Program
    {
        private static FileSystemManager _fileSystemManager;

        private const string _sshIpAddress = "192.168.2.152";
        private const int _sshPortNumber = 22;
        private const string _sshUser = "dashboard";
        private const string _sshPassword = "icprctecp";

        private const string serverFileDestination = "/primaryData/dashboardFileSystem/testFile.plo";
        private const string testFileSource = @"D:\PlumeOPT\170906 - PlumeOPT Stress Testing\21123 1-5.plo";
        private const string testDownloadDestination = @"C:\Users\Ari\Desktop\testFile.plo";

        static void Main(string[] args)
        {
            _fileSystemManager = new FileSystemManager(_sshIpAddress, _sshPortNumber, _sshUser, _sshPassword);

            _fileSystemManager.uploadFileAsync(testFileSource, serverFileDestination).Wait();

            _fileSystemManager.downloadFileAsync(serverFileDestination, testDownloadDestination).Wait();

            Console.ReadKey();
        }
    }
}
