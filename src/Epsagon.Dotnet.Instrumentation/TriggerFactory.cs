using System;
using System.Collections.Generic;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.KinesisEvents;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SNSEvents;
using Amazon.Lambda.SQSEvents;

using Epsagon.Dotnet.Instrumentation.Triggers;

namespace Epsagon.Dotnet.Instrumentation {
    public class TriggerFactory {
        public static ITrigger CreateInstance(Type eventType, object input) {
            var dict = new Dictionary<Type, Func<ITrigger>> {
                { typeof(S3Event), () => new S3Lambda(input as S3Event) },
                { typeof(DynamoDBEvent), () => new DynamoDBLambda(input as DynamoDBEvent) },
                { typeof(ApplicationLoadBalancerRequest), () => new ElasticLoadBalancerLambda(input as ApplicationLoadBalancerRequest) },
                { typeof(ScheduledEvent), () => new EventsLambda(input as ScheduledEvent) },
                { typeof(KinesisEvent), () => new KinesisLambda(input as KinesisEvent) },
                { typeof(APIGatewayProxyRequest), () => new ProxyAPIGatewayLambda(input as APIGatewayProxyRequest) },
                { typeof(SNSEvent), () => new SNSLambda(input as SNSEvent) },
                { typeof(SQSEvent), () => new SQSLambda(input as SQSEvent) },
                { typeof(CloudWatchEvent<object>), () => new CloudWatchEventsLambda(input as CloudWatchEvent<object>) },
            };

            if (dict.ContainsKey(eventType)) {
                return dict[eventType]();
            }

            return new JsonLambda(input);
        }
    }
}
