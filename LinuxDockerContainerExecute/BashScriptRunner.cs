using System.Diagnostics;

namespace LinuxDockerContainerExecute;

public class BashScriptRunner
{
    #region Fields
    private readonly Process _process;
    #endregion

    #region Properties
    public string fileName { get; }
    public string command { get; }    
    public string standardOutput { get; private set; }
    public string standardError { get; private set; }
    #endregion

    #region Constructor
    public BashScriptRunner(string fileName, string command)
    {
        this.command = command;
        this.fileName = fileName;

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = command,
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