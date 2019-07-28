using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers;
using Epsagon.Dotnet.Instrumentation.Handlers.DynamoDB;
using Epsagon.Dotnet.Instrumentation.Handlers.Kinesis;
using Epsagon.Dotnet.Instrumentation.Handlers.S3;
using Epsagon.Dotnet.Instrumentation.Handlers.SNS;

namespace Epsagon.Dotnet.Instrumentation
{
    public class HandlerFactory : IFactory<Type, IServiceHandler>
    {
        public static Dictionary<Type, Func<IServiceHandler>> constructors = new Dictionary<Type, Func<IServiceHandler>>
        {
            { typeof(Amazon.S3.AmazonS3Client), () => new SimpleServiceHandler<S3OperationsFactory>() },
            { typeof(Amazon.Kinesis.AmazonKinesisClient), () => new SimpleServiceHandler<KinesisOperationsFactory>() },
            { typeof(Amazon.DynamoDBv2.AmazonDynamoDBClient), () => new SimpleServiceHandler<DynamoDBOperationsFactory>() },
            { typeof(Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient), () => new SimpleServiceHandler<SNSOperationsFactory>() }
        };

        public IServiceHandler GetInstace(Type key)
        {
            return constructors[key]();
        }
    }
}
