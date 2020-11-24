using System.Collections.Generic;
using Jaeger;
using Newtonsoft.Json;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation
{
    public class EpsagonLabels
    {
        public static void Add(string key, string value)
        {
            if (!(GlobalTracer.Instance?.ActiveSpan is Span span)) return;
            var tags = span.GetTags();

            Dictionary<string, string> labelsDict = new Dictionary<string, string>();
            if (tags.TryGetValue("epsagon.labels", out var labels))
            {
                labelsDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(labels.ToString());
            }

            labelsDict[key] = value;
            span.SetTag("epsagon.labels", JsonConvert.SerializeObject(labelsDict));
        }
    }
}
