using System.Collections.Generic;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Senders;
using Jaeger.Samplers;
using OpenTracing.Util;
using Epsagon.Dotnet.Core;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger
{
    public class JaegerTracer
    {
        public static InMemoryReporter memoryReporter = new InMemoryReporter();
        public static Tracer tracer;

        private static Tracer CreateTracer(IReporter reporter)
        {
            var sampler = new ConstSampler(true);
            tracer = new Tracer.Builder("epsagon-tracer")
                .WithReporter(new CompositeReporter(reporter))
                .WithSampler(sampler)
                .Build();

            if (!GlobalTracer.IsRegistered())
            {
                GlobalTracer.Register(tracer);
            }

            return tracer;
        }

        public static Tracer CreateTracer()
        {
            return CreateTracer(memoryReporter);
        }

        public static Tracer CreateRemoteTracer()
        {
            var sender = new HttpSender
                .Builder(Utils.CurrentConfig.OpenTracingCollectorURL)
                .WithAuth(Utils.CurrentConfig.Token)
                .Build();
            var reporter = new RemoteReporter.Builder().WithSender(sender);
            return CreateTracer(reporter.Build());
        }

        public static void Clear()
        {
            memoryReporter.Clear();
        }

        public static IEnumerable<Span> GetSpans() => memoryReporter.GetSpans();
    }
}
