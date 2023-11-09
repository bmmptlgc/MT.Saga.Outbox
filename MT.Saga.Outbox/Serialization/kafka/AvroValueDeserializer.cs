using System.Collections.Concurrent;
using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace MT.Saga.Outbox.Serialization.kafka
{
    public interface IValueDeserializer
    {
        Task<object> DeserializeAsync(byte[] data, Type type);
    }
    
    public class AvroValueDeserializer : IValueDeserializer
    {
        private readonly ConcurrentDictionary<Type, object> _deserializerCache = new();
        private readonly ISchemaRegistryClient _schemaRegistry;

        public AvroValueDeserializer(ISchemaRegistryClient schemaRegistry)
        {
            _schemaRegistry = schemaRegistry;
        }       
        public Task<object> DeserializeAsync(byte[] data, Type type)
        {
            var deserializer = _deserializerCache.GetOrAdd(type, t =>
                Activator
                    .CreateInstance(
                        typeof(AvroDeserializer<>).MakeGenericType(t),
                        _schemaRegistry,
                        new AvroDeserializerConfig()));

            return DeserializeAsync(deserializer, data);
        }

        private static async Task<object> DeserializeAsync(object deserializer, byte[] data)
        {
            var task = (Task)deserializer.GetType().InvokeMember(
                nameof(AvroDeserializer<object>.DeserializeAsync),
                BindingFlags.InvokeMethod,
                null,
                deserializer,
                new object[] {new ReadOnlyMemory<byte>(data), data == null, SerializationContext.Empty});

            await task;
            return task.GetType().GetProperty("Result")?.GetValue(task);
        }
    }
}