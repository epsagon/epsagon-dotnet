using System;

namespace Epsagon.Dotnet.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EpsagonConfiguration : Attribute, IEpsagonConfiguration
    {   
        public string Token { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_TOKEN") ?? "";
        public string AppName { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_APP_NAME") ?? "Application";
        public bool MetadataOnly { get; set; } = (Environment.GetEnvironmentVariable("EPSAGON_METADATA") ?? "").ToUpper() == "TRUE";
        public bool UseSSL { get; set; }
        public string TraceCollectorURL { get; set; }
        public bool IsEpsagonDisabled { get; set; }
    }
}
