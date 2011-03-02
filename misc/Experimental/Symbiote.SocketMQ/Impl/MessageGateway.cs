/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                    var socketMessage = message.Item2.FromJson<ServerSocketMessage>();
                    var body = DecodeMessageContent(message.Item2);
                    _bus.Send(socketMessage.To, body, socketMessage.RoutingKey);
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

        protected object DecodeMessageContent(string json)
        {
            JToken jsonDoc;
            using (var reader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(reader))
            {
                jsonDoc = JToken.ReadFrom(jsonReader);
            }
            return jsonDoc["Body"].ToString().FromJson();
        }

        private void HandleClientDisconnect(string clientId)
        {
            _bus.Unsubscribe(clientId);
        }

        private void HandleClientConnect(string clientId)
        {
            _bus.AddEndPoint(x => x.QueueName(clientId));
            _bus.Subscribe(clientId);
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