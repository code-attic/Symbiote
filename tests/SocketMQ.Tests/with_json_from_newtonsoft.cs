using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace SocketMQ.Tests
{
    public abstract class with_json_from_newtonsoft : with_message
    {
        protected static string json;
        private Establish context = () =>
                                        {
                                            json = sourceMessage.ToJson(false);
                                        };
    }
}