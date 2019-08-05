using System;
using Epsagon.Dotnet.Core.Configuration;
using Newtonsoft.Json;
using OpenTracing;
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
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static void SetDataIfNeeded(this ISpan span, string tagName, object input)
        {
            if (!Utils.CurrentConfig.MetadataOnly)
            {
                span.SetTag(tagName, JsonConvert.SerializeObject(input));
            }
        }

        public static void RegisterConfiguration(IEpsagonConfiguration configuration) {
            _config = configuration;
        }
    }
}
