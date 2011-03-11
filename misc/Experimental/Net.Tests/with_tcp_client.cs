using System.Net.Sockets;
using Machine.Specifications;

namespace Net.Tests
{
    public abstract class with_tcp_client : with_tcp_listener
    {
        protected static TcpClient client;

        private Establish context = () =>
                                        {
                                            client = new TcpClient();
                                            client.Connect("localhost", 8001);
                                        };
    }
}