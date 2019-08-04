using Amazon.Lambda.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation
{
    public interface ITrigger<TEvent>
    {
         void Handle(TEvent input, ILambdaContext context, IScope scope);
    }
}
