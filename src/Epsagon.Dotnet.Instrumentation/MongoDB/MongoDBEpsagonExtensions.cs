using Epsagon.Dotnet.Core;
using MongoDB.Driver;

namespace Epsagon.Dotnet.Instrumentation.MongoDB
{
    public static class MongoDBEpsagonExtensions
    {
        public static MongoClient WithEpsagon(this MongoClient client)
        {
            var isDisabled = Utils.CurrentConfig.IsEpsagonDisabled;
            if (isDisabled) return client;

            var epsagonSettings = client.Settings.Clone();
            epsagonSettings.ClusterConfigurator = builder => builder.Subscribe(new MongoDBEventsSubscriber());

            return new MongoClient(epsagonSettings);
        }
    }
}
