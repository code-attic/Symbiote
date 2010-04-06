using System;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Impl;
using Symbiote.WebSocket;

namespace Symbiote.Warren
{
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
                var proxy = _proxyFactory.GetProxyForExchange(subscribe.Exchange);
                if (proxy != null)
                {
                    try
                    {
                        proxy.Channel.QueueBind(obj.Item1, subscribe.Exchange, subscribe.RoutingKey, false, null);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                } 
            }
            else
            {
                var socketMessage = obj.Item2.FromJson<SocketMessage>();
                _bus.Send(socketMessage.To, socketMessage);
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