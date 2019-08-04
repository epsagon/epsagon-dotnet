using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class S3Lambda : ITrigger<S3Event>
    {
        public void Handle(S3Event input, ILambdaContext context, IScope scope)
        {
            var requestId = input.Records.First().ResponseElements.XAmzRequestId;
            scope.Span.SetTag("event.id", $"s3-trigger-{requestId}");
            scope.Span.SetTag("resource.name", input.Records.First().S3.Bucket.Name);
            scope.Span.SetTag("resource.operations", input.Records.First().EventName);
            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(new {
                Region = input.Records.First().AwsRegion,
                RequestParameters = input.Records.First().RequestParameters,
                UserIdentity = input.Records.First().UserIdentity,
                ObjectKey = input.Records.First().S3.Object.Key,
                ObjectSize = input.Records.First().S3.Object.Size,
                ObjectEtag= input.Records.First().S3.Object.ETag,
                ObjectSequencer = input.Records.First().S3.Object.Sequencer,
                XAmzRequestId = input.Records.First().ResponseElements.XAmzRequestId
            }));
        }
    }
}
