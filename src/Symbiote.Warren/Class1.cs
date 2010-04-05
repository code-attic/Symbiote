using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;
using Symbiote.WebSocket;
using Symbiote.Core.Extensions;

namespace Symbiote.Warren
{
    [JsonObject(MemberSerialization.OptOut)]
    [Serializable]
    public class SocketMessage
    {
        public object Body { get; set; }
        [JsonIgnore]
        public string MessageType { get { return Body.GetType().Name; } }

        public string To { get; set; }

        public SocketMessage()
        {
        }

        public SocketMessage(object body)
        {
            Body = body;
            To = "";
        }
    }

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

    [JsonObject(MemberSerialization.OptOut)]
    public class Subscribe
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }

    public class Bridge :
        IDisposable
    {
        protected IBus _bus;
        protected IChannelProxyFactory _proxyFactory;
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

        private void ProcessMessage(Tuple<string, string> obj)
        {
            var subscribe = obj.Item2.FromJson<Subscribe>();
            if(subscribe.Exchange != null)
            {
                var proxy = _proxyFactory.GetProxyForQueue(obj.Item1);
                if (proxy != null)
                {
                    proxy.Channel.QueueBind(obj.Item1, subscribe.Exchange, subscribe.RoutingKey, true, null);
                } 
            }
            else
            {
                var socketMessage = obj.Item2.FromJson<SocketMessage>();
                _bus.Send(socketMessage.To, socketMessage.Body);
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
            
        }

        public Bridge(IBus bus, IChannelProxyFactory proxyFactory, ISocketServer server)
        {
            _bus = bus;
            _proxyFactory = proxyFactory;
            _server = server;
        }


        public void Dispose()
        {
            Stop();
            _bus.Dispose();
            _server.Dispose();
        }
    }
}
