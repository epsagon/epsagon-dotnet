using System;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class SQSHandler : EpsagonPipelineHandler
    {
        protected override string ServiceId => "SQS";

        protected override void AfterExecution(IExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        protected override void AfterExecution<T>(IExecutionContext executionContext, T res)
        {
            throw new NotImplementedException();
        }

        protected override void BeforeExecution(IExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}
