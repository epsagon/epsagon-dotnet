using System.Text;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Epsagon.Dotnet.Core.Configuration;
using Epsagon.Dotnet.Lambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DemoLambdaFunction
{
    [EpsagonConfiguration(AppName = "Dotnet Test")]
    public class Function : LambdaHandler<string, string>
    {
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            var client = new AmazonLambdaClient(RegionEndpoint.USEast1);
            var request = new InvokeRequest
            {
                FunctionName = "dotnet-private-test",
                InvocationType = InvocationType.RequestResponse,
                Payload = "\"test invoke\""
            };

            var response = Encoding.UTF8.GetString(client.InvokeAsync(request).Result.Payload.ToArray());

            var s3 = new AmazonS3Client(RegionEndpoint.USEast1);
            var putRequest = new PutObjectRequest
            {
                BucketName = "tal-dotnet-test-bucket",
                Key = "test-object",
                ContentBody = response
            };

            var putresult = s3.PutObjectAsync(putRequest).Result;
            return "success";
        }
    }
}
