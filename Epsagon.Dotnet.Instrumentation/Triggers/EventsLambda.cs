using System.Linq;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Epsagon.Dotnet.Core;
using OpenTracing;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class EventsLambda : ITrigger<ScheduledEvent>
    {
        public void Handle(ScheduledEvent input, ILambdaContext context, IScope scope)
        {
            scope.Span.SetTag("event.id", input.Id);
            scope.Span.SetTag("resource.name", input.Resources.First().Split('/').Last());
            scope.Span.SetTag("resource.operation", input.DetailType);
            scope.Span.SetTag("resource.metadata", EpsagonUtils.SerializeObject(new {
                Region = input.Region,
                Detail = input.Detail,
                Account = input.Account
            }));
        }
    }
}
