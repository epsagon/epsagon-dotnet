using System.Linq;

using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

using Epsagon.Dotnet.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers {
    public class SQSLambda : BaseTrigger<SQSEvent> {
        public SQSLambda(SQSEvent input) : base(input) {
        }

        public override void Handle(ILambdaContext context, IScope scope) {
            string queueName = "";
            base.Handle(context, scope);

            var first = input.Records.First();
            var queueArnSplit = first.EventSourceArn?.Split(':');
            if (queueArnSplit != null) {
                queueName = queueArnSplit[queueArnSplit.Length - 1];
            }

            scope.Span.SetTag("event.id", first.MessageId);
            scope.Span.SetTag("resource.type", "sqs");
            scope.Span.SetTag("resource.name", queueName);
            scope.Span.SetTag("resource.operation", "Receive Operation");
            scope.Span.SetTag("aws.sqs.MD5 Of Message Body", first.Md5OfBody);
            scope.Span.SetTag("aws.sqs.Sender ID", first.Attributes["SenderId"]);
            scope.Span.SetTag("aws.sqs.Approximate Receive Count", first.Attributes["ApproximateReceiveCount"]);
            scope.Span.SetTag("aws.sqs.Sent Timestamp", first.Attributes["SentTimestamp"]);
            scope.Span.SetTag("aws.sqs.Approximate First Receive Timestamp", first.Attributes["ApproximateFirstReceiveTimestamp"]);
            scope.Span.SetDataIfNeeded("aws.sqs.Message Body", first.Body);

            if (!Utils.CurrentConfig.MetadataOnly) {
                scope.Span.SetTag("aws.sqs.message_id", first.MessageId);
                if (first.Body.Contains("TopicArn")) {
                    scope.Span.SetTag("aws.sqs.sns_trigger", first.Body);
                }
            }
        }
    }
}
