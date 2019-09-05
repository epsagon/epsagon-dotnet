using Amazon.Runtime;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Empty
{
    public class EmptyOperation : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) { }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) { }
    }
}
