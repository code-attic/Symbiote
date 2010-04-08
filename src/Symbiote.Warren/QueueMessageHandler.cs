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
                var socketMessage = message as SocketMessage;
                if (socketMessage == null)
                    socketMessage = new ServerSocketMessage()
                                        {
                                            Body = message,
                                            From = response.FromQueue,
                                            To = "",
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