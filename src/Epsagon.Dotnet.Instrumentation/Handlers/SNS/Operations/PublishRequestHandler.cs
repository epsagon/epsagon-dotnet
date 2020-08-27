using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SNS.Operations
{
    public class PublishRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as PublishResponse;
            scope.Span.SetTag("aws.sns.Message ID", response.MessageId);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as PublishRequest;
            var topicName = "Invalid Topic ARN";

            try { topicName = request.TopicArn.Split(':')[5]; }
            catch { scope.Span.SetTag("aws.sns.topic.arn", request.TopicArn); }

            scope.Span.SetTag("resource.name", topicName);
            scope.Span.SetTag("aws.sns.message", request.Message);
        }
    }
}
