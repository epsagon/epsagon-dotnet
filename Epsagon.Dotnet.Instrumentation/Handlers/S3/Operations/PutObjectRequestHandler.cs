using System;
using Amazon.Runtime;
using Amazon.S3.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class PutObjectRequestHandler : IOperationHandler
    {
        public void HandleOperationAfter(IExecutionContext context, IScope scope)
        {
            var response = context.ResponseContext.Response as PutObjectResponse;
            scope.Span.SetTag("s3.etag", response.ETag);
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope)
        {
            var request = context.RequestContext.OriginalRequest as PutObjectRequest;
            scope.Span.SetTag("resource.name", request.BucketName);
            scope.Span.SetTag("s3.bucket", request.BucketName);
            scope.Span.SetTag("s3.key", request.Key);
        }
    }
}
