using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

using Epsagon.Dotnet.Core;
using Epsagon.Dotnet.Instrumentation.ADONET;

using Microsoft.EntityFrameworkCore.Diagnostics;

using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.EFCore {
    /// <summary>
    /// Handle DbLogger Diagnostic Events
    /// </summary>
    public class DbDiagnosticTracer : IObserver<KeyValuePair<string, object>> {
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        /// <summary>
        /// Handle new events from the observable, delegating them the the
        /// relevant handler method
        /// </summary>
        /// <param name="value">next diagnostic event</param>
        public void OnNext(KeyValuePair<string, object> value) {
            if (value.Key == RelationalEventId.CommandExecuted.Name) {
                OnCommandFinished(value.Value as CommandExecutedEventData);
            } else if (value.Key == RelationalEventId.CommandError.Name) {
                OnCommandError(value.Value as CommandErrorEventData);
            }
        }


        /// <summary>
        /// Handle successfull db command
        /// </summary>
        /// <param name="payload">event payload</param>
        public void OnCommandFinished(CommandExecutedEventData payload) {
            var operation = payload.Command.CommandText.Split().First().ToUpper();
            var parameters = payload.Command.Parameters.OfType<DbParameter>().Select(param => new {
                value = param.Value,
                name = param.ParameterName,
                type = Enum.GetName(typeof(DbType), param.DbType),
            }).ToList();

            // do not report unnecessary events
            if (operation == "PRAGMA")
                return;

            using (var scope = GlobalTracer.Instance
                .BuildSpan(operation)
                .WithStartTimestamp(payload.StartTime.UtcDateTime)
                .StartActive(finishSpanOnDispose: true)) {
                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("event.origin", "system-data");
                scope.Span.SetTag("resource.name", payload.Command.Connection.Database);
                scope.Span.SetTag("resource.type", "database");
                scope.Span.SetTag("resource.operation", operation);
                scope.Span.SetTag("sql.driver", payload.Command.Connection.GetType().FullName);
                scope.Span.SetTag("sql.statement", payload.Command.CommandText);
                scope.Span.SetTag("sql.table_name", TableNameExtractor.ExtractTableName(payload.Command.CommandText));
                scope.Span.SetIgnoredKeysIfNeeded("sql.connection_string", payload.Command.Connection.ConnectionString);
                scope.Span.SetDataIfNeeded("sql.parameters", parameters);

                if (payload.Result is DbDataReader) {
                    var result = payload.Result as DbDataReader;
                    scope.Span.SetTag("sql.cursor_row_count", result.RecordsAffected);
                }
            }
        }

        public void OnCommandError(CommandErrorEventData payload) {
            var operation = payload.Command.CommandText.Split().First().ToUpper();
            var parameters = payload.Command.Parameters.OfType<DbParameter>().Select(param => new {
                value = param.Value,
                name = param.ParameterName,
                type = Enum.GetName(typeof(DbType), param.DbType),
            }).ToList();

            // do not report unnecessary events
            if (operation == "PRAGMA")
                return;

            using (var scope = GlobalTracer.Instance
                .BuildSpan(operation)
                .WithStartTimestamp(payload.StartTime.UtcDateTime)
                .StartActive(finishSpanOnDispose: true)) {
                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("event.origin", "system-data");
                scope.Span.SetTag("resource.name", payload.Command.Connection.Database);
                scope.Span.SetTag("resource.type", "database");
                scope.Span.SetTag("resource.operation", operation);
                scope.Span.SetTag("sql.driver", payload.Command.Connection.GetType().FullName);
                scope.Span.SetTag("sql.statement", payload.Command.CommandText);
                scope.Span.SetTag("sql.table_name", TableNameExtractor.ExtractTableName(payload.Command.CommandText));
                scope.Span.SetIgnoredKeysIfNeeded("sql.connection_string", payload.Command.Connection.ConnectionString);
                scope.Span.SetDataIfNeeded("sql.parameters", parameters);

                scope.Span.AddException(payload.Exception);
            }
        }
    }
}
