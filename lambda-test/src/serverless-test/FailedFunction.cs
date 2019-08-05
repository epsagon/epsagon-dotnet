using Amazon.Lambda.Core;
using Epsagon.Dotnet.Lambda;

namespace serverless_test
{
    public class FailedFunction : LambdaHandler<string, string>
    {
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            throw new  System.Exception("failing function");
        }
    }
}
