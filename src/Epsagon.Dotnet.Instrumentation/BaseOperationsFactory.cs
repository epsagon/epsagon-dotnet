using Epsagon.Dotnet.Instrumentation.Handlers.Empty;

namespace Epsagon.Dotnet.Instrumentation {
    public abstract class BaseOperationsFactory : BaseFactory<string, IOperationHandler> {
        protected BaseOperationsFactory() : base(new EmptyOperation()) { }
    }
}
