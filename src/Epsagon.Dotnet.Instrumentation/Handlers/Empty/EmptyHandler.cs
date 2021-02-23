using Amazon.Runtime;

using Epsagon.Dotnet.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Empty {
    public class EmptyHandler : BaseServiceHandler {
        private string _serviceName;

        public EmptyHandler(string serviceName) {
            this._serviceName = serviceName;
        }

        public override void HandleAfter(IExecutionContext executionContext, IScope scope) {
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope) {
            var serviceId = executionContext?.RequestContext?.ServiceMetaData?.ServiceId;
            scope.Span.SetTag("resource.name", string.IsNullOrEmpty(serviceId) ? this._serviceName : serviceId);
            scope.Span.SetTag("resource.type", string.IsNullOrEmpty(serviceId) ? this._serviceName : serviceId);
        }
    }
}
