using System.Linq;
using System.Collections.Generic;
using Jaeger;
using System;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Core.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using OpenTracing.Tag;
using Newtonsoft.Json;

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
            var epsagonEvent = new EpsagonEvent();

            double startTime = tags.GetValue<double>("event.start_time");
            double duration = tags.GetValue<double>("event.duration");

            epsagonEvent.StartTime = startTime != 0 ? startTime : (double?)null;
            epsagonEvent.Duration = duration;
            epsagonEvent.ErrorCode = tags.GetValue<int>("event.error_code");
            epsagonEvent.Id = tags.GetValue<string>("event.id");
            epsagonEvent.Origin = tags.GetValue<string>("event.origin");

            if (tags.GetValue<bool>(Tags.Error.Key))
            {
                epsagonEvent.Exception = new EpsagonException();
                epsagonEvent.Exception.Message = tags.GetValue<string>("error.message");
                epsagonEvent.Exception.Traceback = tags.GetValue<string>("error.stack_trace");
                epsagonEvent.Exception.Type = tags.GetValue<string>("error.type");
                epsagonEvent.Exception.Time = tags.GetValue<double>("error.time");
            }

            epsagonEvent.Resource = new EpsagonResource();
            epsagonEvent.Resource.Name = tags.GetValue<string>("resource.name");
            epsagonEvent.Resource.Type = tags.GetValue<string>("resource.type");
            epsagonEvent.Resource.Operation = tags.GetValue<string>("aws.operation");

            var metadata = new Dictionary<string, object>();
            metadata.Add("Region", tags.GetValue<string>("aws.region"));
            metadata.Add("AwsAccount", tags.GetValue<string>("aws.account"));
            metadata.Add("Memory", tags.GetValue<string>("aws.lambda.memory"));
            metadata.Add("ColdStart", tags.GetValue<bool>("aws.lambda.cold_start"));
            metadata.Add("ReturnValue", tags.GetValue<string>("aws.lambda.return_value"));
            metadata.Add("LogGroupName", tags.GetValue<string>("aws.lambda.log_group_name"));
            metadata.Add("LogStreamName", tags.GetValue<string>("aws.lambda.log_stream_name"));
            metadata.Add("FunctionVersion", tags.GetValue<string>("aws.lambda.function_version"));

            Log.Debug("meta: {@meta}", metadata);
            var newMeta = JsonConvert.DeserializeObject<Dictionary<string, object>>(tags.GetValue<string>("resource.metadata") ?? "{}");
            Log.Debug("new: {@new}", newMeta);

            newMeta.ToList().ForEach(x => metadata[x.Key] = x.Value);
            epsagonEvent.Resource.Metadata = metadata
                .Where(x => !IsNullOrDefault(x.Value))
                .ToDictionary(x => x.Key, x => x.Value);

            return epsagonEvent;
        }

        public static EpsagonTrace CreateTrace(IEnumerable<Span> spans)
        {
            Log.Debug("creating trace, spans: {@spans}", spans);
            var config = Utils.CurrentConfig;

            return new EpsagonTrace
            {
                AppName = config.AppName,
                Token = config.Token,
                Platform = $".NET {System.Environment.Version.Major}.{System.Environment.Version.Minor}",
                Version = "1.0.0",
                Events = spans.Select(span => span.ToEvent()),
                Exceptions = new List<Exception>()
            };
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            if (argument is ValueType || argument != null)
            {
                return object.Equals(argument, GetDefault(argument.GetType()));
            }
            return true;
        }


        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

    }
}
