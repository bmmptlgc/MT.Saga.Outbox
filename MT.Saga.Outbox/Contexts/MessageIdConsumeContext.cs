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
            if (context.Headers.TryGetHeader("Lgc-MessageId", out var headerMessageId))
            {
                if (Guid.TryParse(headerMessageId.ToString(), out var messageId))
                {
                    MessageId = messageId;
                }
            }
            
            if (MessageId == null)
            {
                throw new InvalidOperationException("Lgc-MessageId header is required and must be a valid guid");
            }
        }
    }
}
