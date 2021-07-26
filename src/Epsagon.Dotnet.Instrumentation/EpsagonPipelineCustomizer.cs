using System;

using Amazon.Runtime.Internal;

using Epsagon.Dotnet.Core;

namespace Epsagon.Dotnet.Instrumentation {
    public class EpsagonPipelineCustomizer : IRuntimePipelineCustomizer {
        public string UniqueName => "Epsagon pipeline customizer";

        public void Customize(Type type, RuntimePipeline pipeline) {
            var handler = new HandlerFactory(type.Name).GetInstace(type);
            pipeline.AddHandler(handler);

            Utils.DebugLogIfEnabled("done adding pipeline handler, type: {}", type.Name);
        }
    }
}
