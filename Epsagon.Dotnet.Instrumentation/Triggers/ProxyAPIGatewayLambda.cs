using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using Newtonsoft.Json;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class ProxyAPIGatewayLambda : BaseTrigger<APIGatewayProxyRequest>
    {
        public ProxyAPIGatewayLambda(APIGatewayProxyRequest input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            base.Handle(context, scope);
            scope.Span.SetTag("event.id", input.RequestContext.RequestId);
            scope.Span.SetTag("resource.type", "api_gateway");
            scope.Span.SetTag("resource.name", input.Headers.ContainsKey("Host") ? input.Headers["Host"] : input.RequestContext.ApiId);
            scope.Span.SetTag("aws.operation", input.HttpMethod);
            scope.Span.SetTag("aws.api_gateway.stage", input.RequestContext.Stage);
            scope.Span.SetTag("aws.api_gateway.query_string_parameters", JsonConvert.SerializeObject(input.QueryStringParameters));
            scope.Span.SetTag("aws.api_gateway.resource", input.Resource);
            scope.Span.SetTag("aws.api_gateway.path", input.Path);
            scope.Span.SetTag("aws.api_gateway.path_parameters", JsonConvert.SerializeObject(input.PathParameters));
            scope.Span.SetDataIfNeeded("aws.api_gateway.body", input.Body);
            scope.Span.SetDataIfNeeded("aws.api_gateway.headers", input.Headers);
        }
    }
}
