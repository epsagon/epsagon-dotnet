using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public abstract class EpsagonPipelineHandler : PipelineHandler
    {
        protected abstract string ServiceId { get; }
        protected abstract void BeforeExecution(IExecutionContext executionContext);
        protected abstract void AfterExecution(IExecutionContext executionContext);
        protected abstract void AfterExecution<T>(IExecutionContext executionContext, T res);

        protected virtual bool ShouldHandle(IExecutionContext executionContext)
        {
            var serviceId = executionContext.RequestContext.ServiceMetaData.ServiceId;
            return serviceId == ServiceId;
        }

        public override void InvokeSync(IExecutionContext executionContext)
        {
            var shouldHandle = ShouldHandle(executionContext);

            if (shouldHandle) BeforeExecution(executionContext);
            base.InvokeSync(executionContext);
            if (shouldHandle) AfterExecution(executionContext);
        }

        public override async Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            var shouldHandle = ShouldHandle(executionContext);

            if (shouldHandle) BeforeExecution(executionContext);
            var result = await base.InvokeAsync<T>(executionContext);
            if (shouldHandle) AfterExecution(executionContext, result);

            return result;
        }
    }
}