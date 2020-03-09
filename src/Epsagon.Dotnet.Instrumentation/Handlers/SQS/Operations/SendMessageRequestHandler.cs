using Amazon.Runtime;
using Amazon.SQS.Model;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations
{
    public class SendMessageRequestHandler : IOperationHandler
    {
        IScope _scope;

        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as SendMessageResponse;

            _scope.Span.SetTag("aws.sqs.message_id", response.MessageId);
            _scope.Span.SetTag("aws.sqs.message_body_md5", response.MD5OfMessageBody);
            _scope.Dispose();
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            _scope = GlobalTracer.Instance.BuildSpan(context.RequestContext.RequestName).StartActive(finishSpanOnDispose: true);

            var request = context.RequestContext.OriginalRequest as SendMessageRequest;
            var queueName = "Invalid Queue URL";

            try { queueName = request.QueueUrl.Split('/')[4]; }
            catch { }

            _scope.Span.SetTag("resource.name", queueName);
            _scope.Span.SetTag("aws.sqs.message_body", request.MessageBody);
        }
    }
}
