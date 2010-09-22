using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Symbiote.Redis.Impl.Serialization
{
    public class JsonCacheSerializer
        : ICacheSerializer
    {
        protected JsonSerializerSettings GetDefaultSettings()
        {
            var settings = new JsonSerializerSettings()
                               {
                                   NullValueHandling = NullValueHandling.Ignore,
                                   MissingMemberHandling = MissingMemberHandling.Ignore,
                                   TypeNameHandling = TypeNameHandling.All,
                                   TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                               };
            settings.Converters.Add(new IsoDateTimeConverter());
            return settings;
        }

        public byte[] Serialize<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None, GetDefaultSettings());
            return UTF8Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var json = UTF8Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json, GetDefaultSettings());
        }
    }
}