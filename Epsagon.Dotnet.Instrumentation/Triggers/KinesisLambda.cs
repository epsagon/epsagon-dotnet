using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class KinesisLambda : BaseTrigger<KinesisEvent>
    {
        public KinesisLambda(KinesisEvent input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            base.Handle(context, scope);
            scope.Span.SetTag("event.id", input.Records.First().EventId);
            scope.Span.SetTag("resource.type", "kinesis");
            scope.Span.SetTag("resource.name", input.Records.First().EventSourceARN.Split('/').Last());
            scope.Span.SetTag("aws.operation", input.Records.First().EventName.Replace("aws:kinesis", ""));
            scope.Span.SetTag("aws.kinesis.region", input.Records.First().AwsRegion);
            scope.Span.SetTag("aws.kinesis.invoke_identity", input.Records.First().InvokeIdentityArn);
            scope.Span.SetTag("aws.kinesis.sequence_number", input.Records.First().Kinesis.SequenceNumber);
            scope.Span.SetTag("aws.kinesis.partition_key", input.Records.First().Kinesis.PartitionKey);
        }
    }
}
