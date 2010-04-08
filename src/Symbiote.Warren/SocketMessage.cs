using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Symbiote.Warren
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class SocketMessage
    {
        public object Body { get; set; }
        [JsonIgnore]
        public string MessageType { get { return Body.GetType().Name; } }

        public string To { get; set; }
        public string From { get; set; }
        public string RoutingKey { get; set; }

        public SocketMessage()
        {
        }

        public SocketMessage(object body)
        {
            Body = body;
            To = "";
            From = "";
            RoutingKey = "";
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ClientSocketMessage : SocketMessage
    {
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ServerSocketMessage : SocketMessage
    {
    }
}
