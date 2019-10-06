using System;
using System.Collections.Generic;
using Serilog;

namespace Epsagon.Dotnet.Instrumentation
{
    public abstract class BaseFactory<TKey, TVal> : IFactory<TKey, TVal>
    {
        protected abstract Dictionary<TKey, Func<TVal>> Operations { get; }
        public TVal Default { get; set; }
        private Dictionary<TKey, Func<TVal>> _operations;

        protected BaseFactory(TVal defaultValue)
        {
            // avoid creating the ops dictionary for each request
            this._operations = this.Operations;
            this.Default = defaultValue;
        }

        public TVal GetInstace(TKey key)
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("Creating instance for key: {key}", key);
            }

            if (this.Operations.ContainsKey(key)) return this.Operations[key]();
            return this.Default;
        }
    }
}
