using System;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Epsagon.Dotnet.Core;
using OpenTracing;
using Serilog;
using static Amazon.Lambda.SNSEvents.SNSEvent;

namespace Epsagon.Dotnet.Instrumentation.Triggers
{
    public class SNSLambda : BaseTrigger<SNSEvent>
    {
        public SNSLambda(SNSEvent input) : base(input)
        {
        }

        public override void Handle(ILambdaContext context, IScope scope)
        {
            SNSRecord first = null;
            string[] topicArnSplit = null;
            string topicName = "";
            string message = "";

            try
            {
                base.Handle(context, scope);

                first = input?.Records?.FirstOrDefault();
                topicArnSplit = first.EventSubscriptionArn?.Split(':');

                if (topicArnSplit != null)
                {
                    topicName = topicArnSplit[topicArnSplit.Length - 2];
                }

                message = first.Sns?.Message;

                scope.Span.SetTag("event.id", first.Sns?.MessageId);
                scope.Span.SetTag("resource.type", "sns");
                scope.Span.SetTag("resource.name", topicName);
                scope.Span.SetTag("resource.operation", first.Sns?.Type);
                scope.Span.SetTag("aws.sns.Notification Subject", first.Sns?.Subject);
                scope.Span.SetDataIfNeeded("aws.sns.Notification Message", first.Sns?.Message);
            }
            catch (NullReferenceException)
            {
                Utils.DebugLogIfEnabled("null reference, locals: {@locals}", new
                {
                    Context = context,
                    scope = scope,
                    First = first,
                    TopicArn = topicArnSplit,
                    TopicName = topicName,
                    Message = message
                });
            }
        }
    }
}
