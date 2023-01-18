using MassTransit;
using MT.Contracts.Commands.Product;
using MT.Contracts.Events.Product;

namespace MT.All.In.One.Service.Consumers;

public class SellProductConsumer : IConsumer<SellProduct>
{
    public async Task Consume(ConsumeContext<SellProduct> context)
    {
        await context.Publish<ProductSold>(new
        {
            __MyMessageId = Guid.NewGuid(),
            ProductId = context.Message.ProductId,
            Quantity = context.Message.Quantity,
            OrderId = context.Message.OrderId
        });
    }
}