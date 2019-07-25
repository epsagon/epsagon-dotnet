using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3
{
    public class S3OperationsFactory
        : IFactory<string, IOperationHandler<AmazonWebServiceRequest, AmazonWebServiceResponse>>
    {
        public IOperationHandler<AmazonWebServiceRequest, AmazonWebServiceResponse> GetInstace(string key)
        {
            var operations = new Dictionary<string, Func<IOperationHandler<AmazonWebServiceRequest, AmazonWebServiceResponse>>>()
            {
                { "PutObjectRequest", () => new PutObjectRequestHandler() }
            };
            if (operations.ContainsKey(key)) return operations[key]();
                return new EmptyOperation();
        }
    }
}