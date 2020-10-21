using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Epsagon.Dotnet.Core.Configuration;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Tag;
using System.Diagnostics;
using Serilog;

namespace Epsagon.Dotnet.Core
{
    public static class Utils
    {
        private static IEpsagonConfiguration _config;
        public static IEpsagonConfiguration CurrentConfig => _config;

        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        public static Dictionary<string, string> DeserializeObject(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str, new JsonSerializerSettings
            {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            return obj
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, object input)
        {
            SetDataIfNeeded(span, tagName, JsonConvert.SerializeObject(input, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, string input)
        {
            if (!Utils.CurrentConfig.MetadataOnly)
            {
                span.SetTag(tagName, input);
            }
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, int input)
        {
            if (!Utils.CurrentConfig.MetadataOnly)
            {
                span.SetTag(tagName, input);
            }
        }

        public static void AddException(this ISpan span, Exception e)
        {
            Tags.Error.Set(span, true);
            span.SetTag("event.error_code", 2); // exception
            span.SetTag("error.message", e.Message);
            span.SetTag("error.stack_trace", e.StackTrace);
            span.SetTag("error.type", e.GetType().Name);
            span.SetTag("error.time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);
            span.Finish();
        }

        public static void RegisterConfiguration(IEpsagonConfiguration configuration)
        {
            _config = configuration;
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            if (argument is ValueType || argument != null)
            {
                return object.Equals(argument, GetDefault(argument.GetType()));
            }
            return true;
        }
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static double ToUnixTime(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() / 1000.0;
        }

        public static void DebugLogIfEnabled(string format, params object[] args)
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug(format, args);
            }
        }

        public static T TimeExecution<T>(Func<T> func, string fname = "")
        {
            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                return func();
            }
            finally
            {
                stopWatch.Stop();
                DebugLogIfEnabled("Execution time: {time}ms, Function: {name}", stopWatch.ElapsedMilliseconds, fname);
            }
        }

        public static void TimeExecution(Action func, string fname = "")
        {
            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                func();
            }
            finally
            {
                stopWatch.Stop();
                DebugLogIfEnabled("Execution time: {time}ms, Function: {name}", stopWatch.ElapsedMilliseconds, fname);
            }
        }
    }
}
