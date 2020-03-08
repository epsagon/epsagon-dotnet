namespace Epsagon.Dotnet.Core.Configuration
{
    public interface IEpsagonConfiguration
    {
        string Token { get; }
        string AppName { get; }
        bool MetadataOnly { get; }
        bool UseSSL { get; }
        string TraceCollectorURL { get; }
        string OpenTracingCollectorURL { get; }
        bool IsEpsagonDisabled { get; }
        bool UseLogsTransport { get; }
    }
}
