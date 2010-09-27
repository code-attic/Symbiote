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
using Microsoft.Practices.ServiceLocation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Impl.Channel
{
    public class ChannelProxy : IChannelProxy
    {
        private IModel _channel;
        private IAmqpEndpointConfiguration _configuration;
        private IMessageSerializer _messageSerializer;
        private Action<IModel, BasicReturnEventArgs> _onReturn;
        private string _protocol;
        private bool _closed;

        private const string CONTENT_TYPE = "text/plain";
        private const string AMQP_08 = "AMQP_0_8";

        public IAmqpEndpointConfiguration EndpointConfiguration
        {
            get { return _configuration; }
        }

        public IModel Channel
        {
            get
            {
                if (_channel == null || !_channel.IsOpen)
                {
                    Close();
                }
                return _channel;
            }
        }

        public bool Closed
        {
            get { return _closed; }
        }

        public string QueueName
        {
            get { return _configuration.QueueName ?? ""; }
        }

        public IMessageSerializer Serializer
        {
            get
            {
                _messageSerializer = _messageSerializer ?? ServiceLocator.Current.GetInstance<IMessageSerializer>();
                return _messageSerializer;
            }
        }

        public void Acknowledge(ulong tag, bool multiple)
        {
            Channel.BasicAck(tag, multiple);
        }

        public virtual Envelope Dequeue()
        {
            BasicGetResult result = null;
            try
            {
                result = Channel.BasicGet(QueueName, _configuration.NoAck);
                if (result != null)
                {
                    var message = Serializer.Deserialize(result.Body);
                    SetMessageCorrelation(message, result);
                    return Envelope.Create(message, this, result);
                }
            }
            catch (Exception e)
            {
                if (result != null)
                {
                    "An exception occurred attempting dequeue a message from the exchange named {0} with routing key {1} \r\n\t {2}"
                        .ToError<IBus>(result.Exchange, result.RoutingKey, e);
                    Reject(result.DeliveryTag, true);
                }
                else
                {
                    "An exception occurred attempting to dequeue a message from queue {0}"
                        .ToError<IBus>(QueueName);
                }
            }
            return null;
        }

        public void Send<T>(T body, string routingKey) where T : class
        {
            if (body == default(T))
                return;
            var stream = Serializer.Serialize(body);
            IBasicProperties properties = CreatePublishingProperties(CONTENT_TYPE);
            GetMessageCorrelation(body, properties);
            properties.ReplyToAddress = new PublicationAddress(EndpointConfiguration.ExchangeType.ToString(), EndpointConfiguration.ExchangeName, routingKey);
            Channel.BasicPublish(
                _configuration.ExchangeName, 
                routingKey, 
                EndpointConfiguration.MandatoryDelivery, 
                EndpointConfiguration.ImmediateDelivery,  
                properties, 
                stream);
        }

        public void Reject(ulong tag, bool requeue)
        {
            //This call is currently unimplemented for AMQP 0.8
            if (_protocol != AMQP_08)
                Channel.BasicReject(tag, requeue);
        }

        public void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class
        {
            //This call is currently unimplemented for AMQP 0.8
            if (_protocol != AMQP_08)
            {
                var stream = Serializer.Serialize(response);
                _channel.BasicPublish(address, properties, stream);
            }
        }

        protected IBasicProperties CreatePublishingProperties(string contentType)
        {
            var properties = Channel.CreateBasicProperties();
            properties.DeliveryMode = (byte)(_configuration.PersistentDelivery ? DeliveryMode.Persistent : DeliveryMode.Volatile);
            properties.ContentType = contentType;
            return properties;
        }

        protected void GetMessageCorrelation<T>(T body, IBasicProperties properties)
        {
            var correlated = body as ICorrelate;
            if (correlated != null)
            {
                properties.CorrelationId = correlated.CorrelationId;
            }
        }

        protected void SetMessageCorrelation(object message, BasicGetResult result)
        {
            var correlated = message as ICorrelate;
            if (correlated != null)
            {
                correlated.CorrelationId = result.BasicProperties.CorrelationId;
            }
        }

        public QueueingBasicConsumer GetConsumer()
        {
            var consumer = new QueueingBasicConsumer(Channel);
            _channel.BasicConsume(
                _configuration.QueueName,
                _configuration.NoAck,
                null,
                consumer);

            return consumer;
        }

        public void InitConsumer(IBasicConsumer consumer)
        {
            _channel.BasicConsume(
                _configuration.QueueName,
                _configuration.NoAck,
                null,
                consumer);
        }

        public ChannelProxy(IModel channel, string protocol, IAmqpEndpointConfiguration endpointConfiguration)
        {
            _channel = channel;
            _protocol = protocol;
            _configuration = endpointConfiguration;
            _onReturn = ServiceLocator.Current.GetInstance<Action<IModel, BasicReturnEventArgs>>();
            _channel.BasicReturn += new BasicReturnEventHandler(_onReturn);
            _channel.ModelShutdown += ChannelShutdown;
        }

        public void OnReturn()
        {
            Channel.BasicReturn += new BasicReturnEventHandler(Channel_BasicReturn);
        }

        void Channel_BasicReturn(IModel model, BasicReturnEventArgs args)
        {
            
        }

        private void ChannelShutdown(IModel model, ShutdownEventArgs reason)
        {
            "A channel proxy shut down. \r\n\t Class {0} \r\n\t Method {1} \r\n\t Cause {2} \r\n\t ReplyCode {3} : {4}"
                .ToInfo<IBus>(
                    reason.ClassId,
                    reason.MethodId,
                    reason.Cause,
                    reason.ReplyCode,
                    reason.ReplyText
                    );
        }

        public void Dispose()
        {
            Close();
        }

        protected void Close()
        {
            _closed = true;
            if (_channel != null)
            {
                _channel.BasicReturn -= new BasicReturnEventHandler(_onReturn);
                _channel.ModelShutdown -= ChannelShutdown;
                if (_channel.IsOpen)
                {
                    try
                    {
                        _channel.Close();
                        _channel.Dispose();
                        _channel = null;
                    }
                    catch (Exception e)
                    {
                        "An exception occurred trying to close a RabbitMQ channel \r\n\t {0}"
                            .ToError<IBus>(e);
                    }
                }
            }
        }
    }
}