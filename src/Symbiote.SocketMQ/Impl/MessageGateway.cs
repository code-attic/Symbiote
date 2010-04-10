using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;
using Symbiote.WebSocket;

namespace Symbiote.SocketMQ.Impl
{
    public class MessageGateway :
        IDisposable, IMessageGateway
    {
        protected IBus _bus;
        protected ISocketServer _server;

        public void Start()
        {
            ConfigureSocketServer();
            _server.Start();
        }

        private void ConfigureSocketServer()
        {
            _server.ClientConnected += HandleClientConnect;
            _server.ClientDisconnected += HandleClientDisconnect;
            _server.AddMessageHandle(ProcessMessage);
        }

        private void ProcessMessage(Tuple<string, string> message)
        {
            var subscribe = message.Item2.FromJson<Subscribe>();
            if(subscribe.Exchange != null)
            {
                ProcessSubscriptionRequest(message, subscribe);
            }
            else
            {
                try
                {
                    var socketMessage = message.Item2.FromJson<SocketMessage>();
                    _bus.Send(socketMessage.To, socketMessage.Body, socketMessage.RoutingKey);
                }
                catch (Exception e)
                {
                    var exceptionMessage =
                        "An exception occurred trying to send message '{0}' to the bus \r\n\t {1}";

                    exceptionMessage.ToError<IMessageGateway>(message.Item2, e);
                }
            }

        }

        protected void ProcessSubscriptionRequest(Tuple<string, string> message, Subscribe subscribe)
        {
            try
            {
                _bus.BindQueue(message.Item1, subscribe.Exchange, subscribe.RoutingKeys);
            }
            catch (Exception e)
            {
                var exceptionMessage =
                    "An exception occurred when trying to bind queue {0} to exchange {1} using routing keys {2}";

                exceptionMessage.ToError<IMessageGateway>(message.Item1, subscribe.Exchange, subscribe.RoutingKeys);

                var exception =
                    new SocketMQException(
                        exceptionMessage.AsFormat(message.Item1, subscribe.Exchange, subscribe.RoutingKeys), e);
                
                throw exception;
            }
        }

        private void HandleClientDisconnect(string clientId)
        {
            _bus.Unsubscribe(clientId);
        }

        private void HandleClientConnect(string clientId)
        {
            _bus.AddEndPoint(x => x.QueueName(clientId));
            _bus.Subscribe(clientId, null);
        }

        public void Stop()
        {
            _bus.Dispose();
            _server.Dispose();
        }

        public MessageGateway(IBus bus, ISocketServer server)
        {
            CheckArguments(bus, server);
            _bus = bus;
            _server = server;
        }

        private void CheckArguments(IBus bus, ISocketServer server)
        {
            if (bus == null || server == null)
            {
                var message = "";

                if(bus != null)
                {
                    message =
                        "SocketMQ cannot function without a properly configured web socket server. Did you correctly configure Symbiote.WebSocket?";
                }
                else if(server != null)
                {
                    message =
                        "SocketMQ cannot function without a properly configured Jackalope bus. Did you correctly configure Symbiote.Jackalope?";
                }
                else
                {
                    message =
                        "SocketMQ cannot function without a properly configured Jackalope bus and web socket server. Did you correctly configure Symbiote.Jackalope and Symbiote.WebSocket?";
                }

                message.ToError<IMessageGateway>();
                throw new SocketMQException(message);
            }
        }

        public void Dispose()
        {
            Stop();
            _bus.Dispose();
            _server.Dispose();
        }
    }
}