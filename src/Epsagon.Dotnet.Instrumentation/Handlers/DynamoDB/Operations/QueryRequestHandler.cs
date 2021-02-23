using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

using Epsagon.Dotnet.Core;

using Newtonsoft.Json;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations {
    public class QueryRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
            var response = context.ResponseContext.Response as QueryResponse;
            var data = response.ToDictionary();

            if (Utils.CurrentConfig.MetadataOnly) {
                data.Remove("Items");
                data.Remove("LastEvaluatedKey");
            }

            scope.Span.SetTag("aws.dynamodb.Response", JsonConvert.SerializeObject(data, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as QueryRequest;
            var data = request.ToDictionary();

            if (Utils.CurrentConfig.MetadataOnly) {
                data.Remove("KeyConditions");
                data.Remove("QueryFilter");
                data.Remove("ExclusiveStartKey");
                data.Remove("ProjectionExpression");
                data.Remove("FilterExpression");
                data.Remove("KeyConditionExpression");
                data.Remove("ExpressionAttributeValues");
            }

            scope.Span.SetTag("resource.name", request.TableName);
            scope.Span.SetTag("aws.dynamodb.Parameters", JsonConvert.SerializeObject(data, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
    }
}
