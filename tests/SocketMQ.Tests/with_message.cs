using Machine.Specifications;

namespace SocketMQ.Tests
{
    public abstract class with_message
    {
        protected static TestMessage sourceMessage;

        private Establish context = () => sourceMessage = new TestMessage()
                                                              {
                                                                  To = "Exchange",
                                                                  Body = new ElaborateMessageType() { Content = "Test", Valid = false, Sender = "test"}
                                                              };
    }
}