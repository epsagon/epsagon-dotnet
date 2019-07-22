using System;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Config;
using Epsagon.Dotnet.Lambda;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DemoLambdaFunction
{
    public class Function : LambdaHandler<string, string>
    {
        [Epsagon(AppName = "Dotnet Test")]
        public override string HandlerFunction(string input, ILambdaContext context)
        {
            return input.ToUpper();
        }
    }
}
