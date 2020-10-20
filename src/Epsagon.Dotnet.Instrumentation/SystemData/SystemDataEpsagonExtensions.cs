using System.Data.Common;

namespace Epsagon.Dotnet.Instrumentation.SystemData
{
    public static class SystemDataEpsagonExtensions
    {
        public static DbConnection UseEpsagon(this DbConnection conn) => new EpsagonTracedConnection(conn);
    }
}
