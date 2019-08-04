using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;
using OpenTracing;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;

namespace Epsagon.Dotnet.Lambda
{
    public abstract class LambdaHandler<TEvent, TRes>
    {
        public LambdaHandler()
        {
            EpsagonUtils.RegisterServices();
            EpsagonPipelineCustomizer.PatchPipeline();
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
            var config = EpsagonUtils.GetConfiguration(GetType());
            var logger = EpsagonUtils.GetLogger<LambdaHandler<TEvent, TRes>>();
            var returnValue = default(TRes);

            logger.LogDebug("Epsagon Handler Started, configuration: {@Config}", config);

            using (var handler = new LambdaTriggerHandler<TEvent, TRes>(input, context))
            {
                handler.HandleBefore();
                returnValue = this.HandlerFunction(input, context);
                handler.HandleAfter(returnValue);
            }

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            logger.LogDebug("trace object: {@Trace}", EpsagonUtils.SerializeObject(trace));

            return returnValue;
        }
    }
}

// TODO: epsagonEvent.ErrorCode = tags.GetValue<int>("event.error_code");
// TODO: epsagonEvent.Origin = tags.GetValue<string>("event.origin");
