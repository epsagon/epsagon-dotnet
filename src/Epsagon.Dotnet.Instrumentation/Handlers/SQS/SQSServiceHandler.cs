using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Epsagon.Dotnet.Core;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS
{
    public class SQSServiceHandler : PipelineHandler, IServiceHandler
    {
        IFactory<string, IOperationHandler> _operationsFactory = new SQSOperationsFactory();

        ITracer _tracer = GlobalTracer.Instance;

        public void HandleAfter(IExecutionContext executionContext, IScope scope) { }

        public void HandleBefore(IExecutionContext executionContext, IScope scope) { }

        public override Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            return Task.Run(() =>
            {
                var name = executionContext.RequestContext.RequestName;
                var handler = _operationsFactory.GetInstace(name);

                if (handler == null)
                {
                    Utils.DebugLogIfEnabled("AWSSDK request not supported ({name}), skipping", name);
                    return base.InvokeAsync<T>(executionContext).Result;
                }

                Utils.DebugLogIfEnabled("AWSSDK request invoked, {name}", name);

                try { handler.HandleOperationBefore(executionContext, null); }
                catch { }

                var result = base.InvokeAsync<T>(executionContext).Result;

                try { handler.HandleOperationAfter(executionContext, null); }
                catch { }

                return result;
            });
        }

        public override void InvokeSync(IExecutionContext executionContext)
        {
            var name = executionContext.RequestContext.RequestName;
            var handler = _operationsFactory.GetInstace(name);

            if (handler == null)
            {
                Utils.DebugLogIfEnabled("AWSSDK request not supported ({name}), skipping", name);
                base.InvokeSync(executionContext);
                return;
            }

            Utils.DebugLogIfEnabled("AWSSDK request invoked, {name}", name);

            try { handler.HandleOperationBefore(executionContext, null); }
            catch { }

            base.InvokeSync(executionContext);

            try { handler.HandleOperationAfter(executionContext, null); }
            catch { }
        }
    }
}
