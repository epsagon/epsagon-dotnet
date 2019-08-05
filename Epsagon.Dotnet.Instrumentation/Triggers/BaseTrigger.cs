using Amazon.Lambda.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public abstract class BaseTrigger<T> : ITrigger
    {
        protected T input;

        protected BaseTrigger(T input)
        {
            this.input = input;
        }

        public abstract void Handle(ILambdaContext context, IScope scope);
    }
}
