using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS
{
    public class SQSOperationsFactory : BaseFactory<string, IOperationHandler>
    {
        public SQSOperationsFactory() : base(null)
        {
        }

        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>> {
            { "SendMessageRequest", () => new SendMessageRequestHandler() },
            { "ReceiveMessageRequest", () => new ReceiveMessageRequestHandler() }
        };
    }
}
