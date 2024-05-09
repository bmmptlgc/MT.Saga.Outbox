using MassTransit;
using MassTransit.Middleware;
using MT.Contracts.Events.Order;
using MT.Contracts.Events.Product;
using MT.Saga.Outbox.Domain.Persistence;

namespace MT.Saga.Outbox.Domain
{
    public class OrderStateDefinition : SagaDefinition<OrderState>
    {
        readonly IServiceProvider _provider;
        readonly IPartitioner _partition;

        public OrderStateDefinition(IServiceProvider provider)
        {
            _provider = provider;
            _partition = new Partitioner(64, new Murmur3UnsafeHashGenerator());
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
        {
            sagaConfigurator.Message<OrderCreated>(x => x.UsePartitioner(_partition, m => m.Message.OrderId));
            sagaConfigurator.Message<ProductSold>(x => x.UsePartitioner(_partition, m => m.Message.OrderId));
            sagaConfigurator.Message<OrderCompleted>(x => x.UsePartitioner(_partition, m => m.Message.OrderId));
            sagaConfigurator.Message<ProductFailedToSell>(x => x.UsePartitioner(_partition, m => m.Message.OrderId));

            endpointConfigurator.UseMessageRetry(r =>
            {
                r.Ignore<InvalidOperationException>(e => e.Message.Equals("Proprietary-MessageId header is required and must be a valid guid"));
                
                r.Intervals(20, 50, 100, 1000, 5000);
            });

            endpointConfigurator.UseEntityFrameworkOutbox<OrderDbContext>(_provider);
        }
    }
}
