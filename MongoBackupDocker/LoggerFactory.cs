using Serilog;

namespace MongoBackupDocker;

public static class LoggerFactory
{
    private const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {ThreadId}] <{ClassType}>  {Message} {Exception}{NewLine}";

    #region Methods
    public static ILogger getLogger<T>()
    {
        var configuration = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.With(new TypeLogEnricher(typeof(T)))
            .WriteTo.Console(outputTemplate: outputTemplate);

        return configuration.CreateLogger();
    }

    public static ILogger getLogger(Type typeInput)
    {
        var configuration = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.With(new TypeLogEnricher(typeInput))
            .WriteTo.Console(outputTemplate: outputTemplate);

        return configuration.CreateLogger();
    }
    #endregion
}