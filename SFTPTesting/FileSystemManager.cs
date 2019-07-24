using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SFTPTesting
{
    public class FileSystemManager
    {
        #region Delegates
        public event Func<bool> overwritePrompt;
        #endregion

        #region Fields
        private readonly SftpClient _sftpClient;
        private readonly Stopwatch _stopwatch;
        #endregion

        #region Constructor
        public FileSystemManager(string hostAddress, int hostPort, string hostUserName, string hostPassword)
        {
            _stopwatch = new Stopwatch();
            _sftpClient = new SftpClient(hostAddress, hostPort, hostUserName, hostPassword);

            Console.WriteLine($"Attempting SFTP Connection to {hostAddress}:{hostPort} as {hostUserName}");

            try
            {
                _stopwatch.Restart();
                _sftpClient.Connect();
                _stopwatch.Stop();

                Console.WriteLine($"Connection Time = {_stopwatch.Elapsed.TotalMilliseconds} ms");
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()}: {e.Message} (Addr: {hostAddress}:{hostPort}, User: {hostUserName})");
            }
        }

        public FileSystemManager(IPEndPoint hostIpEndPoint, string hostUserName, string hostPassword) : this(hostIpEndPoint.Address.ToString(), hostIpEndPoint.Port, hostUserName, hostPassword) { }
        #endregion

        #region Methods
        public async Task uploadFileAsync(string sourcePath, string targetPath, bool autoOverwrite = false)
        {
            if (!File.Exists(sourcePath)) throw new IOException($"File {sourcePath} does not exist on client.");

            var fileInfo = new FileInfo(sourcePath);
            Console.WriteLine($"SFTP file upload from {sourcePath} (File Size = {fileInfo.Length:N0} bytes)");

            using (var fs = new FileStream(sourcePath, FileMode.Open))
                await uploadFileFromStream(fs, targetPath, autoOverwrite);
        }

        public async Task uploadFileFromStream(Stream stream, string targetPath, bool autoOverwrite = false)
        {
            var targetDirectory = Path.GetDirectoryName(targetPath);

            if (!_sftpClient.Exists(targetDirectory))
            {
                Console.WriteLine($"{targetDirectory} does not exist on SFTP server. Creating...");
                _sftpClient.CreateDirectory(targetDirectory);
            }

            if (_sftpClient.Exists(targetPath) && !autoOverwrite)
            {
                var doOverwrite = overwritePrompt?.Invoke();
                if (doOverwrite == false) return;
            }

            _stopwatch.Restart();

            await Task.Run(() =>
            {
                _sftpClient.UploadFile(stream, targetPath);
            });

            _stopwatch.Stop();

            Console.WriteLine($"SFTP Upload time to {targetPath}: {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public async Task downloadFileAsync(string sourcePath, string targetPath, bool autoOverwrite = false)
        {
            if (!_sftpClient.Exists(sourcePath)) throw new IOException($"File {sourcePath} does not exist on SFTP server.");

            var fileInfo = _sftpClient.GetAttributes(sourcePath);
            Console.WriteLine($"SFTP file download from {sourcePath} (File Size = {fileInfo.Size:N0} bytes)");

            var targetDirectory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine($"{targetDirectory} does not exist on client. Creating...");
                _sftpClient.CreateDirectory(targetDirectory);
            }

            if (File.Exists(targetPath) && !autoOverwrite)
            {
                var doOverwrite = overwritePrompt?.Invoke();
                if (doOverwrite == false) return;
            }

            using (var fs = new FileStream(targetPath, FileMode.Create))
                await downloadFileToStreamAsync(sourcePath, fs);
        }

        public async Task downloadFileToStreamAsync(string sourcePath, Stream stream)
        {
            _stopwatch.Restart();

            await Task.Run(() =>
            {
                _sftpClient.DownloadFile(sourcePath, stream);
            });

            _stopwatch.Stop();

            Console.WriteLine($"SFTP download time from {sourcePath}: {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }
        #endregion
    }
}