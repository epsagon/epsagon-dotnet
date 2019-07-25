using System;
using Amazon.Runtime;
using Amazon.S3.Model;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations
{
    public class PutObjectRequestHandler : IOperationHandler<PutObjectRequest, PutObjectResponse>
    {
        public void HandleOperationAfter(PutObjectResponse response, IScope scope)
        {
            scope.Span.SetTag("s3.etag", response.ETag);
        }

        public void HandleOperationBefore(PutObjectRequest request, IScope scope)
        {
            scope.Span.SetTag("resource.name", request.BucketName);
            scope.Span.SetTag("s3.bucket", request.BucketName);
            scope.Span.SetTag("s3.key", request.Key);
        }
    }
}
