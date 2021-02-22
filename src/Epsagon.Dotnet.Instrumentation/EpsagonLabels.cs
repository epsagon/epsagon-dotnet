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
			AddLabel(
				key,
				value);
		}

		public static void Add(string key, int value)
		{
			AddLabel(
				key,
				value);
		}

		public static void Add(string key, double value)
		{
			AddLabel(
				key,
				value);
		}

		public static void Add(string key, bool value)
		{
			AddLabel(
				key,
				value);
		}

		private static void AddLabel(string key, object value)
		{
			if (!(GlobalTracer.Instance?.ActiveSpan is Span span)) return;
			var tags = span.GetTags();

			Dictionary<string, object> labelsDict = new Dictionary<string, object>();

			if (tags.TryGetValue(
				"epsagon.labels",
				out var labels))
			{
				labelsDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(labels.ToString());
			}

			labelsDict[key] = value;
			span.SetTag(
				"epsagon.labels",
				JsonConvert.SerializeObject(labelsDict));
		}
	}
}
