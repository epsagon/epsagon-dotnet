using System;
using System.Data;
using System.Data.Common;
using System.Linq;

using Epsagon.Dotnet.Core;

using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.ADONET {
    public class EpsagonTracedCommand : DbCommand {
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

        private void SpanDefaults(OpenTracing.ISpan span) {
            var operation = CommandText.Split().First().ToUpper();
            span.SetOperationName(operation);

            var parameters = DbParameterCollection
                .OfType<DbParameter>()
                .Select(param => new {
                    name = param.ParameterName,
                    type = Enum.GetName(typeof(DbType), param.DbType),
                    value = param.Value
                })
                .ToList();

            span.SetTag("event.id", Guid.NewGuid().ToString());
            span.SetTag("event.origin", "system-data");
            span.SetTag("resource.type", "database");
            span.SetTag("resource.name", DbConnection.Database);
            span.SetTag("resource.operation", operation);
            span.SetTag("sql.driver", DbConnection.GetType().FullName);
            span.SetTag("sql.statement", CommandText);
            span.SetTag("sql.table_name", TableNameExtractor.ExtractTableName(CommandText));
            span.SetTag("sql.connection_string", DbConnection.ConnectionString);
            span.SetDataIfNeeded("sql.parameters", parameters);
        }

        public override int ExecuteNonQuery() {
            var affected_rows = -1;
            var code_executed = false;
            try {
                using (var scope = GlobalTracer.Instance
                                .BuildSpan("")
                                .WithStartTimestamp(DateTime.Now)
                                .StartActive(finishSpanOnDispose: true)) {
                    var span = scope.Span;

                    SpanDefaults(span);

                    try {
                        affected_rows = _inner.ExecuteNonQuery();
                        code_executed = true;
                    } catch (Exception e) {
                        span.AddException(e);
                        throw;
                    }

                    span.SetTag("sql.cursor_row_count", affected_rows);
                    return affected_rows;
                }
            } catch (Exception) {
                if (!code_executed)
                    return _inner.ExecuteNonQuery();
                else
                    return affected_rows;
            }
        }

        public override object ExecuteScalar() {
            object result = null;
            var code_executed = false;
            try {
                using (var scope = GlobalTracer.Instance
                                .BuildSpan("")
                                .WithStartTimestamp(DateTime.Now)
                                .StartActive(finishSpanOnDispose: true)) {
                    var span = scope.Span;

                    SpanDefaults(span);
                    span.SetTag("sql.cursor_row_count", 1);

                    try {
                        result = _inner.ExecuteScalar();
                        code_executed = true;
                        span.SetDataIfNeeded("sql.scalar.result", result);
                    } catch (Exception e) {
                        span.AddException(e);
                        throw;
                    }

                    return result;
                }
            } catch {
                if (!code_executed)
                    return _inner.ExecuteScalar();
                else
                    return result;
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            DbDataReader reader = null;
            var code_executed = false;
            try {
                using (var scope = GlobalTracer.Instance
                                .BuildSpan("")
                                .WithStartTimestamp(DateTime.Now)
                                .StartActive(finishSpanOnDispose: true)) {
                    var span = scope.Span;

                    SpanDefaults(span);

                    try {
                        reader = _inner.ExecuteReader(behavior);
                        code_executed = true;
                        span.SetTag("sql.cursor_row_count", reader.RecordsAffected);
                    } catch (Exception e) {
                        span.AddException(e);
                        throw;
                    }

                    return reader;
                }
            } catch {
                if (!code_executed)
                    return _inner.ExecuteReader(behavior);
                else
                    return reader;
            }
        }
    }
}
