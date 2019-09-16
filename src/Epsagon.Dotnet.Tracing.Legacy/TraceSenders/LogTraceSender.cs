using System;
using System.Text;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Serilog;

namespace Epsagon.Dotnet.Tracing.Legacy.TraceSenders
{
    public class LogTraceSender : ITraceSender
    {
        public void SendTrace(EpsagonTrace trace)
        {
            Log.Debug("sending trace, url: {url}");

            var traceJsonString = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(trace)));
            System.Console.WriteLine("EPSAGON_TRACE: {0}", traceJsonString);

            Log.Debug("sent trace, {trace}", Utils.SerializeObject(trace));
            Log.Debug("trace base64, {trace}", traceJsonString);

            JaegerTracer.Clear();
        }
    }
}
