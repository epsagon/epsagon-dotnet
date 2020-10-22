using System.Data.Common;
using Epsagon.Dotnet.Instrumentation.ADONET;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Epsagon.Dotnet.Instrumentation.EFCore
{
    public class EpsagonCommandInterceptor : DbCommandInterceptor
    {
        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            var command = base.CommandCreated(eventData, result);
            return new EpsagonTracedCommand(command);
        }
    }
}
