using System;
using Amazon.Runtime;
using Amazon.S3.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class GetObjectRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
        }
    }
}
