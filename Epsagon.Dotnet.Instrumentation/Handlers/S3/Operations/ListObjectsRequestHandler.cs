using System;
using System.Linq;
using Amazon.Runtime;
using Amazon.S3.Model;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class ListObjectsRequestHandler : IOperationHandler<ListObjectsRequest, ListObjectsResponse>
    {
        public void HandleOperationAfter(ListObjectsResponse response, IScope scope)
        {
            var summeries = JsonConvert.SerializeObject(response.S3Objects.Select(o => o.Key).ToArray());
            scope.Span.SetTag("s3.keys", summeries);
        }

        public void HandleOperationBefore(ListObjectsRequest request, IScope scope)
        {
            scope.Span.SetTag("resource.name", request.BucketName);
            scope.Span.SetTag("s3.bucket", request.BucketName);
        }
    }
}
