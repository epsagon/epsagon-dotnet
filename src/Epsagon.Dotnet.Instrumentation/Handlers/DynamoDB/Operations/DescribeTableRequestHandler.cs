using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations {
    public class DescribeTableRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as DescribeTableRequest;
            scope.Span.SetTag("resource.name", request.TableName);
        }
    }
}
