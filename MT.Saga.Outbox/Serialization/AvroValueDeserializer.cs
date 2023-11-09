using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using MT.Contracts.Events;
using MT.Saga.Outbox.Serialization.kafka;

namespace MT.Saga.Outbox.Serialization
{
    public class AvroValueDeserializer<T> : IDeserializer<T>
        where T : class, IVersionedIntegrationEvent
    {
        private readonly AvroValueDeserializer _avroValueDeserializer;

        public AvroValueDeserializer(ISchemaRegistryClient schemaRegistry)
        {
            _avroValueDeserializer = new AvroValueDeserializer(schemaRegistry);
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (context.Headers == null)
            {
                throw new ArgumentNullException(nameof(context.Headers));
            }

            context.Headers.TryGet("Proprietary-MessageType", out var typeName);

            var messageType = Assembly.Load(typeof(T).Assembly.GetName())
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName.Split('.').Last());

            if (messageType == null)
            {
                throw new TypeLoadException(
                    $"Type {typeName} was not found in {typeof(T).Assembly.GetName()}. Check if definition of event exists or regenerate event classes using AvroGen tool.");
            }

            return (T)_avroValueDeserializer.DeserializeAsync(data.ToArray(), messageType).Result;
        }
    }
}