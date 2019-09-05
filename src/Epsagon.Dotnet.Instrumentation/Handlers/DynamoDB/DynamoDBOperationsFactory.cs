using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB.Operations;

namespace Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB
{
    public class DynamoDBOperationsFactory : BaseOperationsFactory
    {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>()
        {
            { "PutItemRequest", () => new PutItemRequestHandler() },
            { "UpdateItemRequest", () => new UpdateItemRequestHandler() },
            { "GetItemRequest", () => new GetItemRequestHandler() },
            { "DeleteItemRequest", () => new DeleteItemRequestHandler() },
            { "DescribeTableRequest", () => new DescribeTableRequestHandler() },
            { "QueryRequest", () => new QueryRequestHandler() },
            { "ListTablesRequest", () => new ListTablesRequestHandler() },
        };
    }
}
