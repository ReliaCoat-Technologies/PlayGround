using System.Diagnostics;
using System.IO.Compression;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using Serilog;

namespace MongoBackupDocker;

public class DashboardBackupSession
{
    #region Statics
    private static readonly ILogger Log = LoggerFactory.getLogger(typeof(DashboardBackupSession));
    private const string backupDirectory = "/backups";
    #endregion

    #region Fields
    private readonly string _mongoHostAddress;
    private readonly int _mongoHostPort;
    private readonly string _mongoDatabase;
    private readonly string _sftpHostAddress;
    private readonly int _sftpHostPort;
    private readonly string _sftpUser;
    private readonly string _sftpPassword;
    private readonly string _fileSystemRoot;
    #endregion

    #region Constructor
    public DashboardBackupSession()
    {
        // Populate the environment variables.
        _mongoHostAddress = Environment.GetEnvironmentVariable("MONGO_HOST_ADDRESS");
        _mongoHostPort = int.Parse(Environment.GetEnvironmentVariable("MONGO_HOST_PORT"));
        _mongoDatabase = Environment.GetEnvironmentVariable("MONGO_DBNAME");
        _sftpHostAddress = Environment.GetEnvironmentVariable("SFTP_HOST_ADDRESS");
        _sftpHostPort = int.Parse(Environment.GetEnvironmentVariable("SFTP_HOST_PORT"));
        _sftpUser = Environment.GetEnvironmentVariable("SFTP_USERNAME");
        _sftpPassword = Environment.GetEnvironmentVariable("SFTP_PASSWORD");
        _fileSystemRoot = Environment.GetEnvironmentVariable("FILE_SYSTEM_ROOT");

        Log.Information("Mongo Host Address: {0}", _mongoHostAddress);
        Log.Information("Mongo Host Port: {0}", _mongoHostPort);
        Log.Information("Mongo Database: {0}", _mongoDatabase);
        Log.Information("File System Host Address: {0}", _sftpHostAddress);
        Log.Information("File System Host Port: {0}", _sftpHostPort);
    }
    #endregion

    #region Methods
    public async Task doDatabaseBackupAsync()
    {
        // Create the backups directory.
        if (!Directory.Exists(backupDirectory))
        {
            Log.Information("Creating backups directory");

            Directory.CreateDirectory(backupDirectory);
        }

        // Zip File example: 2022_0605_17:22:35_DatabaseBackup.zip
        var zipFileDateTime = DateTime.Now.ToString("yyyy MMdd");
        var zipFilePath = Path.Combine(backupDirectory, $"{zipFileDateTime} {_mongoDatabase} Backup.zip");

        Log.Information("Creating Mongo Backup for {0}", zipFilePath);

        var sw = new Stopwatch();
        sw.Start();

        // Delete any old file with the same name.
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        // Create a new ZIP file.
        using var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);

        await doMongoDumpAsync(zipArchive);
        await backupFileSystemAsync(zipArchive);

        sw.Stop();

        Log.Information("Backup for {0} Complete. Time Elapsed: {1}", zipFilePath, sw.Elapsed);
    }
    
    private async Task doMongoDumpAsync(ZipArchive zipArchive)
    {
        // File name --> <<dbname>>.agz
        var mongoDumpFileName = $"{_mongoDatabase}.agz";
        var mongoDumpPath = Path.Combine(backupDirectory, mongoDumpFileName);

        var mongoDumpCommand = $"--uri=\"mongodb://{_mongoHostAddress}:{_mongoHostPort}\" " +
                               $"--db=\"{_mongoDatabase}\" " +
                               $"--archive=\"{mongoDumpPath}\""; // Dumps to a single file rather than directories.

        Log.Information("Initializing Mongo Dump for database: {0}", _mongoDatabase);

        var sw = new Stopwatch();
        sw.Start();

        // Execute mongodump on the shell.
        var mongoDumpScriptRunner = new LinuxCommandRunner("mongodump", mongoDumpCommand);
        await mongoDumpScriptRunner.executeAsync();

        // Check if the mongodump is successful.
        if (!File.Exists(mongoDumpPath))
        {
            throw new Exception("Mongodump was not created.");
        }

        sw.Stop();

        // Archive the mongodump file to the ZIP archive.
        zipArchive.CreateEntryFromFile(mongoDumpPath, mongoDumpFileName);

        Log.Information("Mongo Dump Time: {0}", sw.Elapsed);
        
        // Delete the mongodump as its no longer needed.
        File.Delete(mongoDumpPath);
    }

    private async Task backupFileSystemAsync(ZipArchive zipArchive)
    {
        Log.Information("Beginning Connection to SFTP Server");

        // Get the directory of the database. (<fileSystemRoot>/<databasename>)
        var rootDirectory = Path.Combine(_fileSystemRoot, _mongoDatabase);

        using var sftpClient = new SftpClient(_sftpHostAddress, _sftpHostPort, _sftpUser, _sftpPassword);
        {
            // Increase the buffer size to reduce the number of handshakes.
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

            // Get current state of the file system.
            var filesToDownload = getFileArchive(sftpClient, rootDirectory)
                .ToList();

            #if DEBUG
            filesToDownload = filesToDownload.Take(5).ToList();
            #endif 

            await processFilesAsync(sftpClient, zipArchive, filesToDownload);
        }
    }

    private static IEnumerable<SftpFile> getFileArchive(ISftpClient sftpClient, string directory)
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
                // Recurse through all directories of the root folder.
                foreach (var subFile in getFileArchive(sftpClient, file.FullName))
                {
                    yield return subFile;
                }
            }
        }
    }

    private static async Task processFilesAsync(ISftpClient client, ZipArchive zipArchive, IList<SftpFile> files)
    {
        Log.Information("Found {0} files to archive.", files.Count);

        // Skip current and upper directories.
        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];

            Log.Information("({0} of {1}) File Found {2} ({3} kb)", 
                i + 1,
                files.Count,
                file.Name,
                file.Length / 1024);

            var timer = new Stopwatch();
            timer.Start();

            await processFileAsync(client, zipArchive, file);

            timer.Stop();
            Log.Information("Transfer Time for {0}: {1} ms", file.Name, timer.Elapsed.TotalMilliseconds);
        }
    }

    private static async Task processFileAsync(ISftpClient client, ZipArchive zipArchive, SftpFile file)
    {
        // Check if the file exists (state of filesystem may have changed since starting).
        if (!client.Exists(file.FullName))
        {
            Log.Warning("Could not find file: {0}", file.FullName);

            return;
        }

        // Entry name must not have any leading slashes in the ZIP file.
        var entryName = file.FullName.TrimStart('\\', '/');

        // Create a ZIP entry.
        var entry = zipArchive.CreateEntry(entryName);

        // Initialize the SFTP and ZIP archive streams.
        await using var sftpStream = client.OpenRead(file.FullName);
        await using var zipStream = entry.Open();

        // Stream the file from SFTP to the ZIP archive.
        await sftpStream.CopyToAsync(zipStream);
    }
    #endregion
}