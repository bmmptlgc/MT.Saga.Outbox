using MassTransit;

namespace MT.Saga.Outbox.Domain
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public byte[] RowVersion { get; set; }
        public Guid? OrderExpirationToken { get; set; }
        public Guid? ProductSaleExpirationToken { get; set; }
    }
}