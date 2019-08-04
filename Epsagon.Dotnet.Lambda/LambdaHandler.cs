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
    public abstract class LambdaHandler<TReq, TRes>
    {
        private static bool _coldstart = true;

        private static bool ColdStart
        {
            get
            {
                var value = _coldstart;
                _coldstart = false;
                return value;
            }
        }

        public LambdaHandler() : base()
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
        public abstract TRes HandlerFunction(TReq input, ILambdaContext context);

        /// <summary>
        /// Epsagon enabled lambda handler based on <see cref="HandlerFunction(TReq, ILambdaContext)"/>
        /// implemented in a derived class
        /// </summary>
        /// <param name="input">input event from AWS Lambda</param>
        /// <param name="context">lambda context</param>
        /// <returns></returns>
        private TRes EpsagonEnabledHandler(TReq input, ILambdaContext context)
        {
            var config = EpsagonUtils.GetConfiguration(GetType());
            var logger = EpsagonUtils.GetLogger<LambdaHandler<TReq, TRes>>();

            logger.LogDebug("Epsagon Handler Started, configuration: {@Config}", config);

            var scope = EpsagonUtils.GetService<ITracer>()
                                           .BuildSpan(input.GetType().Name)
                                           .StartActive(finishSpanOnDispose: true);
            scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            scope.Span.SetTag("event.start_time", DateTime.UtcNow.ToBinary());
            scope.Span.SetTag("resource.name", context.FunctionName);
            scope.Span.SetTag("resource.type", "lambda");
            scope.Span.SetTag("aws.operation", "invoke");
            scope.Span.SetTag("aws.lambda.memory", context.MemoryLimitInMB.ToString());
            scope.Span.SetTag("aws.lambda.function_version", context.FunctionVersion);
            scope.Span.SetTag("aws.lambda.log_group_name", context.LogGroupName);
            scope.Span.SetTag("aws.lambda.log_stream_name", context.LogStreamName);
            scope.Span.SetTag("aws.lambda.cold_start", ColdStart);

            var timer = Stopwatch.StartNew();
            var returnValue = this.HandlerFunction(input, context);
            timer.Stop();

            scope.Span.SetTag("event.duration", timer.ElapsedMilliseconds);
            scope.Span.SetTag("aws.lambda.return_value", JsonConvert.SerializeObject(returnValue));

            scope.Dispose();

            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            logger.LogDebug("trace object: {@Trace}", JsonConvert.SerializeObject(trace, new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            }));

            return returnValue;
        }
    }
}

// epsagonEvent.ErrorCode = tags.GetValue<int>("event.error_code");
// epsagonEvent.Origin = tags.GetValue<string>("event.origin");
// epsagonEvent.Resource.Metadata.AwsAccount = tags.GetValue<string>("aws.account");
