using System;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;

namespace Symbiote.SocketMQ
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class JsonSocketMessage
    {
        public object Body { get; set; }

        [JsonIgnore]
        public string MessageType { get { return Body.GetType().Name; } }

        public string To { get; set; }
        public string From { get; set; }
        public string RoutingKey { get; set; }

        public JsonSocketMessage()
        {
        }

        public JsonSocketMessage(object body)
        {
            Body = body;
            To = "";
            From = "";
            RoutingKey = "";
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class ServerSocketMessage
    {
        [JsonIgnore]
        protected string jsonBody;
        [JsonIgnore]
        protected object body;

        [JsonProperty]
        protected string JsonBody
        {
            get
            {
                jsonBody = jsonBody ?? body.ToJson();
                return jsonBody;
            }
            set
            {
                jsonBody = value;
                body = jsonBody.FromJson();
            }
        }

        [JsonIgnore]
        public object Body
        {
            get { return body; }
            set
            {
                JsonBody = value.ToJson();
            }
        }

        [JsonIgnore]
        public string MessageType { get { return Body.GetType().Name; } }

        public string To { get; set; }
        public string From { get; set; }
        public string RoutingKey { get; set; }

        public ServerSocketMessage()
        {
        }

        public ServerSocketMessage(object body)
        {
            JsonBody = body.ToJson();
            To = "";
            From = "";
            RoutingKey = "";
        }
    }
}
