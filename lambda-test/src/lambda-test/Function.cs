using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Epsagon.Dotnet.Lambda;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace lambda_test
{
    public class Function : LambdaHandler<string, string>
    {
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            // var request = new PutObjectRequest
            // {
            //     BucketName = "tal-dotnet-test-bucket",
            //     Key = "test",
            //     ContentBody = input
            // };

            // var client = new AmazonS3Client(RegionEndpoint.USEast1);

            // System.Console.WriteLine("Putting object");
            // client.PutObjectAsync(request).Wait();

            return input.ToUpper();
        }
    }
}
