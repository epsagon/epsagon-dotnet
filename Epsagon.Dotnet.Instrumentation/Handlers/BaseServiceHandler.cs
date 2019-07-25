using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Epsagon.Dotnet.Core;
using OpenTracing;
using OpenTracing.Tag;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public abstract class BaseServiceHandler : PipelineHandler, IServiceHandler
    {
        public abstract void HandleAfter(IExecutionContext executionContext, IScope scope);
        public abstract void HandleBefore(IExecutionContext executionContext, IScope scope);

        protected ITracer tracer = EpsagonUtils.GetService<ITracer>();
        public override void InvokeSync(IExecutionContext executionContext)
        {
            var name = executionContext.RequestContext.RequestName;
            using (var scope = tracer.BuildSpan(name).StartActive(finishSpanOnDispose: true))
            {
                BuildSpan(executionContext, scope.Span);

                try { HandleBefore(executionContext, scope); }
                catch { Tags.Error.Set(scope.Span, true); }

                base.InvokeSync(executionContext);

                try { HandleAfter(executionContext, scope); }
                catch { Tags.Error.Set(scope.Span, true); }
            }
        }

        public override Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            var name = executionContext.RequestContext.RequestName;
            using (var scope = tracer.BuildSpan(name).StartActive(finishSpanOnDispose: true))
            {
                BuildSpan(executionContext, scope.Span);

                try { HandleBefore(executionContext, scope); }
                catch { Tags.Error.Set(scope.Span, true); }

                var result = base.InvokeAsync<T>(executionContext);

                try { HandleAfter(executionContext, scope); }
                catch { Tags.Error.Set(scope.Span, true); }

                return result;
            }
        }

        private void BuildSpan(IExecutionContext context, ISpan span)
        {
            var resoureType = context?.RequestContext?.ServiceMetaData.ServiceId;
            var serviceName = context?.RequestContext?.Request?.ServiceName;
            var operationName = context?.RequestContext?.RequestName;
            var endpoint = context?.RequestContext?.Request?.Endpoint?.ToString();
            var region = context?.RequestContext?.Request?.DeterminedSigningRegion;

            span.SetTag("resource.type", resoureType.ToLower());
            span.SetTag("aws.agent", "aws-sdk");
            span.SetTag("aws.agentVersion", ">1.11.0");
            span.SetTag("aws.service", serviceName);
            span.SetTag("aws.operation", operationName);
            span.SetTag("aws.endpoint", endpoint);
            span.SetTag("aws.region", region);
        }
    }
}
