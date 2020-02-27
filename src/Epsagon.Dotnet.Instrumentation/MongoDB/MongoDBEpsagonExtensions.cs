using MongoDB.Driver;

namespace Epsagon.Dotnet.Instrumentation.MongoDB
{
    public static class MongoDBEpsagonExtensions
    {
        public static MongoClient WithEpsagon(this MongoClient client)
        {
            var epsagonSettings = client.Settings.Clone();
            epsagonSettings.ClusterConfigurator = builder => builder.Subscribe(new MongoDBEventsSubscriber());

            return new MongoClient(epsagonSettings);
        }
    }
}
