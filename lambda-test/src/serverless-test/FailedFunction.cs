using Amazon.Lambda.Core;
using Epsagon.Dotnet.Lambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace serverless_test
{
    public class FailedFunction : LambdaHandler<string, string>
    {
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            throw new System.Exception("failing function");
        }
    }
}
