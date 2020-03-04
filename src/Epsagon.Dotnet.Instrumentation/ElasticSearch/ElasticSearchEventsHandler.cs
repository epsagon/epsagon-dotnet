using System;
using System.Collections.Generic;
using System.Text;
using Elasticsearch.Net;
using Epsagon.Dotnet.Core;
using OpenTracing.Tag;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.ElasticSearch
{
    public static class ElasticSearchEventsHandler
    {
        public static void HandleRequestCompleted(IApiCallDetails details)
        {
            using (var scope = GlobalTracer.Instance.BuildSpan(details.HttpMethod.ToString()).StartActive(finishSpanOnDispose: true))
            {
                var uri = details.Uri;
                var requestBody = details.RequestBodyInBytes ?? new byte[] { };
                var responseBody = details.ResponseBodyInBytes ?? new byte[] { };

                scope.Span.SetTag("event.id", Guid.NewGuid().ToString());
                scope.Span.SetTag("resource.name", $"{uri.Scheme}://{uri.Host}:{uri.Port}");
                scope.Span.SetTag("resource.operation", details.HttpMethod.ToString());
                scope.Span.SetTag("resource.type", "ElasticSearch");
                scope.Span.SetTag("elastic.request_uri", details.Uri.ToString());

                if (!details.Success)
                {
                    Tags.Error.Set(scope.Span, !details.Success);
                    scope.Span.Log(new Dictionary<string, object> {
                        { OpenTracing.LogFields.ErrorObject, details.OriginalException },
                        { OpenTracing.LogFields.ErrorKind, details.OriginalException.GetType().Name },
                        { OpenTracing.LogFields.Stack, details.OriginalException.StackTrace },
                        { OpenTracing.LogFields.Message, details.OriginalException.Message }
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
