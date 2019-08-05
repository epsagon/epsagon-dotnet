using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Epsagon.Dotnet.Lambda;

namespace serverless_test
{
    public class S3Function : LambdaHandler<S3Event, string>
    {
        private static readonly string TopicARN = "arn:aws:sns:us-east-1:066549572091:NewObjectsTopicTest";
        public override string HandlerFunction(S3Event input, ILambdaContext context)
        {
            var client = new AmazonSimpleNotificationServiceClient();
            var message = $"{input.Records.First().S3.Object.Key} was created";
            var request = new PublishRequest(TopicARN, message);

            var response = client.PublishAsync(request).Result;
            return input.Records.First().S3.Object.Key;
        }
    }
}
