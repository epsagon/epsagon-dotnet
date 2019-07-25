using System;
using System.Collections.Generic;
using Amazon.Runtime;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class EmptyHandler : BaseServiceHandler
    {
        public override void HandleAfter(IExecutionContext executionContext, IScope scope)
        {
        }

        public override void HandleBefore(IExecutionContext executionContext, IScope scope)
        {
        }
    }
}
