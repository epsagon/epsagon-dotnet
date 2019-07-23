using System;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class KinesisHandler : EpsagonPipelineHandler
    {
        protected override string ServiceId => "Kinesis";

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
