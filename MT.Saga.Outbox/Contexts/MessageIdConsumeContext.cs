using MassTransit;
using MassTransit.Context;

namespace MT.Saga.Outbox.Contexts
{
    public class MessageIdConsumeContext :
        ConsumeContextProxy
    {
        public sealed override Guid? MessageId { get; }

        public MessageIdConsumeContext(ConsumeContext context) : base(context)
        {
            if (context.Headers.TryGetHeader("Proprietary-MessageId", out var headerMessageId))
            {
                if (Guid.TryParse(headerMessageId.ToString(), out var messageId))
                {
                    MessageId = messageId;
                }
            }

            if (MessageId == null)
            {
                throw new InvalidOperationException("Proprietary-MessageId header is required and must be a valid guid");
            }
        }
    }
}
