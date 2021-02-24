using System;
using System.Linq;

using Amazon.Kinesis.Model;
using Amazon.Runtime;

using Epsagon.Dotnet.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.Kinesis.Operations {
    public class PutRecordsRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
            var response = context.ResponseContext.Response as PutRecordsResponse;

            scope.Span.SetTag("aws.kinesis.shard_id", response.Records.First().ShardId);
            scope.Span.SetTag("aws.kinesis.sequence_number", response.Records.First().SequenceNumber);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as PutRecordsRequest;
            var data = request.Records.Select(record => new {
                record.PartitionKey,
                Data = BitConverter.ToString(record.Data.ToArray()).Replace("-", "")
            });

            scope.Span.SetTag("resource.name", request.StreamName);
            scope.Span.SetTag("aws.kinesis.stream_name", request.StreamName);
            scope.Span.SetTag("aws.kinesis.partition_keys", Utils.SerializeObject(request.Records.Select(record => record.PartitionKey).ToArray()));
            scope.Span.SetDataIfNeeded("aws.kinesis.data", data);
        }
    }
}
