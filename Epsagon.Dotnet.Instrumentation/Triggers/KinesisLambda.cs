using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class KinesisLambda : ITrigger<KinesisEvent>
    {
        public void Handle(KinesisEvent input, ILambdaContext context, IScope scope)
        {
            scope.Span.SetTag("event.id", input.Records.First().EventId);
            scope.Span.SetTag("resource.name", input.Records.First().EventSourceARN.Split('/').Last());
            scope.Span.SetTag("resource.operation", input.Records.First().EventName.Replace("aws:kinesis", ""));
            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(new {
                Region = input.Records.First().AwsRegion,
                InvokeIdentity = input.Records.First().InvokeIdentityArn,
                SequenceNumber = input.Records.First().Kinesis.SequenceNumber,
                PartitionKey = input.Records.First().Kinesis.PartitionKey
            }));
        }
    }
}
