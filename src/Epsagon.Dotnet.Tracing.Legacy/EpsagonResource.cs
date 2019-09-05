using System.Collections.Generic;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonResource
    {
        public IDictionary<string, object> Metadata { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public string Type { get; set; }

        public EpsagonResource(
            string name,
            string operation,
            string type,
            IDictionary<string, object> metadata
        ) {
            this.Name = name;
            this.Operation = operation;
            this.Type = type;
            this.Metadata = metadata;
        }
    }
}
