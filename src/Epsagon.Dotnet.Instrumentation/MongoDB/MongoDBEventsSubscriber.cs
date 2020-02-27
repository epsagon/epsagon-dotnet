using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Epsagon.Dotnet.Core;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;
using OpenTracing;
using OpenTracing.Util;

namespace Epsagon.Dotnet.Instrumentation.MongoDB
{
    public class MongoDBEventsSubscriber : IEventSubscriber
    {
        private ReflectionEventSubscriber eventSubscriber;

        private Dictionary<int, IScope> scopes;

        public MongoDBEventsSubscriber()
        {
            this.eventSubscriber = new ReflectionEventSubscriber(this);
        }

        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            return this.eventSubscriber.TryGetEventHandler(out handler);
        }

        public void Handle(CommandStartedEvent startedEvent)
        {
            var commandName = startedEvent.CommandName;
            if (commandName != "insert") return;

            var tracer = GlobalTracer.Instance;
            var scope = tracer.BuildSpan($"{commandName}-{startedEvent.RequestId}").StartActive(finishSpanOnDispose: true);
            var documents = startedEvent.Command.GetValue("documents").AsBsonArray;
            var ids = documents.Select(doc => doc.AsBsonDocument.GetValue("_id").ToString());

            scope.Span.SetTag("event.id", $"mongo-{Guid.NewGuid()}");
            scope.Span.SetTag("resource.name", startedEvent.DatabaseNamespace.DatabaseName);
            scope.Span.SetTag("resource.operation", commandName);
            scope.Span.SetTag("database.mongodb.DB URL", GetEndpointUrl(startedEvent.ConnectionId.ServerId.EndPoint));
            scope.Span.SetTag("database.mongodb.DB Name", startedEvent.DatabaseNamespace.DatabaseName);
            scope.Span.SetTag("database.mongodb.Collection Name", startedEvent.Command.GetValue("insert").AsString);
            scope.Span.SetTag("database.mongodb.inserted_ids", Utils.SerializeObject(ids));
            scope.Span.SetDataIfNeeded("database.mongodb.Items", BsonTypeMapper.MapToDotNetValue(documents));

            // store the scope to finish when the command is finished
            this.scopes.Add(startedEvent.RequestId, scope);
        }

        public void Handle(CommandSucceededEvent succeededEvent)
        {
            var commandName = succeededEvent.CommandName;
            if (commandName != "insert" || !this.scopes.ContainsKey(succeededEvent.RequestId)) return;

            // close the scope and finish the span
            this.scopes[succeededEvent.RequestId].Dispose();
            this.scopes.Remove(succeededEvent.RequestId);
        }

        public void Handle(CommandFailedEvent failedEvent)
        {
            var commandName = failedEvent.CommandName;
            if (commandName != "insert" || !this.scopes.ContainsKey(failedEvent.RequestId)) return;

            var scope = this.scopes[failedEvent.RequestId];
            scope.Span.AddException(failedEvent.Failure);
            scope.Dispose();

            this.scopes.Remove(failedEvent.RequestId);
        }

        private string GetEndpointUrl(EndPoint endPoint)
        {
            if (endPoint is IPEndPoint ip) return ip.Address.ToString();
            if (endPoint is DnsEndPoint dns) return $"{dns.Host}:{dns.Port}";
            return endPoint.ToString();
        }
    }
}
