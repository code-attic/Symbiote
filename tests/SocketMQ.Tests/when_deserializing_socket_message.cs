using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace SocketMQ.Tests
{
    [Subject("deserializing socket message")]
    public class when_deserializing_socket_message : with_json_from_newtonsoft
    {
        protected static TestMessage message;
        protected static ElaborateMessageType body;

        private Because of = () =>
                                 {
                                     message = json.FromJson<TestMessage>();
                                     body = message.Body as ElaborateMessageType;
                                 };
        
        private It should_have_to_value = () => 
                                          message.To.ShouldEqual("Exchange");
        private It should_have_correct_message_type =
            () => message.Body.ShouldBeOfType(typeof (ElaborateMessageType));
        private It should_have_content = () => body.Content.ShouldEqual("Test");
        private It should_have_sender = () => body.Sender.ShouldEqual("test");
        private It should_be_invalid = () => body.Valid.ShouldBeFalse();
    }
}