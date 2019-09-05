using System.Linq;
using Amazon.Runtime;
using Amazon.S3.Model;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class ListObjectsRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext as ListObjectsResponse;
            var summeries = JsonConvert.SerializeObject(response.S3Objects.Select(o => o.Key).ToArray());
            scope.Span.SetTag("aws.s3.keys", summeries);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as ListObjectsRequest;
            scope.Span.SetTag("resource.name", request.BucketName);
            scope.Span.SetTag("aws.s3.bucket", request.BucketName);
        }
    }
}
