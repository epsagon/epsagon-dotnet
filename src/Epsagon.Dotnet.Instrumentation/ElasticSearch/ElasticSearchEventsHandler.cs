using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elasticsearch.Net;
using Epsagon.Dotnet.Core;
using Jaeger;
using OpenTracing.Tag;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.ElasticSearch
{
    public static class ElasticSearchEventsHandler
    {
        public static void HandleRequestCompleted(IApiCallDetails details)
        {
            using (var scope = GlobalTracer.Instance
                .BuildSpan(details.HttpMethod.ToString())
                .WithStartTimestamp(new DateTimeOffset(details.AuditTrail.First().Started))
                .StartActive(finishSpanOnDispose: true))
            {
                var uri = details.Uri;
                var requestBody = details.RequestBodyInBytes ?? new byte[] { };
                var responseBody = details.ResponseBodyInBytes ?? new byte[] { };

                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("resource.name", $"{uri.Scheme}://{uri.Host}:{uri.Port}");
                scope.Span.SetTag("resource.operation", details.HttpMethod.ToString());
                scope.Span.SetTag("resource.type", "elasticsearch");
                scope.Span.SetTag("elastic.request_uri", details.Uri.ToString());

                if (!details.Success)
                {
                    object error = details?.OriginalException;
                    Tags.Error.Set(scope.Span, !details.Success);
                    scope.Span.Log(new Dictionary<string, object> {
                        { OpenTracing.LogFields.ErrorObject, error != null ? error : details?.ServerError?.Error },
                        { OpenTracing.LogFields.ErrorKind, details?.OriginalException?.GetType()?.Name ?? details?.ServerError?.Error?.Type },
                        { OpenTracing.LogFields.Stack, details?.OriginalException?.StackTrace ?? details?.ServerError?.Error?.Index },
                        { OpenTracing.LogFields.Message, details?.OriginalException?.Message ?? details?.ServerError?.Error?.Reason }
                    });
                }

                Tags.HttpMethod.Set(scope.Span, details.HttpMethod.ToString());
                Tags.HttpUrl.Set(scope.Span, uri.ToString());

                if (details.HttpStatusCode.HasValue)
                    Tags.HttpStatus.Set(scope.Span, details.HttpStatusCode.Value);

                scope.Span.SetDataIfNeeded("elastic.request_body", Encoding.UTF8.GetString(requestBody));
                scope.Span.SetDataIfNeeded("elastic.response_body", Encoding.UTF8.GetString(responseBody));
            }
        }

        public static void HandleRequestDataCreated(RequestData request)
        {
        }
    }
}
