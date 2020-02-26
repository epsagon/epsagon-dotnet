using Amazon.Runtime;
using OpenTracing;
using Epsagon.Dotnet.Core;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Empty
{
    public class EmptyHandler : BaseServiceHandler
    {
        private string _serviceName;

        public EmptyHandler(string serviceName)
        {
            this._serviceName = serviceName;
        }

        public override void HandleAfter(IExecutionContext executionContext, IScope scope)
        {
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope)
        {
            scope.Span.SetTag("resource.name", this._serviceName);
            scope.Span.SetTag("resource.type", this._serviceName);
        }
    }
}
