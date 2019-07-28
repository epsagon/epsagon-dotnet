using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB
{
    public class DynamoDBOperationsFactory : BaseFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "DeleteItemRequest", () => new DeleteItemRequestHandler() },
            { "GetItemRequest", () => new GetItemRequestHandler() },
            { "ListTablesRequest", () => new ListTablesRequestHandler() },
            { "PutItemRequest", () => new PutItemRequestHandler() },
            { "UpdateItemRequest", () => new UpdateItemRequestHandler() },
        };
    }
}
