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

        public Event<OrderExpired> OrderExpired { get; private set; }
        public Event<ProductFailedToSell> ProductFailedToSell{ get; private set; }

        public State Created { get; private set; }
        public State Sold { get; private set; }

        public OrderStateMachine(ILogger<OrderStateMachine> logger) 
        {
            Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => ProductSold, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderCompleted, x => x.CorrelateById(m => m.Message.OrderId));

            Event(() => OrderExpired, x => x
                .CorrelateById(m => m.Message.OrderId));
            
            Event(() => ProductFailedToSell, x => x
                .CorrelateById(m => m.Message.OrderId));

            Schedule(
                () => OrderExpirationSchedule,
                x => x.OrderExpirationToken,
                x => x.Delay = TimeSpan.FromSeconds(5));
            
            Schedule(
                () => ProductSaleExpirationSchedule,
                x => x.ProductSaleExpirationToken,
                x => x.Delay = TimeSpan.FromSeconds(5));
            
            InstanceState(x => x.CurrentState, Created, Sold);
            
            Initially(
                When(OrderCreated)
                    .Then(context =>
                    {
                        logger.LogInformation("Created: {0}", context.Saga.CorrelationId);
                    })
                    .SendAsync(context =>
                    {
                        logger.LogInformation("Send SellProduct command: {0}", context.Saga.CorrelationId);
                        return context.Init<SellProduct>(new
                        {
                            OrderId = context.Data.OrderId,
                            ProductId = context.Data.ProductId,
                            Quantity = context.Data.Quantity
                        });
                    })
                    .Schedule(
                        ProductSaleExpirationSchedule,
                        context =>
                        {
                            logger.LogInformation("Product sale scheduler set for 5 seconds: {0}", context.Saga.CorrelationId);
                            return context.Init<ProductFailedToSell>(new { context.Data.OrderId });
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
                    //.Schedule(
                    //    OrderExpirationSchedule,
                    //    context =>
                    //    {
                    //        logger.LogInformation("Order expired Scheduler set for 5 seconds: {0}", context.Saga.CorrelationId);
                    //        return context.Init<OrderExpired>(new { context.Data.OrderId });
                    //    })
                    .TransitionTo(Sold),
                When(ProductFailedToSell)
                    .Then(context =>
                    {
                        logger.LogInformation("Product failed to sell: {0}", context.Saga.CorrelationId);
                    })
                    .Finalize()
                );

            During(Sold,
                When(OrderCompleted)
                    .Then(context =>
                    {
                        logger.LogInformation("Completed: {0}", context.Saga.CorrelationId);
                    })
                    .Finalize(),
                When(OrderExpired)
                    .Then(context =>
                    {
                        logger.LogInformation("Expired: {0}", context.Saga.CorrelationId);
                    })
                    .Finalize(),
                When(ProductFailedToSell)
                    .Then(context =>
                    {
                        logger.LogInformation("Product failed to sell: {0}", context.Saga.CorrelationId);
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
        
        public Schedule<OrderState, OrderExpired> OrderExpirationSchedule { get; set; }
        public Schedule<OrderState, ProductFailedToSell> ProductSaleExpirationSchedule { get; set; }
    }

    public class ProductFailedToSell
    {
        public Guid OrderId { get; set; }
    }

    public class OrderExpired
    {
        public Guid OrderId { get; set; }
    }
}