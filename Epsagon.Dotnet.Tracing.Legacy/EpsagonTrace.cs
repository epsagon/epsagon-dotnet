using System;
using System.Collections.Generic;

namespace Epsagon.Dotnet.Tracing.Legacy
{
    public class EpsagonTrace
    {
       public string AppName { get; set; }
       public IEnumerable<EpsagonEvent> Events { get; set; }
       public IEnumerable<Exception> Exceptions { get; set; }
       public string Platform { get; set; }
       public string Token { get; set; }
       public string Version { get; set; }
    }
}
