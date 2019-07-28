using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SNS.Operations
{
    public class CreateTopicRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as CreateTopicRequest;
            scope.Span.SetTag("resource.name", request.Name);
        }
    }
}
