using System;

using Amazon.Kinesis.Model;
using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Kinesis.Operations {
    public class PutRecordRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
            var response = context.ResponseContext.Response as PutRecordResponse;
            scope.Span.SetTag("aws.kinesis.shard_id", response.ShardId);
            scope.Span.SetTag("aws.kinesis.sequence_number", response.SequenceNumber);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as PutRecordRequest;
            scope.Span.SetTag("resource.name", request.StreamName);
            scope.Span.SetTag("aws.kinesis.partition_key", request.PartitionKey);
            scope.Span.SetTag("aws.kinesis.data", BitConverter.ToString(request.Data.ToArray()).Replace("-", ""));
        }
    }
}
