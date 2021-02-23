using System;

using Amazon.Runtime;
using Amazon.S3.Model;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers.S3.Operations {
    public class GetObjectRequestHandler : IOperationHandler {
        public void HandleOperationAfter(IExecutionContext context, IScope scope) {
            var response = context.ResponseContext.Response as GetObjectResponse;
            scope.Span.SetTag("aws.s3.etag", response.ETag);
            scope.Span.SetTag("aws.s3.file_size", response.ContentLength);
            scope.Span.SetTag("aws.s3.last_modified", response.LastModified.ToString());
        }

        public void HandleOperationBefore(IExecutionContext context, IScope scope) {
            var request = context.RequestContext.OriginalRequest as GetObjectRequest;
            scope.Span.SetTag("resource.name", request.BucketName);
            scope.Span.SetTag("aws.s3.bucket", request.BucketName);
            scope.Span.SetTag("aws.s3.key", request.Key);
        }
    }
}
