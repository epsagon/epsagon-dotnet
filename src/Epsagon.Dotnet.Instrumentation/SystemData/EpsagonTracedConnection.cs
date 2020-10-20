using System.Data;
using System.Data.Common;

namespace Epsagon.Dotnet.Instrumentation.SystemData
{
    /// <summary>
    /// Specialized DbConnection object to allow tracing using Epsagon's tracer
    /// 
    /// Delegates all logic to the inner connection, 
    /// apart from the `CreateDbCommand` that wraps the 
    /// generated command with an EpsagonTracedCommand instance
    /// to allow tracing the command execution
    /// </summary>
    public class EpsagonTracedConnection : DbConnection
    {
        private readonly DbConnection _inner;

        /// <summary>
        /// Create a new instance of `EpsagonTracedConnection` 
        /// wrapping the given connection
        /// </summary>
        /// <param name="connection">The `DbConnection` instance to wrap</param>
        public EpsagonTracedConnection(DbConnection connection) => _inner = connection;

        public override string ConnectionString { get => _inner.ConnectionString; set => _inner.ConnectionString = value; }

        public override string Database => _inner.Database;
        public override string DataSource => _inner.DataSource;
        public override string ServerVersion => _inner.ServerVersion;
        public override ConnectionState State => _inner.State;

        public override void Open() => _inner.Open();
        public override void Close() => _inner.Close();
        public override void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => _inner.BeginTransaction(isolationLevel);
        protected override DbCommand CreateDbCommand() => new EpsagonTracedCommand(_inner.CreateCommand());
    }
}
