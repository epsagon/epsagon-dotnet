using Amazon.Runtime;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Empty
{
    public class EmptyHandler : BaseServiceHandler
    {
        public override void HandleAfter(IExecutionContext executionContext, IScope scope)
        {
            scope.Span.SetTag("resource.name", executionContext.RequestContext.ServiceMetaData.ServiceId);
            scope.Span.SetTag("resource.type", executionContext.RequestContext.ServiceMetaData.ServiceId);
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope)
        {
        }
    }
}
