using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Epsagon.Dotnet.Lambda;
using Newtonsoft.Json;

namespace EpsagonDotnetFunction
{
    public class LambdaInvoker
    {
        AmazonLambdaClient _client;

        public LambdaInvoker()
        {
            _client = new AmazonLambdaClient();
        }

        public string InvokeLambda(string functionName)
        {
            return EpsagonGeneralHandler.Handle(() =>
            {
                var request = new InvokeRequest()
                {
                    FunctionName = functionName,
                    InvocationType = InvocationType.RequestResponse,
                    Payload = JsonConvert.SerializeObject(new { Key1 = "value1", Key2 = "value2", Key3 = "value3" })
                };

                var res = _client.InvokeAsync(request).Result;
                return new StreamReader(res.Payload).ReadToEnd();
            });
        }
    }
}
