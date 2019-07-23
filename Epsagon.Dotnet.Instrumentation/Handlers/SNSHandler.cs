using System;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class SNSHandler : EpsagonPipelineHandler
    {
        protected override string ServiceId => "SNS";

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
