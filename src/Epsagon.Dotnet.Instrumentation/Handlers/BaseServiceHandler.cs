using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Epsagon.Dotnet.Core;
using OpenTracing;
using OpenTracing.Util;
using Serilog;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public abstract class BaseServiceHandler : PipelineHandler, IServiceHandler
    {
        public abstract void HandleAfter(IExecutionContext executionContext, IScope scope);
        public abstract void HandleBefore(IExecutionContext executionContext, IScope scope);

        protected ITracer tracer = GlobalTracer.Instance;

        public override void InvokeSync(IExecutionContext executionContext)
        {
            var name = executionContext.RequestContext.RequestName;
            Utils.DebugLogIfEnabled("AWSSDK request invoked, {name}", name);

            using (var scope = tracer.BuildSpan(name).StartActive(finishSpanOnDispose: true))
            {
                BuildSpan(executionContext, scope.Span);

                try { HandleBefore(executionContext, scope); }
                catch (Exception e) { scope.Span.AddException(e); }

                base.InvokeSync(executionContext);

                try
                {
                    HandleAfter(executionContext, scope);
                    scope.Span.SetTag("event.id", executionContext.ResponseContext.Response.ResponseMetadata.RequestId);
                }
                catch (Exception e)
                {
                    scope.Span.SetTag("event.id", executionContext.ResponseContext.Response.ResponseMetadata.RequestId);
                    scope.Span.AddException(e);
                }
            }
        }

        public override Task<T> InvokeAsync<T>(IExecutionContext executionContext)
        {
            var name = executionContext.RequestContext.RequestName;
            Utils.DebugLogIfEnabled("AWSSDK request invoked, {name}", name);

            using (var scope = tracer.BuildSpan(name).StartActive(finishSpanOnDispose: true))
            {
                BuildSpan(executionContext, scope.Span);

                try { HandleBefore(executionContext, scope); }
                catch (Exception e) { scope.Span.AddException(e); }

                var result = base.InvokeAsync<T>(executionContext).Result;

                try
                {
                    HandleAfter(executionContext, scope);
                    scope.Span.SetTag("event.id", executionContext.ResponseContext.Response.ResponseMetadata.RequestId);
                }
                catch (Exception e)
                {
                    scope.Span.SetTag("event.id", executionContext.ResponseContext.Response.ResponseMetadata.RequestId);
                    scope.Span.AddException(e);
                }

                return Task.FromResult(result);
            }
        }

        private void BuildSpan(IExecutionContext context, ISpan span)
        {
            var resoureType = context?.RequestContext?.ServiceMetaData.ServiceId;
            var serviceName = context?.RequestContext?.ServiceMetaData.ServiceId;
            var operationName = context?.RequestContext?.RequestName;

            var endpoint = context?.RequestContext?.Request?.Endpoint?.ToString();
            var region = context?.RequestContext?.ClientConfig?.RegionEndpoint?.SystemName;
            var envRegion = Environment.GetEnvironmentVariable("AWS_REGION");

            span.SetTag("resource.type", resoureType?.ToLower());
            span.SetTag("event.origin", "aws-sdk");
            span.SetTag("event.error_code", 0); // OK
            span.SetTag("aws.service", serviceName);
            span.SetTag("resource.operation", operationName);
            span.SetTag("aws.endpoint", endpoint);
            span.SetTag("aws.region", envRegion ?? region);
        }
    }
}
