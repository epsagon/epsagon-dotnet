using System;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Lambda
{
    public class LambdaTriggerHandler<TEvent, TRes> : IDisposable
    {
        private static readonly int AWS_ACCOUNT_INDEX = 4;
        private static bool _coldStart = true;
        private readonly ILambdaContext context;
        private readonly IScope scope;

        public LambdaTriggerHandler(TEvent ev, ILambdaContext context, IScope scope)
        {
            this.context = context;
            this.scope = scope;
        }

        public void HandleBefore()
        {
            Utils.DebugLogIfEnabled("lambda invoke event - START");
            var coldStart = _coldStart;
            _coldStart = false;

            var awsRequestId = context.AwsRequestId != "1234567890" ? context.AwsRequestId : $"local-{Guid.NewGuid()}";
            var awsAccount = this.context.InvokedFunctionArn.Split(':')[AWS_ACCOUNT_INDEX];
            var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");

            this.scope.Span.SetTag("event.id", awsRequestId);
            this.scope.Span.SetTag("event.origin", "runner");
            this.scope.Span.SetTag("event.error_code", 0); // OK
            this.scope.Span.SetTag("resource.type", "lambda");
            this.scope.Span.SetTag("resource.name", this.context.FunctionName);
            this.scope.Span.SetTag("aws.agent", "aws-sdk");
            this.scope.Span.SetTag("aws.service", "lambda");
            this.scope.Span.SetTag("resource.operation", "invoke");
            this.scope.Span.SetTag("aws.lambda.aws_account", awsAccount);
            this.scope.Span.SetTag("aws.lambda.region", awsRegion);
            this.scope.Span.SetTag("aws.lambda.memory", this.context.MemoryLimitInMB.ToString());
            this.scope.Span.SetTag("aws.lambda.function_version", this.context.FunctionVersion);
            this.scope.Span.SetTag("aws.lambda.log_group_name", this.context.LogGroupName);
            this.scope.Span.SetTag("aws.lambda.log_stream_name", this.context.LogStreamName);
            this.scope.Span.SetTag("aws.lambda.cold_start", coldStart);
            EpsagonUtils.SetLambdaTraceUrl(awsAccount, awsRegion, this.context.FunctionName, awsRequestId);
        }

        public void HandleAfter(TRes returnValue)
        {
            this.scope.Span.SetTag("aws.lambda.return_value", JsonConvert.SerializeObject(returnValue, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));

            EpsagonUtils.ClearTraceUrl();
            Utils.DebugLogIfEnabled("lambda invoke event - FINISHED");
        }

        public void Dispose() => this.scope.Dispose();
    }
}
