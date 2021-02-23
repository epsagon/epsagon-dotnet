using System;
using System.Collections.Generic;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Tracing.Legacy.TraceSenders;

namespace Epsagon.Dotnet.Tracing.Legacy {
    public class EpsagonTrace {
        public string AppName { get; set; }
        public IEnumerable<EpsagonEvent> Events { get; set; }
        public IEnumerable<Exception> Exceptions { get; set; }
        public string Platform { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }

        public EpsagonTrace(
            string appName,
            string platform,
            string token,
            string version,
            IEnumerable<EpsagonEvent> events,
            IEnumerable<Exception> exceptions
        ) {
            this.AppName = appName;
            this.Platform = platform;
            this.Token = token;
            this.Version = version;
            this.Events = events;
            this.Exceptions = exceptions;
        }

        public static void SendTrace(EpsagonTrace trace) {
            Utils.TimeExecution(() => {
                var sender = GetTraceSender();
                sender.SendTrace(trace);
            }, "SendTrace");
        }

        public static ITraceSender GetTraceSender() {
            var config = Utils.CurrentConfig;

            if (config.UseLogsTransport) {
                return new LogTraceSender();
            }

            return new HTTPTraceSender(config.TraceCollectorURL, config.Token);
        }
    }
}
