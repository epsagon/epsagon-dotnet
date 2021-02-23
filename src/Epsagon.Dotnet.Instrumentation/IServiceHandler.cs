using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation {
    public interface IServiceHandler : IPipelineHandler {
        void HandleBefore(IExecutionContext executionContext, IScope scope);
        void HandleAfter(IExecutionContext executionContext, IScope scope);
    }
}
