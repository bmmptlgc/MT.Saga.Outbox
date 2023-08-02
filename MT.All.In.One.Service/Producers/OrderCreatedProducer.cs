using MassTransit;
using MT.Contracts.Events.Order;

namespace MT.All.In.One.Service.Producers
{
    public class OrderCreatedProducer : BackgroundService
    {
        private readonly ILogger<OrderCreatedProducer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OrderCreatedProducer(ILogger<OrderCreatedProducer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var producer = scope.ServiceProvider.GetService<ITopicProducer<Guid, OrderCreated>>();
                if (producer == null) throw new OperationCanceledException();
                await Produce(producer, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopping");
            }
        }

        private static async Task Produce(ITopicProducer<Guid, OrderCreated> producer, CancellationToken stoppingToken)
        {
            async Task ProduceMessage(Guid key, object value) => 
                await producer.Produce(
                    key,
                    value,
                    Pipe.Execute<SendContext>(context =>
                    {
                        context.Headers.Set("Lgc-MessageType", typeof(OrderCreated).FullName);
                    }),
                    stoppingToken);

            await ProduceMessage(
                Guid.NewGuid(),
                new {
                    Source = "MT.Outbox",
                    SourceId = "MT.Outbox",
                    OrderId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 3
                });
        }
    }
}
