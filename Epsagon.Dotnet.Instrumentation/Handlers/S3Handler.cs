using System;
using Amazon.Runtime;

namespace Epsagon.Dotnet.Instrumentation.Handlers
{
    public class S3Handler : EpsagonPipelineHandler
    {
        protected override string ServiceId => "S3";

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
