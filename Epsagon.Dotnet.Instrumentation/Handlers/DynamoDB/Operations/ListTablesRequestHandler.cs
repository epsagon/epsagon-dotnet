using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations
{
    public class ListTablesRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as ListTablesResponse;
            scope.Span.SetTag("aws.dynamodb.tables", JsonConvert.SerializeObject(response.TableNames));
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            scope.Span.SetTag("resource.name", "DynamoDBEngine");
        }
    }
}
