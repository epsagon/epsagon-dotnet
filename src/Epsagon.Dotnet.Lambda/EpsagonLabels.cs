using System.Collections.Generic;
using Newtonsoft.Json;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Lambda
{
    public class EpsagonLabels
    {
        public static readonly string LABELS_TAG = "aws.lambda.labels";
        public static Dictionary<string, string> labels = new Dictionary<string, string>();
        public static void Add(string key, string value) {
            OpenTracing.ITracer tracer =  GlobalTracer.Instance;
            EpsagonLabels.labels.Add(key, value);
        }

        public static void Set() {
            var tracer = GlobalTracer.Instance;
            tracer.ActiveSpan.SetTag(LABELS_TAG, JsonConvert.SerializeObject(
                EpsagonLabels.labels, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
        public static void Clear() {
            EpsagonLabels.labels.Clear();
        }
    }
}
