using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.Empty;
using Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3
{
    public class S3OperationsFactory
        : IFactory<string, IOperationHandler>
    {
        public IOperationHandler GetInstace(string key)
        {
            var operations = new Dictionary<string, Func<IOperationHandler>>()
            {
                { "PutObjectRequest", () => new PutObjectRequestHandler() },
                { "GetObjectRequest", () => new GetObjectRequestHandler() },
                { "ListObjectsRequest", () => new ListObjectsRequestHandler() }
            };

            if (operations.ContainsKey(key)) return operations[key]();
            return new EmptyOperation();
        }
    }
}
