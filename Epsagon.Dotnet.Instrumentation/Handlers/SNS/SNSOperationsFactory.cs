using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.SNS.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SNS
{
    public class SNSOperationsFactory : BaseFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "CreateTopicRequest", () => new CreateTopicRequestHandler() },
            { "PublishRequest", () => new PublishRequestHandler() }
        };
    }
}
