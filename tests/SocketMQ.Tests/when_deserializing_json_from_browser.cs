using System.IO;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Symbiote.Core.Extensions;
using Symbiote.SocketMQ;

namespace SocketMQ.Tests
{
    [Subject("deserializing socket message")]
    public class when_deserializing_json_from_browser : with_json_from_browser
    {
        protected static JsonSocketMessage message;
        protected static ElaborateMessageType body;

        private Because of = () =>
                                 {
                                     message = json.FromJson<JsonSocketMessage>();
                                     JToken jsonDoc;
                                     using(var reader = new StringReader(json))
                                     using(var jsonReader = new JsonTextReader(reader))
                                     {
                                         jsonDoc = JToken.ReadFrom(jsonReader);
                                     }
                                     body = jsonDoc["Body"].ToString().FromJson() as ElaborateMessageType;
                                 };

        private It should_have_to_value = () =>
                                          message.To.ShouldEqual("Exchange");
        private It should_have_content = () => body.Content.ShouldEqual("Test");
        private It should_have_sender = () => body.Sender.ShouldEqual("test");
        private It should_be_invalid = () => body.Valid.ShouldBeFalse();
    }
}