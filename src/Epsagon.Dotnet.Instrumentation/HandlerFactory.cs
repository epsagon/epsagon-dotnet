using System;
using System.Collections.Generic;

using Epsagon.Dotnet.Instrumentation.Handlers;
using Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB;
using Epsagon.Dotnet.Instrumentation.Handlers.Empty;
using Epsagon.Dotnet.Instrumentation.Handlers.Kinesis;
using Epsagon.Dotnet.Instrumentation.Handlers.Lambda;
using Epsagon.Dotnet.Instrumentation.Handlers.S3;
using Epsagon.Dotnet.Instrumentation.Handlers.SES;
using Epsagon.Dotnet.Instrumentation.Handlers.SNS;
using Epsagon.Dotnet.Instrumentation.Handlers.SQS;

namespace Epsagon.Dotnet.Instrumentation {
    public class HandlerFactory : BaseFactory<Type, IServiceHandler> {
        public HandlerFactory(string serviceName) : base(new EmptyHandler(serviceName = "empty")) { }

        protected override Dictionary<Type, Func<IServiceHandler>> Operations => new Dictionary<Type, Func<IServiceHandler>>
        {
            { typeof(Amazon.S3.AmazonS3Client), () => new SimpleServiceHandler<S3OperationsFactory>() },
            { typeof(Amazon.Kinesis.AmazonKinesisClient), () => new SimpleServiceHandler<KinesisOperationsFactory>() },
            { typeof(Amazon.DynamoDBv2.AmazonDynamoDBClient), () => new SimpleServiceHandler<DynamoDBOperationsFactory>() },
            { typeof(Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient), () => new SimpleServiceHandler<SNSOperationsFactory>() },
            { typeof(Amazon.SimpleEmail.AmazonSimpleEmailServiceClient), () => new SESServiceHandler() },
            { typeof(Amazon.SQS.AmazonSQSClient), () => new SQSServiceHandler() },
            { typeof(Amazon.Lambda.AmazonLambdaClient), () => new SimpleServiceHandler<LambdaOperationsFactory>() }
        };
    }
}
