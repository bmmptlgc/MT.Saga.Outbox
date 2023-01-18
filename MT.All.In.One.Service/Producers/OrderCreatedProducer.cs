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
                var producer = scope.ServiceProvider.GetService<ITopicProducer<OrderCreated>>();
                if (producer == null) throw new OperationCanceledException();
                await Produce(producer, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopping");
            }
        }

        private static async Task Produce(ITopicProducer<OrderCreated> producer, CancellationToken stoppingToken)
        {
            async Task ProduceMessage<TOrderCreated>(object value) => 
                await producer.Produce(value, stoppingToken);

            await ProduceMessage<OrderCreated>(new
            {
                __MyMessageId = Guid.NewGuid(),
                OrderId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 3
            });
        }
    }
}
