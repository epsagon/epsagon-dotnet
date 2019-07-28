using System;
using OpenTracing;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation
{
    public interface IOperationHandler
    {
        void HandleOperationBefore(IExecutionContext context, IScope scope);
        void HandleOperationAfter(IExecutionContext context, IScope scope);
    }
}
