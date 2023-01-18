namespace MT.Contracts.Commands.Product
{
    public class SellProduct
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
