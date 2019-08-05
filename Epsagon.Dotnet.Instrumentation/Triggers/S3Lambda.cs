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
            scope.Span.SetTag("aws.operation", input.Records.First().EventName);
            scope.Span.SetTag("resource.metadata", JsonConvert.SerializeObject(new
            {
                Region = input.Records.First().AwsRegion,
                RequestParameters = input.Records.First().RequestParameters.ToString(),
                UserIdentity = input.Records.First().UserIdentity.ToString(),
                ObjectKey = input.Records.First().S3.Object.Key,
                ObjectSize = input.Records.First().S3.Object.Size,
                ObjectEtag = input.Records.First().S3.Object.ETag,
                ObjectSequencer = input.Records.First().S3.Object.Sequencer,
                XAmzRequestId = input.Records.First().ResponseElements.XAmzRequestId
            }));
        }
    }
}
