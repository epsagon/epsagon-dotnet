using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Epsagon.Dotnet.Core;
using Newtonsoft.Json;
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
            scope.Span.SetTag("resource.type", "dynamodb");
            scope.Span.SetTag("aws.operation", input.Records.First().EventName);
            scope.Span.SetTag("aws.dynamodb.region", input.Records.First().AwsRegion);
            scope.Span.SetTag("aws.dynamodb.sequence_number", input.Records.First().Dynamodb.SequenceNumber);
            scope.Span.SetTag("aws.dynamodb.item_hash", "test");
        }
    }
}
