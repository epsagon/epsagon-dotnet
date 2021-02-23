using System;
using System.Collections.Generic;

namespace Epsagon.Dotnet.Instrumentation.Handlers.SES {
    public class SESOperationsFactory : BaseOperationsFactory {
        protected override Dictionary<string, Func<IOperationHandler>> Operations => new Dictionary<string, Func<IOperationHandler>>() {

        };
    }
}
