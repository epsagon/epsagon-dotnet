using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Epsagon.Dotnet.Core;
using RestSharp;
using OpenTracing;
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


// lambda invoked -> bootstrap epsagon -> handle invocation event -> handle lambda event -> handle operation event -> send trace
