using System.Diagnostics;
using Serilog;
using Serilog.Core;

namespace MongoBackupDocker;

public class LinuxCommandRunner
{
    #region Statics
    private static readonly ILogger Log = LoggerFactory.getLogger<LinuxCommandRunner>();
    #endregion

    #region Fields
    private readonly Process _process;
    #endregion

    #region Properties
    public string command { get; }
    public string args { get; }    
    public bool redirectOutput { get; }
    public string standardOutput { get; private set; }
    public string standardError { get; private set; }
    #endregion

    #region Constructor
    public LinuxCommandRunner(string command, string args = "", bool redirectOutput = false)
    {
        this.args = args;
        this.command = command;

        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = redirectOutput,
            RedirectStandardError = redirectOutput,
            UseShellExecute = false,
            CreateNoWindow = false,
        };

        _process = new Process
        {
            StartInfo = startInfo,
        };
    }
    #endregion

    #region Methods
    public async Task executeAsync()
    {
        _process.Start();
        
        await _process.WaitForExitAsync();

        if (redirectOutput)
        {
            standardOutput = await _process.StandardOutput.ReadToEndAsync();
            standardError = await _process.StandardError.ReadToEndAsync();
        }

        _process.Dispose();
    }
    #endregion
}