using System.Collections.Generic;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger
{
    public class JaegerTracer
    {
        public static InMemoryReporter memoryReporter = new InMemoryReporter();
        public static Tracer tracer;

        public static Tracer CreateTracer(ILoggerFactory loggerFactory)
        {
            var loggingReporter = new LoggingReporter(loggerFactory);
            var sampler = new ConstSampler(true);
            tracer = new Tracer.Builder("epsagon-tracer")
                .WithLoggerFactory(loggerFactory)
                .WithReporter(new CompositeReporter(loggingReporter, memoryReporter))
                .WithSampler(sampler)
                .Build();

            GlobalTracer.Register(tracer);
            return tracer;
        }

        public static void Clear()
        {
            memoryReporter.Clear();
        }

        public static IEnumerable<Span> GetSpans() => memoryReporter.GetSpans();
    }
}
