using System.Linq;
using System.Collections.Generic;
using Jaeger;
using System;
using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Core.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

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

            epsagonEvent.Exception = new EpsagonException();
            epsagonEvent.Exception.Message = tags.GetValue<string>("error.message");
            epsagonEvent.Exception.Traceback = tags.GetValue<string>("error.stack_trace");
            epsagonEvent.Exception.Type = tags.GetValue<string>("error.type");
            epsagonEvent.Exception.Time = tags.GetValue<double>("error.time");

            epsagonEvent.Resource = new EpsagonResource();
            epsagonEvent.Resource.Name = tags.GetValue<string>("resource.name");
            epsagonEvent.Resource.Type = tags.GetValue<string>("resource.type");
            epsagonEvent.Resource.Operation = tags.GetValue<string>("aws.operation");

            epsagonEvent.Resource.Metadata = new EpsagonMetadata();
            epsagonEvent.Resource.Metadata.Region = tags.GetValue<string>("aws.region");
            epsagonEvent.Resource.Metadata.AwsAccount = tags.GetValue<string>("aws.account");
            epsagonEvent.Resource.Metadata.Memory = tags.GetValue<string>("aws.lambda.memory");
            epsagonEvent.Resource.Metadata.ColdStart = tags.GetValue<bool>("aws.lambda.cold_start");
            epsagonEvent.Resource.Metadata.ReturnValue = tags.GetValue<string>("aws.lambda.return_value");
            epsagonEvent.Resource.Metadata.LogGroupName = tags.GetValue<string>("aws.lambda.log_group_name");
            epsagonEvent.Resource.Metadata.LogStreamName = tags.GetValue<string>("aws.lambda.log_stream_name");
            epsagonEvent.Resource.Metadata.FunctionVersion = tags.GetValue<string>("aws.lambda.function_version");

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
    }
}
