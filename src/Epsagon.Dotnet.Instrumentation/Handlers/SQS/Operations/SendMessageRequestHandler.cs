using System;

using Amazon.Runtime;
using Amazon.SQS.Model;

using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SQS.Operations {
    public class SendMessageRequestHandler : IOperationHandler {
        IScope _scope;

        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
            var response = context.ResponseContext.Response as SendMessageResponse;

            _scope.Span.SetTag("aws.sqs.Message ID", response.MessageId);
            _scope.Span.SetTag("aws.sqs.message_id", response.MessageId);
            _scope.Span.SetTag("aws.sqs.message_body_md5", response.MD5OfMessageBody);
            _scope.Dispose();
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var resoureType = context?.RequestContext?.ServiceMetaData.ServiceId;
            var serviceName = context?.RequestContext?.ServiceMetaData.ServiceId;
            var operationName = context?.RequestContext?.RequestName;
            var endpoint = context?.RequestContext?.Request?.Endpoint?.ToString();
            var region = context?.RequestContext?.ClientConfig?.RegionEndpoint?.SystemName;
            var envRegion = Environment.GetEnvironmentVariable("AWS_REGION");

            _scope = GlobalTracer.Instance.BuildSpan(context.RequestContext.RequestName).StartActive(finishSpanOnDispose: true);
            _scope.Span.SetTag("resource.type", resoureType?.ToLower());
            _scope.Span.SetTag("event.origin", "aws-sdk");
            _scope.Span.SetTag("event.error_code", 0); // OK
            _scope.Span.SetTag("aws.service", serviceName);
            _scope.Span.SetTag("resource.operation", operationName);
            _scope.Span.SetTag("resource.name", serviceName);
            _scope.Span.SetTag("aws.endpoint", endpoint);
            _scope.Span.SetTag("aws.region", envRegion ?? region);

            var request = context.RequestContext.OriginalRequest as SendMessageRequest;
            var queueName = "Invalid Queue URL";

            try { queueName = request.QueueUrl.Split('/')[4]; } catch { }

            _scope.Span.SetTag("resource.name", queueName);
            _scope.Span.SetTag("aws.sqs.message_body", request.MessageBody);
        }
    }
}
