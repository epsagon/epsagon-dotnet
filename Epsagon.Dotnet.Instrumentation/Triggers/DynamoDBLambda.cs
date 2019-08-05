using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class DynamoDBLambda : BaseTrigger<DynamoDBEvent>
    {
        public DynamoDBLambda(DynamoDBEvent input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            base.Handle(context, scope);
            var eventSourceSplit = input.Records.First().EventSourceArn.Split('/');
            var resourceName = eventSourceSplit[eventSourceSplit.Length - 3];

            scope.Span.SetTag("event.id", input.Records.First().EventID);
            scope.Span.SetTag("resource.name", resourceName);
            scope.Span.SetTag("resource.operation", input.Records.First().EventName);
            scope.Span.SetTag("resource.metadata", Utils.SerializeObject(new {
                Region = input.Records.First().AwsRegion,
                SequenceNumber = input.Records.First().Dynamodb.SequenceNumber,
                ItemHash = "test" // same as epsagon-java
            }));
        }
    }
}
