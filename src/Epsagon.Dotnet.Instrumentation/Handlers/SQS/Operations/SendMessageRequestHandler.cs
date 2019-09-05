using Amazon.Runtime;
using Amazon.SQS.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations
{
    public class SendMessageRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as SendMessageResponse;
            scope.Span.SetTag("aws.sqs.message_id", response.MessageId);
            scope.Span.SetTag("aws.sqs.message_body_md5", response.MD5OfMessageBody);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as SendMessageRequest;
            var queueName = "Invalid Queue URL";

            try { queueName = request.QueueUrl.Split('/')[4]; }
            catch { }

            scope.Span.SetTag("resource.name", queueName);
            scope.Span.SetTag("aws.sqs.message_body", request.MessageBody);
        }
    }
}
