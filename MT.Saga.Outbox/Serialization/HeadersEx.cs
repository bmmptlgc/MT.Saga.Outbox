using System.Text;
using Confluent.Kafka;

namespace MT.Saga.Outbox.Serialization
{
    internal static class HeadersEx
    {
        public static string Get(this Headers headers, string key)
            => headers.GetLastBytes(key).ToHeaderString();

        public static bool TryGet(this Headers headers, string key, out string value)
        {
            var res = headers.TryGetLastBytes(key, out var bytes);
            value = res ? bytes.ToHeaderString() : default;
            return res;
        }
        
        private static string ToHeaderString(this byte[] bytes)
            => bytes == default ? default : Encoding.UTF8.GetString(bytes);
    }
}