using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
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
            scope.Span.SetTag("resource.name", input.Headers.ContainsKey("Host") ? input.Headers["Host"] : input.RequestContext.ApiId);
            scope.Span.SetTag("resource.operation", input.HttpMethod);
            scope.Span.SetTag("resource.metadata", Utils.SerializeObject(new
            {
                Stage = input.RequestContext.Stage,
                QueryStringParameters = input.QueryStringParameters,
                Resource = input.Resource,
                Path = input.Path,
                PathParameters = input.PathParameters
            }));

            scope.Span.SetDataIfNeeded("resource.metadata.body", input.Body);
            scope.Span.SetDataIfNeeded("resource.metadata.headers", input.Headers);
        }
    }
}
