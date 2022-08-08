using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Epsagon.Dotnet.Core.Configuration;

using Newtonsoft.Json;

using OpenTracing;
using OpenTracing.Tag;

using Serilog;

namespace Epsagon.Dotnet.Core {
    public static class Utils {
        private static IEpsagonConfiguration _config;
        public static IEpsagonConfiguration CurrentConfig => _config;
        public static List<IDisposable> disposables = new List<IDisposable>();

        public static string SerializeObject(object obj) {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            });
        }

        public static Dictionary<string, string> DeserializeObject(string str) {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str, new JsonSerializerSettings {
                ContractResolver = new JsonLowerCaseUnderscoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static Dictionary<string, object> ToDictionary(this object obj) {
            return obj
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, object input) {
            SetDataIfNeeded(span, tagName, JsonConvert.SerializeObject(input, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, string input) {
            if ((!CurrentConfig?.MetadataOnly).GetValueOrDefault(false)) {
                span.SetTag(tagName, input);
            }
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, int input) {
            if ((!CurrentConfig?.MetadataOnly).GetValueOrDefault(false)) {
                span.SetTag(tagName, input);
            }
        }

        public static void SetIgnoredKeysIfNeeded(this ISpan span, string tagName, string attr) {
            try {
                    Dictionary<string, string> input = StrToDict(attr);
                    if (String.IsNullOrEmpty(CurrentConfig?.IgnoredKeys)) {
                        span.SetTag(tagName, string.Join(";", input.Select(x => x.Key + "=" + x.Value)));
                    } else {
                        foreach (var key in input.Keys) {
                        if (CurrentConfig.IgnoredKeys.Contains(key.Trim())){
                                input[key] = "*********";
                            }
                        }
                        span.SetTag(tagName, string.Join(";", input.Select(x => x.Key + "=" + x.Value)));  
                    } 
                }
                catch (Exception e) {
                    span.AddException(e);
                    throw;
                }
        }

        public static void AddException(this ISpan span, Exception e) {
            Tags.Error.Set(span, true);
            span.SetTag("event.error_code", 2); // exception
            span.SetTag("error.message", e.Message);
            span.SetTag("error.stack_trace", e.StackTrace);
            span.SetTag("error.type", e.GetType().Name);
            span.SetTag("error.time", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);
            span.Finish();
        }

        public static void RegisterConfiguration(IEpsagonConfiguration configuration) {
            _config = configuration;
        }

        public static bool IsNullOrDefault<T>(T argument) {
            if (argument is ValueType || argument != null) {
                return object.Equals(argument, GetDefault(argument.GetType()));
            }
            return true;
        }
        private static object GetDefault(Type type) {
            if (type.IsValueType) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static double ToUnixTime(this DateTime dateTime) {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() / 1000.0;
        }

        public static void DebugLogIfEnabled(string format, params object[] args) {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug)) {
                Log.Debug(format, args);
            }
        }

        public static void AddDisposable(IDisposable disposable) {
            disposables.Add(disposable);
        }

        public static T ExtractAttribute<T>(this object source, string name) where T : class {
            var type = source.GetType();
            var propertyInfo = type.GetProperty(name);
            var propertyValue = propertyInfo.GetValue(source);

            return propertyValue as T;
        }

        public static Dictionary<string, string> StrToDict(string obj) {
            return obj.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0], split => split[1]);
        }
    }
}
