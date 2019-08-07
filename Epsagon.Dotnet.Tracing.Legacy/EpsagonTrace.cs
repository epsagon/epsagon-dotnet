using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Core;
using RestSharp;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Serilog;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonTrace
    {
        public string AppName { get; set; }
        public IEnumerable<EpsagonEvent> Events { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
        public string Platform { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }

        public EpsagonTrace(
            string appName,
            string platform,
            string token,
            string version,
            IEnumerable<EpsagonEvent> events,
            IEnumerable<Exception> exceptions
        ) {
            this.AppName = appName;
            this.Platform = platform;
            this.Token = token;
            this.Version = version;
            this.Events = events;
            this.Exceptions = exceptions;
        }
        public static void SendTrace(EpsagonTrace trace, string region)
        {
            var config = Utils.CurrentConfig;
            Log.Debug("sending trace, url: {url}", config.TraceCollectorURL);

            var client = new RestClient(config.TraceCollectorURL) { Timeout = 5000 };
            var request = new RestRequest(Method.POST);

            request
                .AddHeader("Authorization", $"Bearer {config.Token}")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(Utils.SerializeObject(trace));

            var res = client.Execute(request);

            Log.Debug("sent trace, {trace}", Utils.SerializeObject(trace));
            Log.Debug("request: {@request}", request);
            Log.Debug("response: {@response}", res);

            JaegerTracer.Clear();
        }
    }
}
