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
            string[] operationSplit = null;
            string operation = "";
            string message = "";

            try
            {
                base.Handle(context, scope);

                first = input?.Records?.FirstOrDefault();
                operationSplit = first.EventSubscriptionArn?.Split(':');

                if (operationSplit != null)
                {
                    operation = operationSplit[operationSplit.Length - 2];
                }

                message = first.Sns?.Message;

                scope.Span.SetTag("event.id", first.Sns?.MessageId);
                scope.Span.SetTag("resource.type", "sns");
                scope.Span.SetTag("resource.operation", operation);
                scope.Span.SetTag("aws.sns.Notification Subject", first.Sns?.Subject);
                scope.Span.SetDataIfNeeded("aws.sns.Notification Message", first.Sns?.Message);
            }
            catch (NullReferenceException)
            {
                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug("null reference, locals: {@locals}", new
                    {
                        Context = context,
                        scope = scope,
                        First = first,
                        OperationSplit = operationSplit,
                        Operation = operation,
                        Message = message
                    });
                }
            }
        }
    }
}
