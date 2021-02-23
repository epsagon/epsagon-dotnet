using Amazon.Lambda.Core;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation {
    public interface ITrigger {
        void Handle(ILambdaContext context, IScope scope);
    }
}
