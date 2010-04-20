using System;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;

namespace SocketMQ.Tests
{
    [Serializable]
    public class TestMessage
    {
        [JsonIgnore] protected string jsonBody;
        [JsonIgnore] protected object body;

        public string To { get; set; }

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
    }
}