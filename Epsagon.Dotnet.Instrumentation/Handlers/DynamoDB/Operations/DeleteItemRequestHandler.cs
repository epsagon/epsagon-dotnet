using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations
{
    public class DeleteItemRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as DeleteItemRequest;
            scope.Span.SetTag("resource.name", request.TableName);
            scope.Span.SetTag("dynamodb.key", JsonConvert.SerializeObject(request.Key));
        }
    }
}
