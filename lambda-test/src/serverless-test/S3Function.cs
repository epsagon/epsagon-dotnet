using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Epsagon.Dotnet.Lambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace serverless_test
{
    public class S3Function : LambdaHandler<S3Event, string>
    {
        public override string HandlerFunction(S3Event input, ILambdaContext context)
        {
            return input.Records.First().S3.Bucket.Name;
        }
    }
}
