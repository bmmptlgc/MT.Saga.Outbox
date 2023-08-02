using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace MT.All.In.One.Service.Serialization
{
    public class AvroValueSerializer : IValueSerializer
    {
        private readonly IAsyncSerializer<ISpecificRecord> _serializer;

        private static readonly AvroSerializerConfig SerializerConfig = new()
        {
            AutoRegisterSchemas = true,
            SubjectNameStrategy = SubjectNameStrategy.Record
        };

        public AvroValueSerializer(ISchemaRegistryClient schemaRegistry)
        {
            _serializer = new AvroSerializer<ISpecificRecord>(schemaRegistry, SerializerConfig);
        }

        public Task<byte[]> SerializeAsync<T>(T message) where T : ISpecificRecord
            => _serializer.SerializeAsync(message, SerializationContext.Empty);
    }
}