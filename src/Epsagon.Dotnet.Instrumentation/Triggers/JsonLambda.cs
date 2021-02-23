using System;

using Amazon.Lambda.Core;

using Epsagon.Dotnet.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers {
    /// <summary>
    /// Generic json invoke lambda trigger
    /// </summary>
    public class JsonLambda : BaseTrigger<object> {
        public JsonLambda(object input) : base(input) {
        }

        public override void Handle(ILambdaContext context, IScope scope) {
            base.Handle(context, scope);
            scope.Span.SetTag("resource.type", "json");
            scope.Span.SetTag("resource.name", $"trigger-{context.FunctionName}");
            scope.Span.SetTag("resource.operation", "json");
            scope.Span.SetTag("event.id", $"trigger-{Guid.NewGuid().ToString()}");
            scope.Span.SetDataIfNeeded("aws.json.data", input);
        }
    }
}
