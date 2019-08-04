using System;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonEvent
    {
        public double? StartTime { get; set; }
        public double? Duration { get; set; }
        public int ErrorCode { get; set; }
        public string Id { get; set; }
        public string Origin { get; set; }
        public EpsagonResource Resource { get; set; }
    }
}
