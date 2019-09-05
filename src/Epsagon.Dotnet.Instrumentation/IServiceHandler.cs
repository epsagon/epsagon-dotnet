using OpenTracing;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation
{
    public interface IServiceHandler : IPipelineHandler
    {
        void HandleBefore(IExecutionContext executionContext, IScope scope);
        void HandleAfter(IExecutionContext executionContext, IScope scope);
    }
}
