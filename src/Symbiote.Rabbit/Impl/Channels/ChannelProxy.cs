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
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Endpoint;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelProxy : IChannelProxy
    {
        private IModel _channel;
        private RabbitEndpoint _configuration;
        private IMessageSerializer _messageSerializer;
        private Action<IModel, BasicReturnEventArgs> _onReturn;
        private string _protocol;
        private bool _closed;
        private readonly long EPOCH = 621355968000000000L;

        private const string CONTENT_TYPE = "text/plain";
        private const string AMQP_08 = "AMQP_0_8";

        public RabbitEndpoint EndpointConfiguration
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

        public void Acknowledge(ulong tag, bool multiple)
        {
            Action<ulong, bool> call = ActualAck;
            call.BeginInvoke(tag, multiple, null, null);
        }

        protected void ActualAck(ulong tag, bool multiple)
        {
            Channel.BasicAck(tag, multiple);
        }

        public void Send<T>(RabbitEnvelope<T> envelope, byte[] stream)
        {
            IBasicProperties properties = CreatePublishingProperties(CONTENT_TYPE, envelope);
            Channel.BasicPublish(
                _configuration.ExchangeName,
                envelope.RoutingKey,
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

        protected IBasicProperties CreatePublishingProperties<T>(string contentType, RabbitEnvelope<T> envelope)
        {
            var properties = Channel.CreateBasicProperties();
            properties.DeliveryMode = (byte)(_configuration.PersistentDelivery ? DeliveryMode.Persistent : DeliveryMode.Volatile);
            properties.ContentType = contentType;
            properties.CorrelationId = envelope.CorrelationId;
            properties.MessageId = envelope.MessageId.ToString();
            properties.Headers = new Dictionary<object, object>();
            properties.Headers.Add("Sequence", envelope.Sequence);
            properties.Headers.Add("SequenceEnd", envelope.SequenceEnd);
            properties.Headers.Add("Position", envelope.Position);
            properties.ReplyToAddress = new PublicationAddress(ExchangeType.direct.ToString(), envelope.ReplyToExchange, envelope.ReplyToKey);
            properties.Timestamp = new AmqpTimestamp( DateTime.Now.Ticks + EPOCH );
            return properties;
        }

        protected void SetMessageCorrelation(IEnvelope envelope, BasicGetResult result)
        {
            envelope.CorrelationId = result.BasicProperties.CorrelationId;
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

        public ChannelProxy(IModel channel, string protocol, RabbitEndpoint endpointConfiguration)
        {
            _channel = channel;
            _protocol = protocol;
            _configuration = endpointConfiguration;
            if (_configuration.UseTransactions)
                channel.TxSelect();
            //_onReturn = Assimilate.GetInstanceOf<Action<IModel, BasicReturnEventArgs>>();
            //_channel.BasicReturn += new BasicReturnEventHandler(_onReturn);
            //_channel.ModelShutdown += ChannelShutdown;
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