using Amazon.Runtime;
using Amazon.SimpleEmail.Model;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SES.Operations
{
    public class SendEmailRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as SendEmailResponse;
            scope.Span.SetTag("aws.ses.message_id", response.MessageId);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as SendEmailRequest;
            scope.Span.SetTag("aws.ses.source", request.Source);
            scope.Span.SetTag("aws.ses.body", JsonConvert.SerializeObject(request.Message.Body, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            scope.Span.SetTag("aws.ses.destination", JsonConvert.SerializeObject(request.Destination, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            scope.Span.SetTag("aws.ses.subject", request.Message.Subject.Data);
        }
    }
}
