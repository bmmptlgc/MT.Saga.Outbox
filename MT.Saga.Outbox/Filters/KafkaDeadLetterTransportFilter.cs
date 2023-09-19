using MassTransit.Transports;
using MassTransit;
using MT.Contracts.Events.Order;

namespace MT.Saga.Outbox.Filters
{
    public class KafkaDeadLetterTransportFilter<T> :
        IFilter<ReceiveContext> where T : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("dead-letter");
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            context.TryGetPayload(out ConsumeContext? consumeContext);
            consumeContext?.TryGetMessage(out ConsumeContext<T>? message);

            // TODO: Use a producer to send the message to a dead letter topic or persistent storage

            await next.Send(context).ConfigureAwait(false);
        }
    }
}
