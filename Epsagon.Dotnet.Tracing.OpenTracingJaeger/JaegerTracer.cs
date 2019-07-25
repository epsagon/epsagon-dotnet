using System;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Microsoft.Extensions.Logging;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger
{
    public class JaegerTracer
    {
        public static Jaeger.Tracer CreateTracer(ILoggerFactory loggerFactory)
        {
            var reporter = new LoggingReporter(loggerFactory);
            var sampler = new ConstSampler(true);

            return new Jaeger.Tracer.Builder("testing")
                .WithLoggerFactory(loggerFactory)
                .WithReporter(reporter)
                .WithSampler(sampler)
                .Build();
        }
    }
}
