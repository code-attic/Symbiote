using Newtonsoft.Json;

namespace Symbiote.Warren
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Subscribe
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}