using System;

using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers {
    public class SimpleServiceHandler<TOpFactory> : BaseServiceHandler where TOpFactory : IFactory<string, IOperationHandler> {
        readonly IFactory<string, IOperationHandler> factory = Activator.CreateInstance<TOpFactory>();

        public override void HandleAfter(IExecutionContext executionContext, IScope scope) {
            var operation = executionContext.RequestContext.RequestName;
            var instance = factory.GetInstace(operation);

            if (!(instance is null)) {
                instance.HandleOperationAfter(executionContext, scope);
            }
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope) {
            var operation = executionContext.RequestContext.RequestName;
            var instance = factory.GetInstace(operation);

            if (!(instance is null)) {
                instance.HandleOperationBefore(executionContext, scope);
            }
        }
    }
}
