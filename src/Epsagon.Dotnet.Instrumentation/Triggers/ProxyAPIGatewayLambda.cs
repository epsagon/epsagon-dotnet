using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using Epsagon.Dotnet.Core;

using Newtonsoft.Json;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers {
    public class ProxyAPIGatewayLambda : BaseTrigger<APIGatewayProxyRequest> {
        public ProxyAPIGatewayLambda(APIGatewayProxyRequest input) : base(input) {
        }

        public override void Handle(ILambdaContext context, IScope scope) {
            base.Handle(context, scope);
            scope.Span.SetTag("event.id", input?.RequestContext?.RequestId);
            scope.Span.SetTag("resource.type", "api_gateway");

            var hostKey = input?.Headers?.ContainsKey("Host");

            scope.Span.SetTag("resource.name", hostKey.HasValue && hostKey.Value ? input?.Headers["Host"] : input?.RequestContext?.ApiId);
            scope.Span.SetTag("resource.operation", input?.HttpMethod);
            scope.Span.SetTag("aws.api_gateway.stage", input?.RequestContext?.Stage);
            scope.Span.SetTag("aws.api_gateway.query_string_parameters", JsonConvert.SerializeObject(input?.QueryStringParameters, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            scope.Span.SetTag("aws.api_gateway.path", input?.Resource);
            scope.Span.SetTag("aws.api_gateway.path_parameters", JsonConvert.SerializeObject(input?.PathParameters, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            scope.Span.SetDataIfNeeded("aws.api_gateway.body", input?.Body);
            scope.Span.SetDataIfNeeded("aws.api_gateway.headers", input?.Headers);
        }
    }
}
