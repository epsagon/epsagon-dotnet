using System;
using System.Diagnostics;

using Microsoft.EntityFrameworkCore;

namespace Epsagon.Dotnet.Instrumentation.EFCore {
    /// <summary>
    /// Listen to the creation of new DiagnosticListeners
    /// </summary>
    public class DbDiagnosticsListener : IObserver<DiagnosticListener> {
        public void OnCompleted() { }
        public void OnError(Exception error) { }

        /// <summary>
        /// Attach a new DbDiagnosticTracer to the newly created DiagnosticListener
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(DiagnosticListener value) {
            if (value.Name == DbLoggerCategory.Name) {
                value.Subscribe(new DbDiagnosticTracer());
            }
        }
    }
}
