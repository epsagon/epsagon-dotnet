using System;

namespace Epsagon.Dotnet.Core.Configuration {
    public class EpsagonConfiguration : IEpsagonConfiguration {
        public string Token { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_TOKEN") ?? "";
        public string AppName { get; set; } = Environment.GetEnvironmentVariable("EPSAGON_APP_NAME") ?? "Application";
        public bool MetadataOnly { get; set; } = (Environment.GetEnvironmentVariable("EPSAGON_METADATA") ?? "").ToUpper() == "TRUE";
        public bool UseSSL { get; set; }
        public string TraceCollectorURL { get; set; }
        public string OpenTracingCollectorURL { get; set; }
        public bool IsEpsagonDisabled { get; set; } = (Environment.GetEnvironmentVariable("DISABLE_EPSAGON") ?? "").ToUpper() == "TRUE";
        public bool UseLogsTransport { get; set; } = (Environment.GetEnvironmentVariable("EPSAGON_LOG_TRANSPORT") ?? "").ToUpper() == "TRUE";
        public int SendTimeout { get; set; } = ParseInt(Environment.GetEnvironmentVariable("EPSAGON_SEND_TIMEOUT_SEC") ?? "1");

        public EpsagonConfiguration() {
            if ((Environment.GetEnvironmentVariable("EPSAGON_TEST") ?? "").ToUpper() == "TRUE") {
                TraceCollectorURL = "http://dev.tc.epsagon.com";
                // OpenTracingCollectorURL = "https://dev.otc.epsagon.com/api/traces";
                OpenTracingCollectorURL = "https://5ereq1d4ai.execute-api.us-east-1.amazonaws.com/dev/traces";
            } else if ((Environment.GetEnvironmentVariable("EPSAGON_META") ?? "").ToUpper() == "TRUE") {
                TraceCollectorURL = "https://meta.tc.epsagon.com";
                OpenTracingCollectorURL = "https://meta.otc.epsagon.com/api/traces";
            } else {
                var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
                TraceCollectorURL = $"https://{region}.tc.epsagon.com";
                OpenTracingCollectorURL = $"https://{region}.otc.epsagon.com/api/traces";
            }
        }

        private static int ParseInt(string number) {
            int.TryParse(number, out int result);
            return result;
        }
    }
}
