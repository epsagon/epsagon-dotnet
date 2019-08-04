using System.Collections.Generic;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger
{
    public class JaegerTracer
    {
        public static InMemoryReporter memoryReporter = new InMemoryReporter();

        public static Tracer CreateTracer(ILoggerFactory loggerFactory)
        {
            var loggingReporter = new LoggingReporter(loggerFactory);
            var sampler = new ConstSampler(true);

            return new Tracer.Builder("epsagon-tracer")
                .WithLoggerFactory(loggerFactory)
                .WithReporter(new CompositeReporter(loggingReporter, memoryReporter))
                .WithSampler(sampler)
                .Build();
        }

        public static IEnumerable<Span> GetSpans() => memoryReporter.GetSpans();
    }
}
