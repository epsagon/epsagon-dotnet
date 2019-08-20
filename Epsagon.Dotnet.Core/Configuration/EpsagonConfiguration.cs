using System;

namespace Epsagon.Dotnet.Core.Configuration
{
    public class EpsagonConfiguration : IEpsagonConfiguration
    {
        public string Token { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_TOKEN") ?? "";
        public string AppName { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_APP_NAME") ?? "Application";
        public bool MetadataOnly { get; set; } = (Environment.GetEnvironmentVariable("EPSAGON_METADATA") ?? "").ToUpper() == "TRUE";
        public bool UseSSL { get; set; }
        public string TraceCollectorURL { get; set; }
        public bool IsEpsagonDisabled { get; set; } = (Environment.GetEnvironmentVariable("DISABLE_EPSAGOND") ?? "").ToUpper() == "TRUE";

        public EpsagonConfiguration()
        {
            if ((Environment.GetEnvironmentVariable("EPSAGON_TEST") ?? "").ToUpper() == "TRUE")
            {
                TraceCollectorURL = "http://dev.tc.epsagon.com";
            }
            else if ((Environment.GetEnvironmentVariable("EPSAGON_META") ?? "").ToUpper() == "TRUE") {
                TraceCollectorURL = "https://meta.tc.epsagon.com";
            }
            else
            {
                var region = Environment.GetEnvironmentVariable("AWS_REGION");
                TraceCollectorURL = $"https://{region}.tc.epsagon.com";
            }
        }
    }
}
