using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation {
    public interface IOperationHandler {
        void HandleOperationBefore(IExecutionContext context, IScope scope);
        void HandleOperationAfter(IExecutionContext context, IScope scope);
    }
}
