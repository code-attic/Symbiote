using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.WebSocket;

namespace Symbiote.Warren
{
    public class QueueMessageHandler : IMessageHandler<object>
    {
        protected ISocketServer _server;

        public void Process(object message, IResponse response)
        {
            try
            {
                var socketMessage = new SocketMessage()
                                        {
                                            Body = message,
                                            From = response.ExchangeName,
                                            To = response.FromQueue,
                                            RoutingKey = ""
                                        };

                var body = socketMessage.ToJson(false);
                _server.Send(body, socketMessage.From, socketMessage.To);
                response.Acknowledge();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public QueueMessageHandler(ISocketServer server)
        {
            _server = server;
        }
    }
}