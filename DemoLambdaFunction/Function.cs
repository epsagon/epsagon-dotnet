using System;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Epsagon.Dotnet.Config;
using Epsagon.Dotnet.Lambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DemoLambdaFunction
{
    public class Function : LambdaHandler<string, string>
    {
        [Epsagon(AppName = "Dotnet Test")]
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            var client = new AmazonLambdaClient(RegionEndpoint.USEast1);
            var request = new InvokeRequest
            {
                FunctionName = "dotnet-private-test",
                InvocationType = InvocationType.RequestResponse,
                Payload = "\"test invoke\""
            };

            var response = client.InvokeAsync(request).Result;
            return response.Payload.ToString();
        }
    }
}
