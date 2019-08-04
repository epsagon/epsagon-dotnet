using System;
using System.Diagnostics;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Lambda
{
    public class LambdaTriggerHandler<TEvent, TRes> : IDisposable
    {
        private static readonly int AWS_ACCOUNT_INDEX = 4;
        private static bool _coldStart = true;
        private ILambdaContext context;
        private IScope scope;
        private Stopwatch timer;

        public LambdaTriggerHandler(TEvent ev, ILambdaContext context)
        {
            this.context = context;
            this.scope = EpsagonUtils
                .GetService<ITracer>()
                .BuildSpan((typeof(TEvent).Name))
                .StartActive(finishSpanOnDispose: true);

            this.BuildSpan(context, scope.Span);
        }


        private void BuildSpan(ILambdaContext context, ISpan span)
        {
            var envRegion = Environment.GetEnvironmentVariable("AWS_REGION");

            span.SetTag("resource.type", "lambda");
            span.SetTag("aws.agent", "aws-sdk");
            span.SetTag("aws.agentVersion", ">1.11.0");
            span.SetTag("aws.service", "lambda");
            span.SetTag("aws.operation", "invoke");
            span.SetTag("aws.region", envRegion);
            span.SetTag("aws.lambda.error_code", 0); // OK
        }

        public void HandleBefore() {
            var coldStart = _coldStart;
            _coldStart = false;

            this.scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            this.scope.Span.SetTag("event.start_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);
            this.scope.Span.SetTag("resource.name", this.context.FunctionName);
            this.scope.Span.SetTag("resource.type", "lambda");
            this.scope.Span.SetTag("event.origin", "runner");
            this.scope.Span.SetTag("aws.account", this.context.InvokedFunctionArn.Split(':')[AWS_ACCOUNT_INDEX]);
            this.scope.Span.SetTag("aws.operation", "invoke");
            this.scope.Span.SetTag("aws.lambda.memory", this.context.MemoryLimitInMB.ToString());
            this.scope.Span.SetTag("aws.lambda.function_version", this.context.FunctionVersion);
            this.scope.Span.SetTag("aws.lambda.log_group_name", this.context.LogGroupName);
            this.scope.Span.SetTag("aws.lambda.log_stream_name", this.context.LogStreamName);
            this.scope.Span.SetTag("aws.lambda.cold_start", coldStart);

            this.timer = Stopwatch.StartNew();
        }

        public void HandleAfter(TRes returnValue) {
            var logger = EpsagonUtils.GetLogger<LambdaHandler<TEvent, TRes>>();
            this.timer.Stop();

            logger.LogDebug("Duration {duration}", this.timer.Elapsed.TotalMilliseconds);
            this.scope.Span.SetTag("event.duration", this.timer.Elapsed.TotalSeconds);
            // this.scope.Span.SetTag("aws.lambda.return_value", JsonConvert.SerializeObject(returnValue));
            this.scope.Span.SetTag("aws.lambda.return_value", "");
        }

        public void Dispose() => this.scope.Dispose();
    }
}
