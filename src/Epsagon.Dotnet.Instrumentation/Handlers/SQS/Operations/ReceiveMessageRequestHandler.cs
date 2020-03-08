using System;
using Amazon.Runtime;
using Amazon.SQS.Model;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations
{
    public class ReceiveMessageRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as ReceiveMessageRequest;
            var response = context.ResponseContext.Response as ReceiveMessageResponse;
            var messages = response.Messages;

            scope.Span.SetTag("aws.sqs.messages_count", messages.Count);

            foreach (var message in messages)
            {
                using (var messageScope = GlobalTracer.Instance
                    .BuildSpan("ReceiveMessage")
                    .AsChildOf(scope.Span.Context)
                    .StartActive(finishSpanOnDispose: true))
                {
                    string queueName;
                    try { queueName = request.QueueUrl.Split('/')[4]; }
                    catch { queueName = "Invalid Queue Name"; }

                    messageScope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                    messageScope.Span.SetTag("resource.name", queueName);
                    messageScope.Span.SetTag("resource.type", "sqs");
                    messageScope.Span.SetTag("event.origin", "aws-sdk");
                    messageScope.Span.SetTag("aws.sqs.queue.url", request.QueueUrl);
                    messageScope.Span.SetTag("aws.sqs.message_body", message.Body);
                    messageScope.Span.SetTag("aws.sqs.message_id", message.MessageId);
                    messageScope.Span.SetTag("aws.sqs.message_body_md5", message.MD5OfBody);
                }
            }
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as ReceiveMessageRequest;

            string queueName;
            try { queueName = request.QueueUrl.Split('/')[4]; }
            catch { queueName = "Invalid Queue Name"; }

            scope.Span.SetTag("resource.name", queueName);
            scope.Span.SetTag("resource.type", "sqs");
        }
    }
}
