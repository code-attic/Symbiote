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
                var socketMessage = new JsonSocketMessage()
                                        {
                                            Body = message,
                                            From = messageDelivery.Exchange,
                                            To = messageDelivery.Queue,
                                            RoutingKey = ""
                                        };

                var body = socketMessage.ToJson();
                if (_server.Send(body, socketMessage.From, socketMessage.To))
                    messageDelivery.Acknowledge();
                else
                    "Message could not be delivered to client {0} because they disconnected"
                        .ToError<IMessageGateway>(socketMessage.To);
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