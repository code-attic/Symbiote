using Newtonsoft.Json;

namespace Core.Tests.Serialization.Extensions
{
    public class MarkedReadonlyProperties
    {
        [JsonIgnore]
        public string ReadOnly { get { return "test"; } }
    }
}