using Serilog;

namespace MongoBackupDocker;

public static class ErrorHandlingFunctions
{
    private static readonly ILogger Log = LoggerFactory.getLogger(typeof(ErrorHandlingFunctions));

    public static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.IsTerminating)
        {
            Log.Fatal(args.ExceptionObject as Exception, "Fatal Error Occurred");
        }
        else
        {
            Log.Error(args.ExceptionObject as Exception, "Fatal Error Occurred");
        }
    }
}