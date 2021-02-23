using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Epsagon.Dotnet.Core;

using Jaeger;
using Jaeger.Reporters;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger {
    internal class DebugReporter : IReporter {
        public Task CloseAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public void Report(Span span) {
            var tags = span.GetTags()
                .Where(t => t.Key != null && t.Value != null)
                .Select(t => $"Tag(key={t.Key}, value={Truncate(t.Value.ToString(), 25)})");

            Utils.DebugLogIfEnabled(
                "Reporting Span(OperationName={opname}, Tags=[\n\t\t{tags}])",
                span.OperationName,
                string.Join($"{Environment.NewLine}\t\t", tags));
        }

        private string Truncate(string str, int length) {
            str = str ?? "";
            if (str.Length <= length)
                return str;
            return str.Substring(0, length - 3) + "...";
        }
    }
}
