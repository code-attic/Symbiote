/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
