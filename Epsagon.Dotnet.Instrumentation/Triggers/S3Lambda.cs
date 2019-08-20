using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class S3Lambda : BaseTrigger<S3Event>
    {
        public S3Lambda(S3Event input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            base.Handle(context, scope);
            var requestId = input.Records.First().ResponseElements.XAmzRequestId;
            scope.Span.SetTag("event.id", $"s3-trigger-{requestId}");
            scope.Span.SetTag("resource.type", "s3");
            scope.Span.SetTag("resource.name", input.Records.First().S3.Bucket.Name);
            scope.Span.SetTag("resource.operation", input.Records.First().EventName);
            scope.Span.SetTag("aws.s3.region", input.Records.First().AwsRegion);
            scope.Span.SetTag("aws.s3.request_parameters", input.Records.First().RequestParameters.ToString());
            scope.Span.SetTag("aws.s3.user_identity", input.Records.First().UserIdentity.ToString());
            scope.Span.SetTag("aws.s3.object_key", input.Records.First().S3.Object.Key);
            scope.Span.SetTag("aws.s3.object_size", input.Records.First().S3.Object.Size);
            scope.Span.SetTag("aws.s3.object_etag", input.Records.First().S3.Object.ETag);
            scope.Span.SetTag("aws.s3.object_sequencer", input.Records.First().S3.Object.Sequencer);
            scope.Span.SetTag("aws.s3.x_amz_request_id", input.Records.First().ResponseElements.XAmzRequestId);
        }
    }
}
