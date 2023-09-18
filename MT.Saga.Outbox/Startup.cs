using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using LetsGetChecked.Bus.Kafka;
using LetsGetChecked.Bus.Kafka.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MT.Contracts.Commands.Order;
using MT.Contracts.Commands.Product;
using MT.Contracts.Events.Order;
using MT.Saga.Outbox.Domain;
using MT.Saga.Outbox.Domain.Persistence;
using MT.Saga.Outbox.Filters;
using MT.Saga.Outbox.Serialization;
using ClientConfig = Amazon.Runtime.ClientConfig;
using RegionEndpoint = Amazon.RegionEndpoint;

namespace MT.Saga.Outbox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(c =>
                {
                    var schemaRegistry = new SchemaRegistryConfig
                    {
                        Url = "localhost:8081"
                    };

                    return new CachedSchemaRegistryClient(schemaRegistry);
                })
                .AddDbContext<OrderDbContext>(builder =>
                {
                    builder.UseSqlServer(Configuration.GetConnectionString("SqlServerDbUs")!);
                })
                .AddMassTransit(busConfig =>
                {
                    busConfig.AddEntityFrameworkOutbox<OrderDbContext>(o =>
                    {
                        o.UseSqlServer();

                        o.UseBusOutbox();
                    });

                    busConfig.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateDefinition>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                            r.ExistingDbContext<OrderDbContext>();
                            r.UseSqlServer();
                        });
                    
                    busConfig.UsingAmazonSqs((context, amazonSqsConfig) =>
                    {
                        amazonSqsConfig.Host("eu-west-1", h =>
                        {
                            var regionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1");

                            h.Config(CreateAwsClientConfig<AmazonSimpleNotificationServiceConfig>(regionEndpoint));
                            h.Config(CreateAwsClientConfig<AmazonSQSConfig>(regionEndpoint));
                            h.AccessKey("test");
                            h.SecretKey("test");
                        });

                        EndpointConvention.Map<SellProduct>(new Uri("amazonsqs://eu-west-1/product-commands",
                            UriKind.Absolute));
                        EndpointConvention.Map<CompleteOrder>(new Uri("amazonsqs://eu-west-1/order-commands",
                            UriKind.Absolute));

                        var sqsQueueName = context.GetService<IConfiguration>()?.GetSection("Saga")
                            .GetValue<string>("SqsQueueName");

                        if (sqsQueueName == null)
                        {
                            throw new InvalidOperationException("SqsQueueName must be configured under Saga");
                        }

                        amazonSqsConfig.ReceiveEndpoint(sqsQueueName, e =>
                        {
                            e.Subscribe("product-events");

                            e.ConfigureSaga<OrderState>(context);
                        });
                    });

                    busConfig.AddRider(riderConfig =>
                    {
                        riderConfig.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateDefinition>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                                r.ExistingDbContext<OrderDbContext>();
                                r.UseSqlServer();
                            });

                        riderConfig.UsingKafka((riderContext, kafkaConfig) =>
                        {
                            kafkaConfig.Host(new List<string>() { "localhost:19092" });

                            var cachedSchemaRegistryClient = riderContext.GetService<CachedSchemaRegistryClient>();

                            if (cachedSchemaRegistryClient == null)
                            {
                                throw new InvalidOperationException("The schema registry client is not registered");
                            }
                            
                            kafkaConfig.TopicEndpoint<OrderCreated>(
                                nameof(OrderCreated),
                                "saga-orders",
                                topicConfig =>
                                {
                                    topicConfig.CreateIfMissing();
                                    topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                                    topicConfig.SetValueDeserializer(new AvroValueDeserializer<OrderCreated>(cachedSchemaRegistryClient));
                                    topicConfig.UseFilter(new MessageIdConsumeContextFilter());
                                    topicConfig.ConfigureSaga<OrderState>(riderContext);
                                });

                            kafkaConfig.TopicEndpoint<OrderCompleted>(
                                nameof(OrderCompleted),
                                "saga-orders",
                                topicConfig =>
                                {
                                    topicConfig.CreateIfMissing();
                                    topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                                    topicConfig.SetValueDeserializer(new AvroValueDeserializer<OrderCompleted>(cachedSchemaRegistryClient));
                                    topicConfig.UseFilter(new MessageIdConsumeContextFilter());
                                    topicConfig.ConfigureSaga<OrderState>(riderContext);
                                });
                        });
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure()
        {
        }


        public static T CreateAwsClientConfig<T>(RegionEndpoint endpoint)
            where T : ClientConfig, new()
        {
            if (endpoint == default)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            var config = new T { RegionEndpoint = endpoint };

            var uriBuilder = new UriBuilder("http://localhost:4566");

            config.ServiceURL = uriBuilder.ToString().TrimEnd('/');
            config.AuthenticationRegion = endpoint.SystemName;

            return config;
        }
    }
}
