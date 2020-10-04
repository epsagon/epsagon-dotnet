using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using OpenTracing.Util;
using Serilog;
using Serilog.Events;

namespace Epsagon.Dotnet.Lambda
{
    public class EpsagonHandler
    {

        public static TRes Handle<TEvent, TRes>(TEvent input, ILambdaContext context, Func<TRes> handlerFn)
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
            {
                return handlerFn();
            }

            var clientCodeExecuted = false;
            var returnValue = default(TRes);
            Exception exception = null;

            try
            {

                Utils.DebugLogIfEnabled("entered epsagon lambda handler");
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
                EpsagonTrace.SendTrace(trace);
                JaegerTracer.Clear();
                Utils.DebugLogIfEnabled("finishing epsagon lambda handler");
            }
            catch (Exception ex)
            {
                HandleInstrumentationError(ex);
            }
            finally
            {
                if (exception != null)
                {
                    throw exception;
                }

                if (!clientCodeExecuted)
                {
                    returnValue = handlerFn();
                }
            }
            return returnValue;
        }

        public static async Task<TRes> Handle<TEvent, TRes>(TEvent input, ILambdaContext context, Func<Task<TRes>> handlerFn)
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
            {
                return await handlerFn();
            }

            var clientCodeExecuted = false;
            var returnValue = default(TRes);
            Exception exception = null;

            try
            {
                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    return await handlerFn();
                }

                Utils.DebugLogIfEnabled("entered epsagon lambda handler");
                Utils.DebugLogIfEnabled("handling trigger event");

                using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
                {
                    var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                    trigger.Handle(context, scope);
                }

                // handle invocation event
                Utils.DebugLogIfEnabled("handling invocation event");
                using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
                using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context, scope))
                {
                    Utils.DebugLogIfEnabled("handling before execution");
                    handler.HandleBefore();

                    Utils.DebugLogIfEnabled("calling client handler");
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


                    Utils.DebugLogIfEnabled("handling after execution");
                    handler.HandleAfter(returnValue);
                }

                Utils.DebugLogIfEnabled("creating trace");
                var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
                EpsagonTrace.SendTrace(trace);
                JaegerTracer.Clear();

                Utils.DebugLogIfEnabled("finishing epsagon lambda handler");
                return returnValue;
            }
            catch (Exception ex) { HandleInstrumentationError(ex); }
            finally
            {
                if (exception != null)
                {
                    throw exception;
                }

                if (!clientCodeExecuted)
                {
                    returnValue = await handlerFn();
                }
            }

            return returnValue;
        }

        public static async Task Handle<TEvent>(TEvent input, ILambdaContext context, Func<Task> handlerFn)
        {
            if (Utils.CurrentConfig == null || Utils.CurrentConfig.IsEpsagonDisabled)
            {
                await handlerFn();
                return;
            }

            var clientCodeExecuted = false;
            Exception exception = null;

            try
            {

                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    await handlerFn();
                }

                Utils.DebugLogIfEnabled("entered epsagon lambda handler");
                Utils.DebugLogIfEnabled("handling trigger event");

                using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
                {
                    var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                    trigger.Handle(context, scope);
                }

                // handle invocation event
                Utils.DebugLogIfEnabled("handling invocation event");
                using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
                using (var handler = new LambdaTriggerHandler<TEvent, string>(input, context, scope))
                {
                    Utils.DebugLogIfEnabled("handling before execution");
                    handler.HandleBefore();

                    Utils.DebugLogIfEnabled("calling client handler");
                    try
                    {
                        clientCodeExecuted = true;
                        await handlerFn();
                    }
                    catch (Exception e)
                    {
                        scope.Span.AddException(e);
                        exception = e;
                    }


                    Utils.DebugLogIfEnabled("handling after execution");
                    handler.HandleAfter("");
                }

                Utils.DebugLogIfEnabled("creating trace");
                var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
                EpsagonTrace.SendTrace(trace);
                JaegerTracer.Clear();

                Utils.DebugLogIfEnabled("finishing epsagon lambda handler");
            }
            catch (Exception ex)
            {
                HandleInstrumentationError(ex);
            }
            finally
            {
                if (exception != null)
                {
                    throw exception;
                }

                if (!clientCodeExecuted)
                {
                    await handlerFn();
                }
            }
        }

        private static void HandleInstrumentationError(Exception ex)
        {
            Utils.DebugLogIfEnabled("Exception thrown during instrumentation code");
            Utils.DebugLogIfEnabled("Exception: {@ex}", ex);

            InstumentationExceptionsCollector.Exceptions.Add(ex);
        }
    }
}
