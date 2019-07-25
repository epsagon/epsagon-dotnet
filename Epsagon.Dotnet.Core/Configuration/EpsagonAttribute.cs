using System;
namespace Epsagon.Dotnet.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class EpsagonAttribute : Attribute
    {
        public string Token { get; set; }
        public string AppName { get; set; }
        public bool MetadataOnly { get; set; }
        public bool UseSSL { get; set; }
        public string TraceCollectorURL { get; set; }
        public bool IsEpsagonDisabled { get; set; }
    }
}
