using Amazon.Runtime;
using Amazon.SQS.Model;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations
{
    public class ReceiveMessageRequestHandler : IOperationHandler
    {
        public string QueueName { get; set; }

        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as ReceiveMessageResponse;
            var queueName = "";
            var messages = response.Messages;

            foreach (var message in messages)
            {
                using (var messageScope = GlobalTracer.Instance.BuildSpan("ReceiveMessage").StartActive(finishSpanOnDispose: true))
                {
                    messageScope.Span.SetTag("resource.name", queueName);
                    messageScope.Span.SetTag("resource.type", "sqs");
                    messageScope.Span.SetTag("aws.sqs.message_body", message.Body);
                    messageScope.Span.SetTag("aws.sqs.message_id", message.MessageId);
                    messageScope.Span.SetTag("aws.sqs.message_body_md5", message.MD5OfBody);
                }
            }
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as ReceiveMessageRequest;

            QueueName = "Invalid Queue Name";
            try { QueueName = request.QueueUrl.Split('/')[4]; }
            catch { scope.Span.SetTag("aws.sqs.queue.url", request.QueueUrl); }
        }
    }
}
