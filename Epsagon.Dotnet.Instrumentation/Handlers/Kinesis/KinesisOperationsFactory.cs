using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.Empty;
using Epsagon.Dotnet.Instrumentation.Handlers.Kinesis.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Kinesis
{
    public class KinesisOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "PutRecordRequest", () => new PutRecordRequestHandler() }
        };
    }
}
