using MassTransit;
using MT.Saga.Outbox.Contexts;

namespace MT.Saga.Outbox.Filters
{
    internal class MessageIdConsumeContextFilter : IFilter<ConsumeContext>
    {
        public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(MessageIdConsumeContextFilter));

        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            var messageIdContext = new MessageIdConsumeContext(context);

            return next.Send(messageIdContext);
        }
    }
}
