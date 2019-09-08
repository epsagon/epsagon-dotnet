using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations
{
    public class UpdateItemRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as UpdateItemRequest;
            var updateParams = new Dictionary<string, string>();

            updateParams.Add("Key", JsonConvert.SerializeObject(request.Key, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            updateParams.Add("Expression Attribute Names", JsonConvert.SerializeObject(request.ExpressionAttributeNames, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            updateParams.Add("Expression Attribute Values", JsonConvert.SerializeObject(request.ExpressionAttributeValues, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            updateParams.Add("Update Expression", request.UpdateExpression);
            updateParams.Add("Condition Expression", request.ConditionExpression);

            scope.Span.SetTag("resource.name", request.TableName);
            scope.Span.SetTag("aws.dynamodb.update_parameters", JsonConvert.SerializeObject(updateParams, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
    }
}
