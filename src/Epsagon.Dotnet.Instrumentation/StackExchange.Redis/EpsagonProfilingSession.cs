using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using Epsagon.Dotnet.Core;

using OpenTracing.Util;

using StackExchange.Redis;
using StackExchange.Redis.Profiling;

namespace Epsagon.Dotnet.Instrumentation.StackExchange.Redis {
    public class EpsagonProfilingSession : IDisposable {
        public ProfilingSession Session { get; set; }
        public ConnectionMultiplexer RedisConnection { get; set; }

        public EpsagonProfilingSession(ConnectionMultiplexer connection) {
            this.Session = new ProfilingSession();
            this.RedisConnection = connection;
        }

        public static Func<ProfilingSession> SessionProvider(ConnectionMultiplexer connection) {
            return () => {
                var eps = new EpsagonProfilingSession(connection);
                Utils.AddDisposable(eps);

                return eps.Session;
            };
        }

        public void Dispose() {
            var redisCommands = this.Session.FinishProfiling();
            this.ReportCommands(redisCommands);
        }

        private void ReportCommands(IEnumerable<IProfiledCommand> commands) {
            foreach (var command in commands) {
                var scope = GlobalTracer.Instance
                    .BuildSpan(command.Command)
                    .WithStartTimestamp(new DateTimeOffset(command.CommandCreated))
                    .StartActive();

                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("event.origin", "stackexchange.redis");
                scope.Span.SetTag("resource.type", "redis");
                scope.Span.SetTag("resource.name", RedisConnection.Configuration);
                scope.Span.SetTag("resource.operation", command.Command);
                scope.Span.SetTag("redis.db_index", command.Db);
                scope.Span.SetTag("redis.client_name", RedisConnection.ClientName);
                scope.Span.SetDataIfNeeded("redis.flags", command.Flags.ToDictionary());

                scope.Span.Finish(new DateTimeOffset(command.CommandCreated + command.ElapsedTime));
            }
        }
    }
}
