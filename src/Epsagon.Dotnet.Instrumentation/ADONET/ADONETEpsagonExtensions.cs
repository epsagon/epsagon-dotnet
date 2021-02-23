using System.Data.Common;

namespace Epsagon.Dotnet.Instrumentation.ADONET {
    public static class ADONETEpsagonExtensions {
        public static DbConnection UseEpsagon(this DbConnection conn) => new EpsagonTracedConnection(conn);
    }
}
