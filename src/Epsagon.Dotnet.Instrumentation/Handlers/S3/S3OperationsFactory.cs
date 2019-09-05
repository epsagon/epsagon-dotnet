using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3
{
    public class S3OperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "PutObjectRequest", () => new PutObjectRequestHandler() },
            { "GetObjectRequest", () => new GetObjectRequestHandler() },
            { "ListObjectsRequest", () => new ListObjectsRequestHandler() },
            { "HeadObjectRequest", () => new HeadObjectRequestHandler() }
        };
    }
}
