using System;
using System.Security.Cryptography;
using System.Text;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations
{
    public class PutItemRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as PutItemRequest;
            var item = JsonConvert.SerializeObject(request.Item, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            scope.Span.SetTag("resource.name", request.TableName);
            scope.Span.SetTag("aws.dynamodb.item", item);
            scope.Span.SetTag("aws.dynamodb.item_hash", CalculateMD5(item));
        }

        private string CalculateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
