using MassTransit.Transports;
using MassTransit;

namespace MT.Saga.Outbox.Filters
{
    public class KafkaDeadLetterTransportFilter :
        IFilter<ReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("dead-letter");
        }

        async Task IFilter<ReceiveContext>.Send(ReceiveContext context, IPipe<ReceiveContext> next)
        {
            await next.Send(context).ConfigureAwait(false);
        }
    }
}
