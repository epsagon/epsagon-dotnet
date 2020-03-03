using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using OpenTracing;
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

            using (var scope = CreateRunner(methodName))
            {
                ExecuteClientCode(clientFn, scope);
            }
        }

        public static T Handle<T>(Func<T> clientFn, [CallerMemberName] string methodName = "")
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled) return clientFn();

            T result;
            using (var scope = CreateRunner(methodName))
            {
                result = ExecuteClientCode(clientFn, scope);
                scope.Span.SetDataIfNeeded("aws.lambda.return_value", result);
            }

            return result;
        }

        public static async Task<T> Handle<T>(Func<Task<T>> clientFn, [CallerMemberName] string methodName = "")
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled) return await clientFn();
            T result;
            using (var scope = CreateRunner(methodName))
            {
                result = await ExecuteClientCode(clientFn, scope);
                scope.Span.SetDataIfNeeded("aws.lambda.return_value", result);
            }

            return result;
        }

        private static void CreateTraceAndSend()
        {
            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace);
            JaegerTracer.Clear();
        }

        private static void ExecuteClientCode(Action clientFn, IScope scope)
        {
            try { clientFn(); }
            catch (Exception e)
            {
                scope.Span.AddException(e);
                throw;
            }
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

        private static async Task<T> ExecuteClientCode<T>(Func<Task<T>> clientFn, IScope scope)
        {
            try
            {
                T result = await clientFn();
                return result;
            }
            catch (Exception e)
            {
                scope.Span.AddException(e);
                throw;
            }
        }

        private static IScope CreateRunner(string methodName)
        {
            var scope = GlobalTracer.Instance.BuildSpan("invoke").StartActive(finishSpanOnDispose: true);
            scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            scope.Span.SetTag("event.origin", "runner");
            scope.Span.SetTag("resource.name", methodName);
            scope.Span.SetTag("resource.type", "dotnet_function");
            scope.Span.SetTag("resource.operation", "invoke");
            return scope;
        }
    }
}
