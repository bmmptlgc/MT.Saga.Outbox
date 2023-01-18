namespace MT.Contracts.Events.Order
{
    public abstract class OrderIntegrationEventBase
    {
        public Guid OrderId { get; set; }
    }
}