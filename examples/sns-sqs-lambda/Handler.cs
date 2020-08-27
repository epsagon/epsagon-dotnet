using System;
using Epsagon.Dotnet.Lambda;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsDotnetCsharp
{
    public class SNSSender : LambdaHandler<Request, Response>
    {
        public override Response HandlerFunction(Request request, ILambdaContext context)
        {
            string topicArn = System.Environment.GetEnvironmentVariable("CF_MyTopic");
            string message = "Hello at " + DateTime.Now.ToShortTimeString();

            var client = new AmazonSimpleNotificationServiceClient(region: Amazon.RegionEndpoint.USEast1);

            var snsRequest = new PublishRequest
            {
                Message = message,
                TopicArn = topicArn
            };

            try
            {
                var response = client.Publish(snsRequest);

                Console.WriteLine("Message sent to topic:");
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception publishing request:");
                Console.WriteLine(ex.Message);
            }

            return new Response("SNS Message sent", request);
        }
    }

    public class SNSReceiver : LambdaHandler<SNSEvent, string>
    {
        public override string HandlerFunction(SNSEvent input, ILambdaContext context)
        {
            return "Go Serverless v1.0! Your function executed successfully!";
        }
    }

    public class SQSReceiver : LambdaHandler<SNSEvent, string>
    {
        public override string HandlerFunction(SNSEvent input, ILambdaContext context)
        {
            return "Go Serverless v1.0! Your function executed successfully!";
        }
    }

    public class Response
    {
        public string Message { get; set; }
        public Request Request { get; set; }

        public Response(string message, Request request)
        {
            Message = message;
            Request = request;
        }
    }

    public class Request
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }

        public Request(string key1, string key2, string key3)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }
    }
}
