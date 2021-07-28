using System;
using System.Collections.Generic;
using System.Reflection;

using Epsagon.Dotnet.Core;

using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;

using OpenTracing.Util;

namespace Epsagon.Dotnet.Tracing.OpenTracingJaeger {
    public class JaegerTracer {
        public static InMemoryReporter memoryReporter = new InMemoryReporter();
        public static Tracer tracer;

        private static Tracer CreateTracer(IReporter reporter) {
            var sampler = new ConstSampler(true);
            tracer = new Tracer.Builder(Utils.CurrentConfig.AppName)
                .WithReporter(new CompositeReporter(reporter, new DebugReporter()))
                .WithSampler(sampler)
                .WithTag("library.version", Assembly.GetAssembly(typeof(Epsagon.Dotnet.Core.Utils)).GetName().Version.ToString())
                .WithTag("library.platform", $".NET {System.Environment.Version.Major}.{System.Environment.Version.Minor}")
                .WithTag("runtime", "opentracing-dotnet")
                .Build();

            if (!GlobalTracer.IsRegistered()) {
                Utils.DebugLogIfEnabled("register type: {t}", reporter.GetType());
                GlobalTracer.Register(tracer);
            }

            return tracer;
        }

        public static Tracer CreateTracer() {
            return CreateTracer(memoryReporter);
        }

        public static Tracer CreateRemoteTracer() {
            var sender = new HttpSender
                .Builder(Utils.CurrentConfig.OpenTracingCollectorURL)
                .WithAuth(Utils.CurrentConfig.Token)
                .Build();
            var reporter = new RemoteReporter.Builder().WithSender(sender);
            return CreateTracer(reporter.Build());
        }

        public static void Clear() {
            memoryReporter.Clear();
        }

        public static IEnumerable<Span> GetSpans() {
            foreach (var disposable in Utils.disposables) {
                disposable.Dispose();
            }

            return memoryReporter.GetSpans();
        }
    }
}
