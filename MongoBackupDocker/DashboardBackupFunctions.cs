using System.Diagnostics;
using System.IO.Compression;
using System.Security;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;

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

        var sw = new Stopwatch();
        sw.Start();

        using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);

        await doMongoDumpAsync(zipArchive);
        await backupFileSystemAsync(zipArchive);

        sw.Stop();
        Log.Information("Backup Complete. Time Elapsed: {0}", sw.Elapsed);
    }

    public static async Task doMongoDumpAsync(ZipArchive zipArchive)
    {
        const string host = "192.168.2.152";
        const int port = 27017;
        const string dbName = "RCT_Main";

        var mongoDumpFileName = $"{dbName}.agz";
        var mongoDumpPath = Path.Combine(backupDirectory, mongoDumpFileName);

        var whoAmIScript = new BashScriptRunner("cd /");
        await whoAmIScript.executeAsync();

        var mongoDumpCommand = $"mongodump --uri=\"mongodb://{host}:{port}\" --db=\"{dbName}\" --archive=\"{mongoDumpPath}\"";

        var mongoDumpScriptRunner = new BashScriptRunner(mongoDumpCommand);
        await mongoDumpScriptRunner.executeAsync();

        if (!File.Exists(mongoDumpPath))
        {
            throw new Exception("Mongodump was not created.");
        }

        zipArchive.CreateEntryFromFile(mongoDumpPath, mongoDumpFileName);

        File.Delete(mongoDumpPath);
    }

    public static async Task backupFileSystemAsync(ZipArchive zipArchive)
    {
        Log.Information("Beginning Connection to SFTP Server");

        const string host = "192.168.2.152";
        const int port = 22022;
        const string user = "fileSystemManager";
        const string pass = "icprctecp";

        var client = new SftpClient(host, port, user, pass);

        try
        {
            client.Connect();
        }
        catch(Exception ex)
        {
            Log.Error(ex, "SFTP connection error.");
            return;
        }
        
        Log.Information("SFTP Server Connection Successful");

        var directories = client.ListDirectory("/files/RCT_Main");

        foreach (var directory in directories.Skip(2))
        {
            if (directory.IsDirectory)
            {
                await processDirectoryAsync(directory, client, zipArchive);
            }
        }
    }

    private static async Task processDirectoryAsync(SftpFile directory, ISftpClient client, ZipArchive zipArchive)
    {
        Log.Information("Directory Found {0}", directory.FullName);

        var files = client.ListDirectory(directory.FullName);

        foreach (var item in files.Skip(2))
        {
            if (item.IsRegularFile)
            {
                await processFileAsync(client, item, zipArchive);
            }
        }
    }

    private static async Task processFileAsync(ISftpClient client, SftpFile file, ZipArchive zipArchive)
    {
        Log.Information("File Found {0}", file.FullName);

        var entry = zipArchive.CreateEntry(file.FullName);

        var timer = new Stopwatch();
        timer.Start();
        
        await using var sftpStream = client.Open(file.FullName, FileMode.Open);
        await using var zipStream = entry.Open();
        
        var buffer = new byte[sftpStream.Length];

        await sftpStream.ReadAsync(buffer, 0, (int)sftpStream.Length);
        await zipStream.WriteAsync(buffer);
        
        timer.Stop();
        Log.Information("File Transfer Time for {0}: {1} ms", file.FullName, timer.Elapsed.TotalMilliseconds);
    }
}