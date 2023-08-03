using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using LetsGetChecked.Bus.Kafka;
using MT.Contracts.Events;

namespace MT.Saga.Outbox.Serialization
{
    public class AvroValueDeserializer<T> : IDeserializer<T>
        where T : class, IVersionedIntegrationEvent
    {
        private readonly AvroValueDeserializer _lgcAvroValueDeserializer;

        public AvroValueDeserializer(ISchemaRegistryClient schemaRegistry)
        {
            _lgcAvroValueDeserializer = new AvroValueDeserializer(schemaRegistry);
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

            context.Headers.TryGet("Lgc-MessageType", out var typeName);

            var messageType = Assembly.Load(typeof(T).Assembly.GetName())
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName.Split('.').Last());

            if (messageType == null)
            {
                throw new TypeLoadException(
                    $"Type {typeName} was not found in {typeof(T).Assembly.GetName()}. Check if definition of event exists or regenerate event classes using AvroGen tool.");
            }

            return (T)_lgcAvroValueDeserializer.DeserializeAsync(data.ToArray(), messageType).Result;
        }
    }
}