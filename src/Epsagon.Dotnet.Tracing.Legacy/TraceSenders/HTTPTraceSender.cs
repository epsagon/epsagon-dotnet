using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using RestSharp;
using Serilog;

namespace Epsagon.Dotnet.Tracing.Legacy.TraceSenders
{
    public class HTTPTraceSender : ITraceSender
    {
        public string CollectorUrl { get; set; }
        public string Token { get; set; }

        public HTTPTraceSender(string collectorUrl, string token)
        {
            this.CollectorUrl = collectorUrl;
            this.Token = token;
        }

        public void SendTrace(EpsagonTrace trace)
        {
            Log.Debug("sending trace, url: {url}", this.CollectorUrl);

            var client = new RestClient(this.CollectorUrl) { Timeout = 5000 };
            var request = new RestRequest(Method.POST);

            request
                .AddHeader("Authorization", $"Bearer {this.Token}")
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
