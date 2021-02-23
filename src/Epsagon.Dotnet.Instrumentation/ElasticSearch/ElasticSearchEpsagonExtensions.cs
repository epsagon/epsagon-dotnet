using System;

using Jaeger;

using Nest;

using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.ElasticSearch {
    public static class ElasticSearchEpsagonExtensions {
        public static ConnectionSettings UseEpasgon(this ConnectionSettings settings) {
            return settings
                .OnRequestCompleted(ElasticSearchEventsHandler.HandleRequestCompleted)
                .OnRequestDataCreated(ElasticSearchEventsHandler.HandleRequestDataCreated);
        }
    }
}
