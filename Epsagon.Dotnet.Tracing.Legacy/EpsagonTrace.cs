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
            var config = EpsagonUtils.GetConfiguration(typeof(EpsagonTrace));
            var logger = EpsagonUtils.GetLogger<EpsagonTrace>();

            logger.LogDebug("Sending trace to {url}", $"http://dev.tc.epsagon.com");

            var client = new RestClient($"http://dev.tc.epsagon.com") { Timeout = 5000 };
            var request = new RestRequest(Method.POST);

            request
                .AddHeader("Authorization", $"Bearer {config.Token}")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(EpsagonUtils.SerializeObject(trace));
            var res = client.Execute(request);
            logger.LogDebug("response: {@success}", res);
            JaegerTracer.Clear();
        }
    }
}


// lambda invoked -> bootstrap epsagon -> handle invocation event -> handle lambda event -> handle operation event -> send trace
