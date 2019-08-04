using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class DynamoDBLambda : ITrigger<DynamoDBEvent>
    {
        public void Handle(DynamoDBEvent input, ILambdaContext context, IScope scope)
        {
            var eventSourceSplit = input.Records.First().EventSourceArn.Split('/');
            var resourceName = eventSourceSplit[eventSourceSplit.Length - 3];

            scope.Span.SetTag("event.id", input.Records.First().EventID);
            scope.Span.SetTag("resource.name", resourceName);
            scope.Span.SetTag("resource.operation", input.Records.First().EventName);
            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(new {
                Region = input.Records.First().AwsRegion,
                SequenceNumber = input.Records.First().Dynamodb.SequenceNumber,
                ItemHash = "test" // same as epsagon-java
            }));
        }
    }
}
