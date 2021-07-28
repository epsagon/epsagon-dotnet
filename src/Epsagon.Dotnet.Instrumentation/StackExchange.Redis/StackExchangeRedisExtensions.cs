using System;

using StackExchange.Redis;
using StackExchange.Redis.Profiling;

namespace Epsagon.Dotnet.Instrumentation.StackExchange.Redis {
    public static class StackExchangeRedisExtensions {
        /// <summary>
        /// register epsagon tracer on the connection
        /// </summary>
        /// <param name="connection">the connection to trace</param>
        /// <returns>the traced connection</returns>
        public static ConnectionMultiplexer UseEpsagon(this ConnectionMultiplexer connection) {
            connection.RegisterProfiler(EpsagonProfilingSession.SessionProvider(connection));
            return connection;
        }
    }
}
