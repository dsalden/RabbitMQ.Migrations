using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace RabbitMQ.Migrations.Helpers
{
    internal static class JsonConvertHelper
    {
        internal static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto
        };

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static byte[] SerializeObjectToByteArray(object value)
        {
            return Encoding.UTF8.GetBytes(SerializeObject(value));
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSerializerSettings);
        }

        public static T DeserializeObject<T>(byte[] value)
        {
            return DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }
    }
}
