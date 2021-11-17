using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

using Epsagon.Dotnet.Core;

namespace Epsagon.Dotnet.Instrumentation.HttpClients {
    /// <summary>
    /// Listen for new diagnostic listeners
    /// </summary>
    public class HttpDiagnosticsListener : IObserver<DiagnosticListener> {
        private static string DiagnosticSourceName = "HttpHandlerDiagnosticListener";

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        /// <summary>
        /// Attach a new DbDiagnosticTracer to the newly created DiagnosticListener
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(DiagnosticListener value) {
            if (value.Name == DiagnosticSourceName) {
                value.Subscribe(new HttpClientTracer());
            }
        }
    }
}
