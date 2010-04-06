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
                var socketMessage = new SocketMessage(message);
                var body = socketMessage.ToJson(false);
                _server.Send(body, response.ExchangeName, response.FromQueue);
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