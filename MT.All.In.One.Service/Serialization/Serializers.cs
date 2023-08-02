using Avro.Specific;

namespace MT.All.In.One.Service.Serialization
{
    public interface IValueSerializer
    {
        Task<byte[]> SerializeAsync<T>(T message) where T : ISpecificRecord;
    }

    public interface IValueDeserializer
    {
        Task<object> DeserializeAsync(byte[] data, Type type);
    }
}