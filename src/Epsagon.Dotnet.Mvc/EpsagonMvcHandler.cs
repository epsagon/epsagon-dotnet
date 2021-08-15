using System;
using System.Linq;
using System.Threading.Tasks;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;

using Microsoft.AspNetCore.Http;

using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Mvc {
    public class EpsagonMvcHandler {

        public static async Task<T> Handle<T>(Func<Task<T>> clientFn, HttpContext context) {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
                return await clientFn();

            T result;

            var startTime = new DateTimeOffset(DateTime.UtcNow);
            using (var scope = CreateRunner(context)) {
                result = await ExecuteClientCode(clientFn, scope);
            }

            CreateTrigger(context, startTime);

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace);
            JaegerTracer.Clear();
            EpsagonUtils.ClearTraceUrl();
            return result;
        }

        public static async Task Handle(Func<Task> clientFn, HttpContext context) {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
                await clientFn();

            var startTime = new DateTimeOffset(DateTime.UtcNow);
            using (var scope = CreateRunner(context)) {
                await ExecuteClientCode(clientFn, scope);
            }

            CreateTrigger(context, startTime);

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace);
            JaegerTracer.Clear();
            EpsagonUtils.ClearTraceUrl();
        }

        private static async Task<T> ExecuteClientCode<T>(Func<Task<T>> clientFn, IScope scope) {
            try {
                T result = await clientFn();
                if (result is Task t) {
                    await t.ContinueWith(task => scope.Span.AddException(task.Exception), TaskContinuationOptions.OnlyOnFaulted);
                    return result;
                }

                return result;
            } catch (Exception e) {
                scope.Span.AddException(e);
                throw;
            }
        }

        private static async Task ExecuteClientCode(Func<Task> clientFn, IScope scope) {
            try {
                await clientFn();
            } catch (Exception e) {
                scope.Span.AddException(e);
                throw;
            }
        }

        private static void CreateTrigger(HttpContext context, DateTimeOffset startTime) {
            var operation = context.Request.Method.ToString();
            using (var scope = GlobalTracer.Instance.BuildSpan(operation).WithStartTimestamp(startTime).StartActive(finishSpanOnDispose: true)) {
                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("event.origin", "trigger");
                scope.Span.SetTag("resource.type", "http");
                scope.Span.SetTag("resource.name", context.Request.Host.Value);
                scope.Span.SetTag("resource.operation", operation);
                scope.Span.SetTag("http.host", context.Request.Host.ToString());
                scope.Span.SetTag("http.path", context.Request.Path.ToString());
                scope.Span.SetTag("http.status", context.Response.StatusCode);
                scope.Span.SetTag("http.base_url", context.Request.PathBase.ToString());
                scope.Span.SetTag("http.user_agent", context.Request.Headers["User-Agent"].ToString());
                scope.Span.SetDataIfNeeded("http.request_headers", context.Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString()));
                scope.Span.SetDataIfNeeded("http.response_headers", context.Response.Headers.ToDictionary(header => header.Key, header => header.Value.ToString()));
            }
        }

        private static IScope CreateRunner(HttpContext context) {
            var scope = GlobalTracer.Instance.BuildSpan("invoke").StartActive(finishSpanOnDispose: true);
            string traceId = Guid.NewGuid().ToString();
            string startTime = ((int) DateTime.UtcNow.ToUnixTime()).ToString();

            scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            scope.Span.SetTag("event.origin", "runner");
            scope.Span.SetTag("resource.name", context.Request.Host.Value);
            scope.Span.SetTag("resource.type", "aspnet");
            scope.Span.SetTag("resource.operation", context.Request.Method.ToString());
            scope.Span.SetTag("trace_id", traceId);

            EpsagonUtils.SetTraceUrl(traceId, startTime);
            return scope;
        }
    }
}
