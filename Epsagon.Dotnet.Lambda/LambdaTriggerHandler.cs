using System;
using System.Diagnostics;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Util;
using Serilog;

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
            this.scope = GlobalTracer.Instance
                .BuildSpan((typeof(TEvent).Name))
                .StartActive(finishSpanOnDispose: true);
        }

        public void HandleBefore()
        {
            Log.Debug("lambda invoke event - START");

            var coldStart = _coldStart;
            _coldStart = false;

            this.scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            this.scope.Span.SetTag("event.origin", "runner");
            this.scope.Span.SetTag("event.start_time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);
            this.scope.Span.SetTag("resource.type", "lambda");
            this.scope.Span.SetTag("resource.name", this.context.FunctionName);
            this.scope.Span.SetTag("aws.agent", "aws-sdk");
            this.scope.Span.SetTag("aws.agentVersion", ">1.11.0");
            this.scope.Span.SetTag("aws.service", "lambda");
            this.scope.Span.SetTag("aws.operation", "invoke");
            this.scope.Span.SetTag("aws.region", Environment.GetEnvironmentVariable("AWS_REGION"));
            this.scope.Span.SetTag("aws.lambda.error_code", 0); // OK
            this.scope.Span.SetTag("aws.account", this.context.InvokedFunctionArn.Split(':')[AWS_ACCOUNT_INDEX]);
            this.scope.Span.SetTag("aws.operation", "invoke");
            this.scope.Span.SetTag("aws.lambda.memory", this.context.MemoryLimitInMB.ToString());
            this.scope.Span.SetTag("aws.lambda.function_version", this.context.FunctionVersion);
            this.scope.Span.SetTag("aws.lambda.log_group_name", this.context.LogGroupName);
            this.scope.Span.SetTag("aws.lambda.log_stream_name", this.context.LogStreamName);
            this.scope.Span.SetTag("aws.lambda.cold_start", coldStart);

            this.timer = Stopwatch.StartNew();
        }

        public void HandleAfter(TRes returnValue)
        {
            this.timer.Stop();
            this.scope.Span.SetTag("event.duration", this.timer.Elapsed.TotalSeconds);
            this.scope.Span.SetTag("aws.lambda.return_value", JsonConvert.SerializeObject(returnValue));

            Log.Debug("lambda invoke event - FINISHED");
        }

        public void Dispose() => this.scope.Dispose();
    }
}
