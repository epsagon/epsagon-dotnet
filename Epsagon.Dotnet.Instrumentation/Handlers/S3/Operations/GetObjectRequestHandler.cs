using System;
using Amazon.Runtime;
using Amazon.S3.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class GetObjectRequestHandler : IOperationHandler<GetObjectRequest, GetObjectResponse>
    {
        public void HandleOperationAfter(GetObjectResponse response, IScope scope)
        {
        }

        public void HandleOperationBefore(GetObjectRequest request, IScope scope)
        {
        }
    }
}
