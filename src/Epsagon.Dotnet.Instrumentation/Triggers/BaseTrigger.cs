using System;

using Amazon.Lambda.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers {
    public abstract class BaseTrigger<T> : ITrigger {
        protected T input;

        protected BaseTrigger(T input) {
            this.input = input;
        }

        public virtual void Handle(ILambdaContext context, IScope scope) {
            scope.Span.SetTag("event.origin", "trigger");
            scope.Span.SetTag("event.error_code", 0);
            scope.Span.SetTag("aws.base.region", Environment.GetEnvironmentVariable("AWS_REGION"));
        }
    }
}
