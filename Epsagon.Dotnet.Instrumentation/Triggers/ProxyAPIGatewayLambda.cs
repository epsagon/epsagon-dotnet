using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class ProxyAPIGatewayLambda : ITrigger<APIGatewayProxyRequest>
    {
        public void Handle(APIGatewayProxyRequest input, ILambdaContext context, IScope scope)
        {
            scope.Span.SetTag("event.id", input.RequestContext.RequestId);
            scope.Span.SetTag("resource.name", input.Headers.ContainsKey("Host") ? input.Headers["Host"] : input.RequestContext.ApiId);
            scope.Span.SetTag("resource.operation", input.HttpMethod);
            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(new
            {
                Stage = input.RequestContext.Stage,
                QueryStringParameters = input.QueryStringParameters,
                Resource = input.Resource,
                Path = input.Path,
                PathParameters = input.PathParameters
            }));

            EpsagonUtils.SetDataIfNeeded(scope, "resource.metadata.body", input.Body);
            EpsagonUtils.SetDataIfNeeded(scope, "resource.metadata.headers", input.Headers);
        }
    }
}
