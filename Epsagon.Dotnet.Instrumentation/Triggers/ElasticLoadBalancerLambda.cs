using System;
using System.Collections.Generic;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class ElasticLoadBalancerLambda : ITrigger<ApplicationLoadBalancerRequest>
    {
        public void Handle(ApplicationLoadBalancerRequest input, ILambdaContext context, IScope scope)
        {
            var config = EpsagonUtils.GetConfiguration(GetType());

            scope.Span.SetTag("event.id", $"elb-{Guid.NewGuid().ToString()}");
            scope.Span.SetTag("resource.name", input.Path);
            scope.Span.SetTag("resource.operation", input.HttpMethod);

            var metadata = new Dictionary<string, string>() {
                { "query_string_parameters", JsonConvert.SerializeObject(input.QueryStringParameters) },
                { "target_group_arn", input.RequestContext.Elb.TargetGroupArn }
            };

            if (!config.MetadataOnly)
            {
                metadata.Add("body", input.Body);
                metadata.Add("headers", JsonConvert.SerializeObject(input.Headers));
            }

            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(metadata));
        }
    }
}
