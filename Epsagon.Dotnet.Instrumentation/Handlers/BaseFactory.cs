using System;
using System.Collections.Generic;
using Epsagon.Dotnet.Instrumentation.Handlers.Empty;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public abstract class BaseFactory : IFactory<string, IOperationHandler>
    {
        protected abstract Dictionary<string, Func<IOperationHandler>> Operations { get; }
        private Dictionary<string, Func<IOperationHandler>> _operations;

        protected BaseFactory()
        {
            // avoid creating the ops dictionary for each request
            this._operations = this.Operations;
        }

        public IOperationHandler GetInstace(string key)
        {
            if (this.Operations.ContainsKey(key)) return this.Operations[key]();
            return new EmptyOperation();
        }
    }
}
