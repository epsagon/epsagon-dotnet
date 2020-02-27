using System;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation.MongoDB;
using Epsagon.Dotnet.Lambda;
using Epsagon.Dotnet.Tracing.Legacy;
using Epsagon.Dotnet.Tracing.OpenTracingJaeger;
using MongoDB.Driver;

namespace MongoDBApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            EpsagonBootstrap.Bootstrap();

            var client = new MongoClient("mongodb://localhost:27017").WithEpsagon();
            var database = client.GetDatabase("epsagon-mongodb-app");
            var usersCollection = database.GetCollection<User>("users");

            usersCollection.InsertOne(new User
            {
                Email = "test@test.com",
                PasswordHash = "secret"
            });


            var trace = EpsagonConverter.CreateTrace(JaegerTracer.GetSpans());
            Utils.DebugLogIfEnabled("sent trace, {trace}", Utils.SerializeObject(trace));
        }
    }
}
