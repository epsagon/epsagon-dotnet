using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.Lambda.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Lambda
{
    public class LambdaOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>
        {
            { "InvokeRequest", () => new InvokeRequestHandler() }
        };
    }
}
