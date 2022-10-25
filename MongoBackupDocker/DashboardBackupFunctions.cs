using System.Diagnostics;
using System.IO.Compression;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;
using File = System.IO.File;

namespace MongoBackupDocker;

public static class DashboardBackupFunctions
{
    private static readonly ILogger Log = LoggerFactory.getLogger(typeof(DashboardBackupFunctions));
    private const string backupDirectory = "/backups";

    public static async Task doDatabaseBackupAsync()
    {
        if (!Directory.Exists(backupDirectory))
        {
            Log.Information("Creating backups directory");

            Directory.CreateDirectory(backupDirectory);
        }

        var zipFileName = DateTime.Now.ToString("yyyy_MMdd_HH:mm:ss");
        var zipFilePath = Path.Combine(backupDirectory, $"{zipFileName}_DatabaseBackup.zip");

        Log.Information("Creating Mongo Backup for {0}", zipFilePath);

        var sw = new Stopwatch();
        sw.Start();

        using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);

        await doMongoDumpAsync(zipArchive);
        await backupFileSystemAsync(zipArchive);

        sw.Stop();

        Log.Information("Backup for {0} Complete. Time Elapsed: {1}", zipFilePath, sw.Elapsed);
    }

    private static async Task doMongoDumpAsync(ZipArchive zipArchive)
    {
        const string host = "192.168.2.152";
        const int port = 27017;
        const string dbName = "RCT_Main";

        var mongoDumpFileName = $"{dbName}.agz";
        var mongoDumpPath = Path.Combine(backupDirectory, mongoDumpFileName);

        var mongoDumpCommand = $"--uri=\"mongodb://{host}:{port}\" --db=\"{dbName}\" --archive=\"{mongoDumpPath}\"";

        Log.Information("Initializing Mongo Dump");

        var sw = new Stopwatch();
        sw.Start();

        var mongoDumpScriptRunner = new LinuxCommandRunner("mongodump", mongoDumpCommand);
        await mongoDumpScriptRunner.executeAsync();

        if (!File.Exists(mongoDumpPath))
        {
            throw new Exception("Mongodump was not created.");
        }

        sw.Stop();

        var lines = mongoDumpScriptRunner
            .standardError
            .Split('\n')
            .Select(x => string.Join(' ', x.Split('\t')
                .Skip(1)
                .ToArray()))
            .ToList();

        foreach (var line in lines)
        {
            Log.Information(line);
        }

        zipArchive.CreateEntryFromFile(mongoDumpPath, mongoDumpFileName);

        Log.Information("Mongo Dump Time: {0}", sw.Elapsed);

        File.Delete(mongoDumpPath);

        

    }

    private static async Task backupFileSystemAsync(ZipArchive zipArchive)
    {
        Log.Information("Beginning Connection to SFTP Server");

        const string host = "192.168.2.152";
        const int port = 22022;
        const string user = "fileSystemManager";
        const string pass = "icprctecp";
        const string rootDirectory = "/files/RCT_Main";

        using var sftpClient = new SftpClient(host, port, user, pass);
        {
            sftpClient.BufferSize = 1024 * 100000; // 1 MB buffer

            try
            {
                sftpClient.Connect();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SFTP connection error.");
                return;
            }

            Log.Information("SFTP Server Connection Successful");

            var filesToDownload = archiveFiles(sftpClient, rootDirectory)
                .ToList();

            await processDirectoryAsync(sftpClient, zipArchive, filesToDownload);
        }
    }

    private static IEnumerable<SftpFile> archiveFiles(SftpClient sftpClient, string directory)
    {
        var files = sftpClient.ListDirectory(directory)
            .Skip(2); // Skip the . and .. directories

        foreach (var file in files)
        {
            if (file.IsRegularFile)
            {
                yield return file;
            }
            else if (file.IsDirectory)
            {
                foreach (var subFile in archiveFiles(sftpClient, file.FullName))
                {
                    yield return subFile;
                }
            }
        }
    }

    private static async Task processDirectoryAsync(SftpClient client, ZipArchive zipArchive, IList<SftpFile> files)
    {
        Log.Information("Found {0} files to archive.", files.Count);

        // Skip current and upper directories.
        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];

            Log.Information("({0} of {1}) File Found {2} ({3} kb)", 
                i + 1,
                files.Count - 2,
                file.Name,
                file.Length / 1024);

            var timer = new Stopwatch();
            timer.Start();

            await processFileAsync(client, zipArchive, file);

            timer.Stop();
            Log.Information("Transfer Time for {0}: {1} ms", file.Name, timer.Elapsed.TotalMilliseconds);
        }
    }

    private static async Task processFileAsync(SftpClient client, ZipArchive zipArchive, SftpFile file)
    {
        var entry = zipArchive.CreateEntry(file.FullName);

        using var sftpStream = client.OpenRead(file.FullName);
        using var zipStream = entry.Open();

        await sftpStream.CopyToAsync(zipStream);
    }
}