using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SES {
    public class SESServiceHandler : BaseServiceHandler {
        private static SESOperationsFactory factory = new SESOperationsFactory();

        public override void HandleAfter(IExecutionContext executionContext, IScope scope) {
            factory
                .GetInstace(executionContext.RequestContext.RequestName)
                .HandleOperationAfter(executionContext, scope);
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope) {
            scope.Span.SetTag("resource.name", "SES Engine");
            factory
                .GetInstace(executionContext.RequestContext.RequestName)
                .HandleOperationBefore(executionContext, scope);
        }
    }
}
