using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using OpenTracing.Util;
using Serilog;

namespace Epsagon.Dotnet.Lambda
{
    public class EpsagonHandler
    {
        public static TRes Handle<TEvent, TRes>(TEvent input, ILambdaContext context, Func<TRes> handlerFn)
        {
            Log.Debug("entered epsagon lambda handler");

            var returnValue = default(TRes);
            Exception exception = null;

            // handle trigger event
            using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                trigger.Handle(context, scope);
            }

            // handle invocation event
            using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
            using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context, scope))
            {
                handler.HandleBefore();

                try { returnValue = handlerFn(); }
                catch (Exception e)
                {
                    scope.Span.AddException(e);
                    exception = e;
                }

                handler.HandleAfter(returnValue);
            }

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace, "us-east-1");
            JaegerTracer.Clear();

            Log.Debug("finishing epsagon lambda handler");

            if (exception != null) throw exception;

            return returnValue;
        }

        public static async Task<TRes> Handle<TEvent, TRes>(TEvent input, ILambdaContext context, Func<Task<TRes>> handlerFn)
        {
            Log.Debug("entered epsagon lambda handler");

            var returnValue = default(TRes);
            Exception exception = null;

            // handle trigger event
            using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                trigger.Handle(context, scope);
            }

            // handle invocation event
            using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
            using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context, scope))
            {
                handler.HandleBefore();

                try { returnValue = await handlerFn(); }
                catch (Exception e)
                {
                    scope.Span.AddException(e);
                    exception = e;
                }

                handler.HandleAfter(returnValue);
            }

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace, "us-east-1");
            JaegerTracer.Clear();

            Log.Debug("finishing epsagon lambda handler");

            if (exception != null) throw exception;

            return returnValue;
        }
    }
}
