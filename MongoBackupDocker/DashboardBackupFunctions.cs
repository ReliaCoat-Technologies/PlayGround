using System.Diagnostics;
using System.IO.Compression;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;

namespace MongoBackupDocker;

public static class DashboardBackupFunctions
{
    private static readonly ILogger Log = LoggerFactory.getLogger(typeof(DashboardBackupFunctions));
    private const string backupDirectory = "/backups";

    private static string mongoHostAddress;
    private static int mongoHostPort;
    private static string mongoDatabase;
    private static string sftpHostAddress;
    private static int sftpHostPort;
    private static string sftpUser;
    private static string sftpPassword;
    private static string fileSystemRoot;

    public static async Task doDatabaseBackupAsync()
    {
        mongoHostAddress = Environment.GetEnvironmentVariable("MONGO_HOST_ADDRESS");
        mongoHostPort = int.Parse(Environment.GetEnvironmentVariable("MONGO_HOST_PORT"));
        mongoDatabase = Environment.GetEnvironmentVariable("MONGO_DBNAME");
        sftpHostAddress = Environment.GetEnvironmentVariable("SFTP_HOST_ADDRESS");
        sftpHostPort = int.Parse(Environment.GetEnvironmentVariable("SFTP_HOST_PORT"));
        sftpUser = Environment.GetEnvironmentVariable("SFTP_USERNAME");
        sftpPassword = Environment.GetEnvironmentVariable("SFTP_PASSWORD");
        fileSystemRoot = Environment.GetEnvironmentVariable("FILE_SYSTEM_ROOT");

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
        var mongoDumpFileName = $"{mongoDatabase}.agz";
        var mongoDumpPath = Path.Combine(backupDirectory, mongoDumpFileName);

        var mongoDumpCommand = $"--uri=\"mongodb://{mongoHostAddress}:{mongoHostPort}\" --db=\"{mongoDatabase}\" --archive=\"{mongoDumpPath}\"";

        Log.Information("Initializing Mongo Dump for database: {0}", mongoDatabase);

        var sw = new Stopwatch();
        sw.Start();

        var mongoDumpScriptRunner = new LinuxCommandRunner("mongodump", mongoDumpCommand);
        await mongoDumpScriptRunner.executeAsync();

        if (!File.Exists(mongoDumpPath))
        {
            throw new Exception("Mongodump was not created.");
        }

        sw.Stop();

        zipArchive.CreateEntryFromFile(mongoDumpPath, mongoDumpFileName);

        Log.Information("Mongo Dump Time: {0}", sw.Elapsed);

        File.Delete(mongoDumpPath);
    }

    private static async Task backupFileSystemAsync(ZipArchive zipArchive)
    {
        Log.Information("Beginning Connection to SFTP Server");

        // const string host = "192.168.2.152";
        // const int port = 22022;
        // const string user = "fileSystemManager";
        // const string pass = "icprctecp";
        var rootDirectory = Path.Combine(fileSystemRoot, mongoDatabase);

        using var sftpClient = new SftpClient(sftpHostAddress, sftpHostPort, sftpUser, sftpPassword);
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