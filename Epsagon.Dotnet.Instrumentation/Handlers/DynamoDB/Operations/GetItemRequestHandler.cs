using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations
{
    public class GetItemRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as GetItemResponse;
            scope.Span.SetTag("dynamodb.item", JsonConvert.SerializeObject(response.Item));
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as GetItemRequest;
            scope.Span.SetTag("resource.name", request.TableName);
            scope.Span.SetTag("dynamodb.key", JsonConvert.SerializeObject(request.Key));
        }
    }
}
