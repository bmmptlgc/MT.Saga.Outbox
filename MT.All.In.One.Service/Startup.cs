using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using MT.All.In.One.Service.Consumers;
using MT.All.In.One.Service.Producers;
using MT.Contracts.Events.Order;
using RegionEndpoint = Amazon.RegionEndpoint;

namespace MT.All.In.One.Service
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
            var schemaRegistry = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistry);
            
            services
            //     .AddSingleton(c =>
            // {
            //     var schemaRegistry = new SchemaRegistryConfig
            //     {
            //         Url = "localhost:8081"
            //     };
            //
            //     return new CachedSchemaRegistryClient(schemaRegistry);
            // })
                .AddMassTransit(x =>
            {
                x.AddConsumer<CompleteOrderConsumer>();

                x.UsingAmazonSqs((context, amazonSqsConfig) =>
                {
                    amazonSqsConfig.Host("eu-west-1", h =>
                    {
                        var regionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1");

                        h.Config(CreateAwsClientConfig<AmazonSimpleNotificationServiceConfig>(regionEndpoint));
                        h.Config(CreateAwsClientConfig<AmazonSQSConfig>(regionEndpoint));

                        h.AccessKey("test");
                        h.SecretKey("test");
                    });

                    amazonSqsConfig.ReceiveEndpoint("product-commands", cfg =>
                    {
                        cfg.Consumer<SellProductConsumer>();
                    });

                    amazonSqsConfig.ReceiveEndpoint("order-commands", cfg =>
                    {
                        cfg.ConfigureConsumer<CompleteOrderConsumer>(context);
                    });
                });

                x.AddRider(riderConfig =>
                {
                    riderConfig.AddProducer<Guid, OrderCreated>(nameof(OrderCreated),
                        (c, pc) =>
                        {
                            pc.SetValueSerializer(new AvroSerializer<OrderCreated>(schemaRegistryClient));
                        });
                    riderConfig.AddProducer<Guid, OrderCompleted>(nameof(OrderCompleted),
                        (c, pc) =>
                        {
                            pc.SetValueSerializer(new AvroSerializer<OrderCompleted>(schemaRegistryClient));
                        });

                    riderConfig.UsingKafka((_, kafkaConfig) =>
                    {
                        kafkaConfig.Host(new List<string>() { "localhost:19092" });
                    });
                });
            });

            services.AddHostedService<OrderCreatedProducer>();
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
