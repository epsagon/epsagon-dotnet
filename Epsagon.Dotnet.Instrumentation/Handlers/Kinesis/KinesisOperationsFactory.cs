using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.Empty;
using Epsagon.Dotnet.Instrumentation.Handlers.Kinesis.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Kinesis
{
    public class KinesisOperationsFactory : IFactory<string, IOperationHandler>
    {
        public IOperationHandler GetInstace(string key)
        {
            var operations = new Dictionary<string, Func<IOperationHandler>>()
            {
                { "PutRecordRequest", () => new PutRecordRequestHandler() }
            };

            if (operations.ContainsKey(key)) return operations[key]();
            return new EmptyOperation();
        }
    }
}
