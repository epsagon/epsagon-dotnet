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
            var clientCodeExecuted = false;
            var returnValue = default(TRes);
            Exception exception = null;

            try
            {
                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    return handlerFn();
                }

                Log.Debug("entered epsagon lambda handler");


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

                    try
                    {
                        clientCodeExecuted = true;
                        returnValue = handlerFn();
                    }
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
            }
            catch (Exception ex)
            {
                HandleInstrumentationError(ex);
            }
            finally
            {
                if (!clientCodeExecuted)
                {
                    returnValue = handlerFn();
                }
            }

            return returnValue;
        }

        public static async Task<TRes> Handle<TEvent, TRes>(TEvent input, ILambdaContext context, Func<Task<TRes>> handlerFn)
        {
            var clientCodeExecuted = false;
            var returnValue = default(TRes);
            Exception exception = null;

            try
            {
                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    return await handlerFn();
                }

                Log.Debug("entered epsagon lambda handler");
                Log.Debug("handling trigger event");
                using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
                {
                    var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                    trigger.Handle(context, scope);
                }

                // handle invocation event
                Log.Debug("handling invocation event");
                using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
                using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context, scope))
                {
                    Log.Debug("handling before execution");
                    handler.HandleBefore();

                    Log.Debug("calling client handler");
                    try
                    {
                        clientCodeExecuted = true;
                        returnValue = await handlerFn();
                    }
                    catch (Exception e)
                    {
                        scope.Span.AddException(e);
                        exception = e;
                    }

                    Log.Debug("handling after execution");
                    handler.HandleAfter(returnValue);
                }

                Log.Debug("creating trace");
                var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
                EpsagonTrace.SendTrace(trace, "us-east-1");
                JaegerTracer.Clear();

                Log.Debug("finishing epsagon lambda handler");

                if (exception != null) throw exception;

                return returnValue;
            }
            catch (Exception ex) { HandleInstrumentationError(ex); }
            finally
            {
                if (!clientCodeExecuted)
                {
                    returnValue = await handlerFn();
                }
            }

            return returnValue;
        }

        private static void HandleInstrumentationError(Exception ex)
        {
            Log.Debug("Exception thrown during instrumentation code");
            Log.Debug("Exception: {@ex}", ex);
        }
    }
}
