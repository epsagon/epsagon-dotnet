using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Epsagon.Dotnet.Core;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.ADONET
{
    public class EpsagonTracedCommand : DbCommand
    {
        private readonly DbCommand _inner;

        /// <summary>
        /// Create a new instance of `EpsagonTracedCommand`
        /// wrapping the given command
        /// </summary>
        /// <param name="command">The `DbCommand` instance to wrap</param>
        public EpsagonTracedCommand(DbCommand command) => _inner = command;

        public override string CommandText { get => _inner.CommandText; set => _inner.CommandText = value; }
        public override int CommandTimeout { get => _inner.CommandTimeout; set => _inner.CommandTimeout = value; }
        public override CommandType CommandType { get => _inner.CommandType; set => _inner.CommandType = value; }
        public override bool DesignTimeVisible { get => _inner.DesignTimeVisible; set => _inner.DesignTimeVisible = value; }
        public override UpdateRowSource UpdatedRowSource { get => _inner.UpdatedRowSource; set => _inner.UpdatedRowSource = value; }
        protected override DbConnection DbConnection { get => _inner.Connection; set => _inner.Connection = value; }
        protected override DbTransaction DbTransaction { get => _inner.Transaction; set => _inner.Transaction = value; }
        protected override DbParameterCollection DbParameterCollection => _inner.Parameters;

        public override void Cancel() => _inner.Cancel();
        public override void Prepare() => _inner.Prepare();
        protected override DbParameter CreateDbParameter() => _inner.CreateParameter();

        private void SpanDefaults(OpenTracing.ISpan span)
        {
            var operation = CommandText.Split().First().ToUpper();
            span.SetOperationName(operation);

            span.SetTag("event.id", Guid.NewGuid().ToString());
            span.SetTag("event.origin", "system-data");
            span.SetTag("resource.type", "database");
            span.SetTag("resource.name", DbConnection.Database);
            span.SetTag("resource.operation", operation);
            span.SetTag("sql.driver", DbConnection.GetType().FullName);
            span.SetTag("sql.statement", CommandText);
            span.SetTag("sql.connection_string", DbConnection.ConnectionString);
        }

        public override int ExecuteNonQuery()
        {
            using (var scope = GlobalTracer.Instance
                            .BuildSpan("")
                            .WithStartTimestamp(DateTime.Now)
                            .StartActive(finishSpanOnDispose: true))
            {
                int affected_rows = -1;
                var span = scope.Span;

                SpanDefaults(span);
                span.SetTag("sql.table_name", ""); // check other instrum to see how this is obtained
                span.SetTag("sql.parameters", ""); // check other instrum to see how this is obtained

                try { affected_rows = _inner.ExecuteNonQuery(); }
                catch (Exception e)
                {
                    span.AddException(e);
                    throw;
                }

                span.SetTag("sql.cursor_row_count", affected_rows);
                return affected_rows;
            }
        }

        public override object ExecuteScalar()
        {
            using (var scope = GlobalTracer.Instance
                            .BuildSpan("")
                            .WithStartTimestamp(DateTime.Now)
                            .StartActive(finishSpanOnDispose: true))
            {
                var span = scope.Span;
                object result = null;

                SpanDefaults(span);
                span.SetTag("sql.table_name", ""); // check other instrum to see how this is obtained
                span.SetTag("sql.parameters", ""); // check other instrum to see how this is obtained
                span.SetTag("sql.cursor_row_count", 1);

                try
                {
                    result = _inner.ExecuteScalar();
                    span.SetDataIfNeeded("sql.scalar.result", result);
                }
                catch (Exception e)
                {
                    span.AddException(e);
                    throw;
                }

                return result;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            using (var scope = GlobalTracer.Instance
                            .BuildSpan("")
                            .WithStartTimestamp(DateTime.Now)
                            .StartActive(finishSpanOnDispose: true))
            {
                var span = scope.Span;
                DbDataReader reader = null;

                SpanDefaults(span);
                span.SetTag("sql.table_name", ""); // check other instrum to see how this is obtained
                span.SetTag("sql.parameters", ""); // check other instrum to see how this is obtained

                try
                {
                    reader = _inner.ExecuteReader(behavior);
                    span.SetTag("sql.cursor_row_count", reader.RecordsAffected);
                }
                catch (Exception e)
                {
                    span.AddException(e);
                    throw;
                }

                return reader;
            }
        }
    }
}
