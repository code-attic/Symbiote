using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;
using Symbiote.WebSocket;

namespace Symbiote.SocketMQ
{
    public class QueueMessageHandler : IMessageHandler<object>
    {
        protected ISocketServer _server;

        public void Process(object message, IMessageDelivery messageDelivery)
        {
            try
            {
                var socketMessage = new SocketMessage()
                                        {
                                            Body = message,
                                            From = messageDelivery.Exchange,
                                            To = messageDelivery.Queue,
                                            RoutingKey = ""
                                        };

                var body = socketMessage.ToJson(false);
                _server.Send(body, socketMessage.From, socketMessage.To);
                messageDelivery.Acknowledge();
            }
            catch (Exception e)
            {
                var exceptionMessage = "An exception occurred attempting to send message {0} from exchange {1} to socket {2}";
                var exception = new SocketMQException(
                    exceptionMessage
                        .AsFormat(message.GetType().FullName, messageDelivery.Exchange, messageDelivery.Queue), e
                    );
                exceptionMessage
                    .ToError<IMessageGateway>(message.GetType().FullName, messageDelivery.Exchange, messageDelivery.Queue);
                throw exception;
            }
        }

        public QueueMessageHandler(ISocketServer server)
        {
            if (server == null)
            {
                var exceptionMessage = "A message cannot be processed by SocketMQ without a configured ISocketServer. Did you correctly configure Symbiote.WebSocket?";
                var exception = new SocketMQException(exceptionMessage);
                exceptionMessage
                    .ToError<IMessageGateway>();
                throw exception;
            }
            _server = server;
        }
    }
}