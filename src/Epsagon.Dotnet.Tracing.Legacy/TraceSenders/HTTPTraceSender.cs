using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;

using RestSharp;

using Serilog;

namespace Epsagon.Dotnet.Tracing.Legacy.TraceSenders {
    public class HTTPTraceSender : ITraceSender {
        public string CollectorUrl { get; set; }
        public string Token { get; set; }

        public HTTPTraceSender(string collectorUrl, string token) {
            this.CollectorUrl = collectorUrl;
            this.Token = token;
        }

        public void SendTrace(EpsagonTrace trace) {
            Utils.DebugLogIfEnabled("sending trace, url: {url}", this.CollectorUrl);

            var client = new RestClient(this.CollectorUrl) { Timeout = Utils.CurrentConfig.SendTimeout * 1000 };
            var request = new RestRequest(Method.POST);

            request
                .AddHeader("Authorization", $"Bearer {this.Token}")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(Utils.SerializeObject(trace));

            var res = client.Execute(request);

            Utils.DebugLogIfEnabled("sent trace, {trace}", Utils.SerializeObject(trace));

            if (!res.IsSuccessful) {
                Utils.DebugLogIfEnabled("request - method: {@method}, endpoint: {@ep}, timeout: {@to}",
                                        request.Method, request.Resource, request.Timeout);
                Utils.DebugLogIfEnabled("request body: {@body}", request.Body);
                Utils.DebugLogIfEnabled("response - headers: {@h}, status: {@s}, error: {@e}",
                                        res.Headers, res.StatusCode, res.ErrorMessage);
                Utils.DebugLogIfEnabled("response content: {@content}", res.Content);
            }

            JaegerTracer.Clear();
        }
    }
}
