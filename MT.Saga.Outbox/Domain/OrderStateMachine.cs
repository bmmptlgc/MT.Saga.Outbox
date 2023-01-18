using MassTransit;
using MT.Contracts.Commands.Order;
using MT.Contracts.Commands.Product;
using MT.Contracts.Events.Order;
using MT.Contracts.Events.Product;

namespace MT.Saga.Outbox.Domain
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public Event<OrderCreated> OrderCreated { get; private set; }
        public Event<ProductSold> ProductSold { get; private set; }
        public Event<OrderCompleted> OrderCompleted { get; private set; }

        public State Created { get; private set; }
        public State Sold { get; private set; }

        public OrderStateMachine(ILogger<OrderStateMachine> logger) 
        {
            Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => ProductSold, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderCompleted, x => x.CorrelateById(m => m.Message.OrderId));

            InstanceState(x => x.CurrentState, Created, Sold);
            
            Initially(
                When(OrderCreated)
                    .Then(context =>
                    {
                        logger.LogInformation("Created: {0}", context.Saga.CorrelationId);
                    })
                    .SendAsync(context =>
                    {
                        return context.Init<SellProduct>(new
                        {
                            OrderId = context.Data.OrderId,
                            ProductId = context.Data.ProductId,
                            Quantity = context.Data.Quantity
                        });
                    })
                    .TransitionTo(Created)
                );

            During(Created,
                When(ProductSold)
                    .Then(context =>
                    {
                        logger.LogInformation("Product sold: {0}", context.Saga.CorrelationId);
                    })
                    .SendAsync(context =>
                    {
                        return context.Init<CompleteOrder>(new
                        {
                            OrderId = context.Data.OrderId
                        });
                    })
                    .TransitionTo(Sold)
                );


            During(Sold,
                When(OrderCompleted)
                    .Then(context =>
                    {
                        logger.LogInformation("Completed: {0}", context.Saga.CorrelationId);
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}