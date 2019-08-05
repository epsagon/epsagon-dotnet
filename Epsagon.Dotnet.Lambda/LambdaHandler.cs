using Amazon.Lambda.Core;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using Serilog;
using OpenTracing.Util;
using Epsagon.Dotnet.Instrumentation;

namespace Epsagon.Dotnet.Lambda
{
    public abstract class LambdaHandler<TEvent, TRes>
    {

        public LambdaHandler()
        {
            EpsagonBootstrap.Bootstrap();
        }

        /// <summary>
        /// in a derived class this handler function is the base for epsagon's
        /// handler function, write business logic in this function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract TRes HandlerFunction(TEvent input, ILambdaContext context);

        /// <summary>
        /// Epsagon enabled lambda handler based on <see cref="HandlerFunction(TReq, ILambdaContext)"/>
        /// implemented in a derived class
        /// </summary>
        /// <param name="input">input event from AWS Lambda</param>
        /// <param name="context">lambda context</param>
        /// <returns></returns>
        private TRes EpsagonEnabledHandler(TEvent input, ILambdaContext context)
        {
            Log.Debug("entered epsagon lambda handler");

            var returnValue = default(TRes);

            // handle trigger event
            using (var scope = GlobalTracer.Instance.BuildSpan("").StartActive(finishSpanOnDispose: true))
            {
                var trigger = TriggerFactory.CreateInstance(input.GetType(), input);
                trigger.Handle(context, scope);
            }

            // handle invocation event
            using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context))
            {
                handler.HandleBefore();
                returnValue = this.HandlerFunction(input, context);
                handler.HandleAfter(returnValue);
            }

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            EpsagonTrace.SendTrace(trace, "us-east-1");
            JaegerTracer.Clear();

            Log.Debug("finishing epsagon lambda handler");

            return default(TRes);
        }

    }
}
