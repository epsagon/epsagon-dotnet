using System;
using System.Net.Http;
using System.Threading.Tasks;
using Epsagon.Dotnet.Instrumentation;

namespace HttpCrash
{
    class Program
    {
        static async Task Main(string[] args)
        {
            EpsagonBootstrap.Bootstrap();
            await GetData();
            Console.WriteLine("EOF.\n");
        }

        public static async Task<HttpResponseMessage> GetData()
        {
            HttpClient client = new HttpClient();
            Console.WriteLine("Getting data");
            try {
                await client.PostAsync("https://github.com", new StringContent(""));
                var response = await client.PostAsync("https://fake", new StringContent(""));
                return response;
            }
            catch(HttpRequestException)
            {
                Console.WriteLine($"Endpoint not Found.");
            }
            return null;
        }
    }
}
