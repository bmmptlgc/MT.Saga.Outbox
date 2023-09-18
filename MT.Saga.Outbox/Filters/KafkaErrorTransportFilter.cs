using MassTransit.Transports;
using MassTransit;

namespace MT.Saga.Outbox.Filters
{
    public class KafkaErrorTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("moveFault");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            await next.Send(context).ConfigureAwait(false);
        }
    }
}
