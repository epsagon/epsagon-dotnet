using System.Collections.Generic;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonResource
    {
        public Dictionary<string, object> Metadata { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }
    }
}
