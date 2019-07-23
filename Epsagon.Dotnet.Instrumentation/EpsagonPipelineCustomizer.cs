using System;
using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace Epsagon.Dotnet.Instrumentation 
{
    public class EpsagonPipelineCustomizer : IRuntimePipelineCustomizer
    {
        public string UniqueName => "Epsagon pipeline customizer";

        public void Customize(Type type, RuntimePipeline pipeline)
        {
            
        }
    }
}
