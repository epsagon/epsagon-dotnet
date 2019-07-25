using System;
using Amazon.Runtime;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class EmptyOperation : IOperationHandler<AmazonWebServiceRequest, AmazonWebServiceResponse>
    {
        public void HandleOperationAfter(AmazonWebServiceResponse response, IScope scope)
        {
        }

        public void HandleOperationBefore(AmazonWebServiceRequest request, IScope scope)
        {
        }
    }
}
