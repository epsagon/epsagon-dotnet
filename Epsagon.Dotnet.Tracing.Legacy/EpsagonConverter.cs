using System.Linq;
using System.Collections.Generic;
using Jaeger;
using System;
using Epsagon.Dotnet.Core;
using OpenTracing.Tag;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public static class EpsagonConverter
    {
        public static T GetValue<T>(this IReadOnlyDictionary<string, object> tags, string tagName)
        {
            if (tags.ContainsKey(tagName)) return (T)tags[tagName];
            return default(T);
        }

        public static EpsagonEvent ToEvent(this Span span)
        {
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

            var resource = new EpsagonResource(resourceName, resourceOperation, resourceType, span.GenerateMetadata());
            var epsagonEvent = new EpsagonEvent(startTime, duration, errorCode, id, origin, resource);

            if (tags.GetValue<bool>(Tags.Error.Key))
            {
                epsagonEvent.Exception = new EpsagonException();
                epsagonEvent.Exception.Message = tags.GetValue<string>("error.message");
                epsagonEvent.Exception.Traceback = tags.GetValue<string>("error.stack_trace");
                epsagonEvent.Exception.Type = tags.GetValue<string>("error.type");
                epsagonEvent.Exception.Time = tags.GetValue<double>("error.time");
            }

            return epsagonEvent;
        }

        public static EpsagonTrace CreateTrace(IEnumerable<Span> spans)
        {
            var config = Utils.CurrentConfig;

            return new EpsagonTrace(
                version: "1.0.0",
                token: config.Token,
                appName: config.AppName,
                exceptions: new List<Exception>(),
                events: spans.Select(span => span.ToEvent()),
                platform: $".NET {System.Environment.Version.Major}.{System.Environment.Version.Minor}"
            );
        }

        public static IDictionary<string, object> GenerateMetadata(this Span span)
        {
            var tags = span.GetTags();
            var metadata = tags
                .Select(x => new { Key = x.Key.Split('.'), Value = x.Value })
                .Where(x => x.Key.First().ToLower() == "aws")
                .Where(x => !Utils.IsNullOrDefault(x.Value))
                .ToDictionary(x => x.Key.Last(), x => x.Value);

            return metadata;
        }
    }
}
