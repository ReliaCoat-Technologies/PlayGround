using Serilog.Core;
using Serilog.Events;

namespace MongoBackupDocker;

public class TypeLogEnricher : ILogEventEnricher
{
    public Type typeParameter { get; }

    public TypeLogEnricher(Type typeParameter)
    {
        this.typeParameter = typeParameter;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClassType", typeParameter.Name));
    }
}