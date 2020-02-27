using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Lambda
{
    public class EpsagonGeneralHandler
    {
        public static void Handle(Action clientFn, [CallerMemberName] string methodName = "")
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
            {
                clientFn();
                return;
            }

            CreateRunner(methodName);
            try { ExecuteClientCode(clientFn); }
            finally { CreateTraceAndSend(); }
        }

        public static T Handle<T>(Func<T> clientFn, [CallerMemberName] string methodName = "")
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled) return clientFn();
            CreateRunner(methodName);

            T result;
            try { result = ExecuteClientCode(clientFn); }
            finally { CreateTraceAndSend(); }
            return result;
        }

        public static async Task<T> Handle<T>(Func<Task<T>> clientFn, [CallerMemberName] string methodName = "")
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled) return await clientFn();
            CreateRunner(methodName);

            T result;
            try { result = await ExecuteClientCode(clientFn); }
            finally { CreateTraceAndSend(); }
            return result;
        }

        private static void CreateTraceAndSend()
        {
            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace);
            JaegerTracer.Clear();
        }

        private static void ExecuteClientCode(Action clientFn)
        {
            using (var span = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                try { clientFn(); }
                catch (Exception e)
                {
                    span.Span.AddException(e);
                    throw;
                }
            }
        }

        private static T ExecuteClientCode<T>(Func<T> clientFn)
        {
            using (var span = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                try
                {
                    T result = clientFn();
                    if (result is Task t) return result;
                    else
                    {
                        span.Span.SetDataIfNeeded("meta.return_value", result);
                        return result;
                    }
                }
                catch (Exception e)
                {
                    span.Span.AddException(e);
                    throw;
                }
            }
        }

        private static async Task<T> ExecuteClientCode<T>(Func<Task<T>> clientFn)
        {
            using (var span = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                try
                {
                    T result = await clientFn();
                    span.Span.SetDataIfNeeded("meta.return_value", result);
                    return result;
                }
                catch (Exception e)
                {
                    span.Span.AddException(e);
                    throw;
                }
            }
        }

        private static void CreateRunner(string methodName)
        {
            using (var span = GlobalTracer.Instance.BuildSpan("invoke").StartActive(finishSpanOnDispose: true))
            {
                span.Span.SetTag("event.id", Guid.NewGuid().ToString());
                span.Span.SetTag("event.origin", "runner");
                span.Span.SetTag("resource.name", methodName);
                span.Span.SetTag("resource.type", "dotnet_function");
                span.Span.SetTag("resource.operation", "invoke");
            }
        }
    }
}
