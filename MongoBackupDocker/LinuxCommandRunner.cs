using System.Diagnostics;

namespace MongoBackupDocker;

public class LinuxCommandRunner
{
    #region Fields
    private readonly Process _process;
    #endregion

    #region Properties
    public string command { get; }
    public string args { get; }    
    public string standardOutput { get; private set; }
    public string standardError { get; private set; }
    #endregion

    #region Constructor
    public LinuxCommandRunner(string command, string args = "")
    {
        this.args = args;
        this.command = command;

        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
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

        standardOutput = await _process.StandardOutput.ReadToEndAsync();
        standardError = await _process.StandardError.ReadToEndAsync();

        _process.Dispose();
    }
    #endregion
}