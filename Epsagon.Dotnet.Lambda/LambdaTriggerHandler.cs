using System;
using System.Diagnostics;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
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
        }

        public void HandleBefore() {
            var coldStart = _coldStart;
            _coldStart = false;

            this.scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
            this.scope.Span.SetTag("event.start_time", DateTime.UtcNow.ToBinary());
            this.scope.Span.SetTag("resource.name", this.context.FunctionName);
            this.scope.Span.SetTag("resource.type", "lambda");
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
            this.timer.Stop();

            this.scope.Span.SetTag("event.duration", this.timer.ElapsedMilliseconds);
            this.scope.Span.SetTag("aws.lambda.return_value", JsonConvert.SerializeObject(returnValue));
        }

        public void Dispose() => this.scope.Dispose();
    }
}
