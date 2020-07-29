using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Mvc
{
    public class EpsagonMvcHandler
    {

        public static T Handle<T>(Func<T> clientFn,  HttpContext context)
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled) return clientFn();

            T result;
            using (var scope = CreateRunner(context))
            {
                result = ExecuteClientCode(clientFn, scope);
            }
            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace);
            JaegerTracer.Clear();
            return result;
        }

        private static T ExecuteClientCode<T>(Func<T> clientFn, IScope scope)
        {
            try
            {
                T result = clientFn();
                if (result is Task t)
                {
                    t.ContinueWith(task => scope.Span.AddException(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                    return result;
                }

                return result;
            }
            catch (Exception e)
            {
                scope.Span.AddException(e);
                throw;
            }
        }


        private static IScope CreateRunner(HttpContext context)
        {
            var scope = GlobalTracer.Instance.BuildSpan("invoke").StartActive(finishSpanOnDispose: true);
            scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            scope.Span.SetTag("event.origin", "runner");
            scope.Span.SetTag("resource.name", context.Request.Host.Value);
            scope.Span.SetTag("resource.type", "dotnet_function");
            scope.Span.SetTag("resource.operation", context.Request.Method.ToString());
            return scope;
        }
    }
}
