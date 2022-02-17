using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

using Epsagon.Dotnet.Core;

using Newtonsoft.Json;

using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.HttpClients {
    /// <summary>
    /// trace http requests using HttpClient
    /// </summary>
    public class HttpClientTracer : IObserver<KeyValuePair<string, object>> {
        private static string ActivityName = "System.Net.Http.HttpRequestOut";

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void OnNext(KeyValuePair<string, object> value) {
            // make sure we get the info when the request finished
            if (Activity.Current?.OperationName != ActivityName || value.Key != "System.Net.Http.HttpRequestOut.Stop")
                return;

            var duration = Activity.Current?.Duration;
            var request = value.Value.ExtractAttribute<HttpRequestMessage>("Request");
            var response = value.Value.ExtractAttribute<HttpResponseMessage>("Response");

            // filter out calls to the trace collector OR calls from amazon sdk
            if (request.RequestUri.ToString().Contains("tc.epsagon.com") || 
            (request.Headers.Contains("amz-sdk-request")
            && request.Headers.UserAgent.ToString().Contains("aws-sdk-dotnet")))
                return;

            using (var scope = GlobalTracer.Instance.BuildSpan(request.Method.ToString())
                    .WithStartTimestamp(Activity.Current.StartTimeUtc)
                    .StartActive(finishSpanOnDispose: true)) {

                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("event.origin", "HttpClient");
                scope.Span.SetTag("resource.type", "http");
                scope.Span.SetTag("resource.name", request.RequestUri.Host);
                scope.Span.SetTag("resource.operation", request.Method.ToString());
                scope.Span.SetTag("http.url", request.RequestUri.ToString());
                scope.Span.SetDataIfNeeded("http.request_headers", request.Headers.ToDictionary());

                var requestBody = this.LoadContent(request.Content);
                scope.Span.SetDataIfNeeded("http.request_body", requestBody);

                // A null response extracted signifies a connection failed to establish.
                if (response is null) {
                    scope.Span.AddException(new HttpRequestException(ActivityName));
                } else {
                    var responseBody = this.LoadContent(response.Content);
                    scope.Span.SetDataIfNeeded("http.response_body", responseBody);
                    scope.Span.SetDataIfNeeded("http.response_headers", response.Headers.ToDictionary());
                    scope.Span.SetTag("http.status_code", ((int) response.StatusCode));

                    if (!response.IsSuccessStatusCode) {
                        scope.Span.SetTag("event.error_code", 2); // exception
                        scope.Span.SetTag("error.message", response.ReasonPhrase);
                    }
                }
            }
        }

        private object LoadContent(HttpContent content) {
            if (content is null)
                return null;

            var contentString = content.ReadAsStringAsync().Result;
            try {
                var obj = JsonConvert.DeserializeObject(contentString);
                return obj;
            } catch { return contentString; }
        }
    }
}
