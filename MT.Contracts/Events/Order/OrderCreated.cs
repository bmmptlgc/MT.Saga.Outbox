namespace MT.Contracts.Events.Order
{
    public class OrderCreated : OrderIntegrationEventBase
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}