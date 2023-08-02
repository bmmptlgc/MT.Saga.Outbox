using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MT.Contracts.Commands.Order;
using MT.Contracts.Events.Order;
using MT.Contracts.Events.Product;

namespace MT.All.In.One.Service.Consumers;

public class CompleteOrderConsumer : IConsumer<CompleteOrder>
{
    private readonly ITopicProducer<Guid, OrderCompleted> _orderCompletedProducer;

    public CompleteOrderConsumer(ITopicProducer<Guid, OrderCompleted> orderCompletedProducer)
    {
        _orderCompletedProducer = orderCompletedProducer ?? throw new ArgumentException(nameof(orderCompletedProducer));
    }

    public async Task Consume(ConsumeContext<CompleteOrder> context)
    {
        await _orderCompletedProducer.Produce(Guid.NewGuid(),new
        {
            OrderId = context.Message.OrderId
        });
    }
}