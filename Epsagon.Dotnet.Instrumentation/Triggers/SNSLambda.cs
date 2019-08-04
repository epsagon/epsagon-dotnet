using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class SNSLambda : ITrigger<SNSEvent>
    {
        public void Handle(SNSEvent input, ILambdaContext context, IScope scope)
        {
            var operationSplit = input.Records.First().EventSubscriptionArn.Split(':');
            var operation = operationSplit[operationSplit.Length - 2];
            var message = input.Records.First().Sns.Message;
            var config = EpsagonUtils.GetConfiguration(GetType());

            scope.Span.SetTag("event.id", input.Records.First().Sns.MessageId);
            scope.Span.SetTag("resource.operation", operation);
            scope.Span.SetTag("resource.metadata", "{ \"Notification Subject\": \"" + input.Records.First().Sns.Subject + "\" }");

            if (!config.MetadataOnly)
            {
                scope.Span.SetTag("resource.metadata", string.Format(@"{{
                    ""Notification Subject"": ""{0}"",
                    ""Notification Message"": ""{1}""
                }}", input.Records.First().Sns.Subject, input.Records.First().Sns.Message));
            }


        }
    }
}
