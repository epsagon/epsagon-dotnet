using System;
using System.Collections.Generic;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class ElasticLoadBalancerLambda : BaseTrigger<ApplicationLoadBalancerRequest>
    {
        public ElasticLoadBalancerLambda(ApplicationLoadBalancerRequest input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            base.Handle(context, scope);
            var config = Utils.CurrentConfig;

            scope.Span.SetTag("event.id", $"elb-{Guid.NewGuid().ToString()}");
            scope.Span.SetTag("resource.type", "elastic_load_balancer");
            scope.Span.SetTag("resource.name", input.Path);
            scope.Span.SetTag("resource.operation", input.HttpMethod);
            scope.Span.SetTag("aws.elb.query_string_parameters", JsonConvert.SerializeObject(input.QueryStringParameters, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            scope.Span.SetTag("aws.elb.target_group_arn", input.RequestContext.Elb.TargetGroupArn);

            if (!config.MetadataOnly)
            {
                scope.Span.SetTag("aws.elb.body", input.Body);
                scope.Span.SetTag("aws.elb.headers", JsonConvert.SerializeObject(input.Headers, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            }
        }
    }
}
