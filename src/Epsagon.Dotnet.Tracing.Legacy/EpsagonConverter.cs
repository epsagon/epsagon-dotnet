using System.Collections.Generic;
using System.Linq;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation;

using Jaeger;

using OpenTracing.Tag;

namespace Epsagon.Dotnet.Tracing.Legacy {
    public static class EpsagonConverter {
        public static T GetValue<T>(this IReadOnlyDictionary<string, object> tags, string tagName) {
            if (tags.ContainsKey(tagName))
                return (T) tags[tagName];
            return default(T);
        }

        public static EpsagonEvent ToEvent(this Span span) {
            return Utils.TimeExecution(() => {
                var tags = span.GetTags();
                string id = tags.GetValue<string>("event.id");
                string origin = tags.GetValue<string>("event.origin");
                int errorCode = tags.GetValue<int>("event.error_code");
                double duration = span.FinishTimestampUtc
                    .GetValueOrDefault(span.StartTimestampUtc)
                    .Subtract(span.StartTimestampUtc)
                    .TotalSeconds;
                double startTime = span.StartTimestampUtc.ToUnixTime();
                string resourceName = tags.GetValue<string>("resource.name");
                string resourceType = tags.GetValue<string>("resource.type");
                string resourceOperation = tags.GetValue<string>("resource.operation");

                var metadata = span.GenerateMetadata();
                var resource = new EpsagonResource(resourceName, resourceOperation, resourceType, metadata);
                var epsagonEvent = new EpsagonEvent(startTime, duration, errorCode, id, origin, resource);

                if (tags.GetValue<bool>(Tags.Error.Key)) {
                    epsagonEvent.Exception = new EpsagonException();
                    epsagonEvent.Exception.Message = tags.GetValue<string>("error.message");
                    epsagonEvent.Exception.Traceback = tags.GetValue<string>("error.stack_trace");
                    epsagonEvent.Exception.Type = tags.GetValue<string>("error.type");
                    epsagonEvent.Exception.Time = tags.GetValue<double>("error.time");
                }

                return epsagonEvent;
            }, "ToEvent");
        }

        public static EpsagonTrace CreateTrace(IEnumerable<Span> spans) {
            return Utils.TimeExecution(() => {
                var config = Utils.CurrentConfig;

                return new EpsagonTrace(
                    version: "1.0.0",
                    token: config.Token,
                    appName: config.AppName,
                    exceptions: InstumentationExceptionsCollector.Exceptions,
                    events: spans.Select(span => span.ToEvent()),
                    platform: $".NET {System.Environment.Version.Major}.{System.Environment.Version.Minor}"
                );
            }, "CreateTrace");
        }

        public static IDictionary<string, object> GenerateMetadata(this Span span) {
            return Utils.TimeExecution(() => {
                var non_meta_tags = new[] { "sampler", "error", "resource", "event" };
                var tags = span.GetTags().Select(x => new { Key = x.Key.Split('.'), x.Value })
                    .Where(x => !non_meta_tags.Contains(x.Key.First()))
                    .Where(x => !Utils.IsNullOrDefault(x.Value));

                var metadata = new Dictionary<string, object>();
                foreach (var tag in tags) {
                    metadata[tag.Key.Last()] = tag.Value;
                }

                return metadata;
            }, "GenerateMetadata");
        }
    }
}
