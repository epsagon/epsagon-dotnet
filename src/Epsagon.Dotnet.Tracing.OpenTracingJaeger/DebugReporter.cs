using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epsagon.Dotnet.Core;
using Jaeger;
using Jaeger.Reporters;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger
{
    internal class DebugReporter : IReporter
    {
        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Report(Span span)
        {
            var tags = span.GetTags().Select(t => $"Tag(key={t.Key}, value={Truncate(t.Value.ToString(), 15)})");
            Utils.DebugLogIfEnabled(
                "Reporting Span(OperationName={opname}, Tags=[{tags}])",
                span.OperationName,
                string.Join(Environment.NewLine, tags));
        }

        private string Truncate(string str, int length)
        {
            if (str.Length <= length) return str;
            return str.Substring(0, length - 3) + "...";
        }
    }
}
