namespace MT.Contracts.Events.Product
{
    public class ProductSold
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
    }
}