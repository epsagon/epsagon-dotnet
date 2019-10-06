using System;
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
            var clientCodeExecuted = false;
            var returnValue = default(TRes);
            Exception exception = null;

            try
            {
                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    return handlerFn();
                }

                if (Log.IsEnabled(LogEventLevel.Debug))
                {
                    Log.Debug("entered epsagon lambda handler");
                }


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

                if (Log.IsEnabled(LogEventLevel.Debug))
                {
                    Log.Debug("finishing epsagon lambda handler");
                }
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
                if (exception != null) throw exception;
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

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("entered epsagon lambda handler");
                    Log.Debug("handling trigger event");
                }
                using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
                {
                    var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                    trigger.Handle(context, scope);
                }

                // handle invocation event
                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("handling invocation event");
                }
                using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
                using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context, scope))
                {
                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("handling before execution");
                    }
                    handler.HandleBefore();

                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("calling client handler");
                    }
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


                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("handling after execution");
                    }
                    handler.HandleAfter(returnValue);
                }

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("creating trace");
                }
                var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
                EpsagonTrace.SendTrace(trace);
                JaegerTracer.Clear();

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("finishing epsagon lambda handler");
                }
                return returnValue;
            }
            catch (Exception ex) { HandleInstrumentationError(ex); }
            finally
            {
                if (!clientCodeExecuted)
                {
                    returnValue = await handlerFn();
                }
                if (exception != null) throw exception;
            }

            return returnValue;
        }

        public static async Task Handle<TEvent>(TEvent input, ILambdaContext context, Func<Task> handlerFn)
        {
            var clientCodeExecuted = false;
            Exception exception = null;

            try
            {
                if (Utils.CurrentConfig.IsEpsagonDisabled)
                {
                    await handlerFn();
                }

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("entered epsagon lambda handler");
                    Log.Debug("handling trigger event");
                }
                using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
                {
                    var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                    trigger.Handle(context, scope);
                }

                // handle invocation event
                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("handling invocation event");
                }
                using (var scope = GlobalTracer.Instance.BuildSpan((typeof(TEvent).Name)).StartActive(finishSpanOnDispose: true))
                using (var handler = new LambdaTriggerHandler<TEvent, string>(input, context, scope))
                {
                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("handling before execution");
                    }
                    handler.HandleBefore();

                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("calling client handler");
                    }
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

                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug("handling after execution");
                    }
                    handler.HandleAfter("");
                }

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("creating trace");
                }
                var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
                EpsagonTrace.SendTrace(trace);
                JaegerTracer.Clear();

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("finishing epsagon lambda handler");
                }
            }
            catch (Exception ex) { HandleInstrumentationError(ex); }
            finally
            {
                if (!clientCodeExecuted)
                {
                    await handlerFn();
                }
                if (exception != null) throw exception;
            }
        }

        private static void HandleInstrumentationError(Exception ex)
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("Exception thrown during instrumentation code");
                Log.Debug("Exception: {@ex}", ex);
            }

            InstumentationExceptionsCollector.Exceptions.Add(ex);
        }
    }
}
