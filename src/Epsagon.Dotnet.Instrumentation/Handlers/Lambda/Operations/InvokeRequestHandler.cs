using Amazon.Lambda.Model;
using Amazon.Runtime;

using Epsagon.Dotnet.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Lambda.Operations {
    public class InvokeRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as InvokeRequest;
            var functionName = request.FunctionName;

            scope.Span.SetTag("resource.name", functionName);
            scope.Span.SetDataIfNeeded("aws.lambda.payload", request.Payload);
        }
    }
}
