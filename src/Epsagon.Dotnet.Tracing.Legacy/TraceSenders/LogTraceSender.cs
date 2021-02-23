using System;
using System.Text;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;

using Serilog;

namespace Epsagon.Dotnet.Tracing.Legacy.TraceSenders {
    public class LogTraceSender : ITraceSender {
        public void SendTrace(EpsagonTrace trace) {
            Utils.DebugLogIfEnabled("sending trace, url: {url}");

            var traceJsonString = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(trace)));
            System.Console.WriteLine("EPSAGON_TRACE: {0}", traceJsonString);

            Utils.DebugLogIfEnabled("sent trace, {trace}", Utils.SerializeObject(trace));
            Utils.DebugLogIfEnabled("trace base64, {trace}", traceJsonString);

            JaegerTracer.Clear();
        }
    }
}
