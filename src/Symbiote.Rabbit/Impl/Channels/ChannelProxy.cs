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
using Symbiote.Rabbit.Impl.Endpoint;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelProxy : IChannelProxy
    {
        private IModel _channel;
        private RabbitEndpoint _endpoint;
        private ChannelDefinition _channelDefinition;
        private Action<IModel, BasicReturnEventArgs> _onReturn;
        private string _protocol;
        private bool _closed;
        private readonly string SEQUENCE = "Sequence";
        private readonly string SEQUENCE_END = "SequenceEnd";
        private readonly string POSITION = "Position";
        private readonly string MESSAGE_TYPE = "MessageType";
        private readonly string DIRECT_EXCHANGE = ExchangeType.direct.ToString();
        private readonly string CONTENT_TYPE = "text/plain";
        private readonly string AMQP_08 = "AMQP_0_8";
        private readonly byte _deliveryMode;
        private IBasicProperties _propertyTemplate;
        private object _lock { get; set; }

        public RabbitEndpoint EndpointConfiguration
        {
            get { return _endpoint; }
        }

        public ChannelDefinition ChannelDefinition
        {
            get { return _channelDefinition; }
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
            get { return _endpoint.QueueName ?? ""; }
        }

        // TODO: Rewrite this, it's stupid.
        public void Acknowledge(ulong tag, bool multiple)
        {
            Action<ulong, bool> call = ActualAck;
            call.BeginInvoke(tag, multiple, null, null);
        }

        protected void ActualAck(ulong tag, bool multiple)
        {
            Channel.BasicAck(tag, multiple);
        }

        public void Send<T>(RabbitEnvelope<T> envelope)
        {
            try
            {
                lock(_lock)
                {
                    IBasicProperties properties = CreatePublishingProperties(CONTENT_TYPE, envelope);
                    Channel.BasicPublish(
                        ChannelDefinition.Exchange,
                        envelope.RoutingKey,
                        ChannelDefinition.Mandatory,
                        ChannelDefinition.Immediate,
                        properties,
                        envelope.ByteStream);
                }
            }
            catch (Exception e)
            {
                "Sending a Rabbit message caused an exception. \r\n\t Exchange: {0} \r\n\t MessageType: {1} \r\n\t MessageId: {2} \r\n\t CorrelationId: {3} \r\n\t {4}"
                    .ToError<IBus>(_endpoint.ExchangeName, typeof(T).AssemblyQualifiedName,  envelope.MessageId, envelope.CorrelationId, e);
            }
        }

        public void Reject(ulong tag, bool requeue)
        {
            //This call is currently unimplemented for AMQP 0.8
            if (_protocol != AMQP_08)
                Channel.BasicReject(tag, requeue);
        }

        protected IBasicProperties CreatePublishingProperties<T>(string contentType, RabbitEnvelope<T> envelope)
        {
            try
            {
                _propertyTemplate.CorrelationId = envelope.CorrelationId;
                _propertyTemplate.MessageId = envelope.MessageId.ToString();
                _propertyTemplate.Headers[SEQUENCE] = envelope.Sequence;
                _propertyTemplate.Headers[SEQUENCE_END] = envelope.SequenceEnd;
                _propertyTemplate.Headers[POSITION] = envelope.Position;
                _propertyTemplate.Headers[MESSAGE_TYPE] = envelope.MessageType.AssemblyQualifiedName;
                _propertyTemplate.ReplyToAddress = new PublicationAddress(DIRECT_EXCHANGE, envelope.ReplyToExchange, envelope.ReplyToKey);
                _propertyTemplate.Timestamp = new AmqpTimestamp(DateTime.UtcNow.ToUnixTimestamp());
                return _propertyTemplate;
            }
            catch (Exception e)
            {
                "Setting publishing properties for a Rabbit message caused an exception. \r\n\t Exchange: {0} \r\n\t MessageType: {1} \r\n\t {2}"
                    .ToError<IBus>(_endpoint.ExchangeName, typeof(T).AssemblyQualifiedName, e);
            }
            return null;
        }

        protected void SetPropertyTemplate()
        {
            var properties = Channel.CreateBasicProperties();
            properties.DeliveryMode = _deliveryMode;
            properties.ContentType = CONTENT_TYPE;
            properties.Headers = new Dictionary<object, object>(4);
            properties.Headers.Add(SEQUENCE, 0);
            properties.Headers.Add(SEQUENCE_END, false);
            properties.Headers.Add(POSITION, 0);
            properties.Headers.Add(MESSAGE_TYPE, "");
            _propertyTemplate = properties;
        }

        protected void SetMessageCorrelation(IEnvelope envelope, BasicGetResult result)
        {
            envelope.CorrelationId = result.BasicProperties.CorrelationId;
        }

        public QueueingBasicConsumer GetConsumer()
        {
            var consumer = new QueueingBasicConsumer(Channel);
            _channel.BasicConsume(
                EndpointConfiguration.QueueName,
                EndpointConfiguration.NoAck,
                null,
                consumer);

            return consumer;
        }

        public void InitConsumer(IBasicConsumer consumer)
        {
            _channel.BasicConsume(
                EndpointConfiguration.QueueName,
                EndpointConfiguration.NoAck,
                null,
                consumer);
        }

        public ChannelProxy(IModel channel, string protocol, RabbitEndpoint endpointConfiguration)
        {
            _lock = new object();
            _channel = channel;
            _protocol = protocol;
            _endpoint = endpointConfiguration;
            if (_endpoint.Transactional)
                channel.TxSelect();
            _onReturn = Assimilate.GetInstanceOf<Action<IModel, BasicReturnEventArgs>>();
            _channel.BasicReturn += new BasicReturnEventHandler(_onReturn);
            _channel.ModelShutdown += ChannelShutdown;
        }

        public ChannelProxy(IModel channel, string protocol, ChannelDefinition channelDefinition)
        {
            _lock = new object();
            _channel = channel;
            _protocol = protocol;
            _channelDefinition = channelDefinition;
            if (_channelDefinition.Transactional)
                channel.TxSelect();
            _onReturn = Assimilate.GetInstanceOf<Action<IModel, BasicReturnEventArgs>>();
            _channel.BasicReturn += new BasicReturnEventHandler(_onReturn);
            _channel.ModelShutdown += ChannelShutdown;
            _deliveryMode = (byte)(ChannelDefinition.PersistentDelivery ? DeliveryMode.Persistent : DeliveryMode.Volatile);
            SetPropertyTemplate();
        }

        public void OnReturn()
        {
            Channel.BasicReturn += Channel_BasicReturn;
        }

        private void Channel_BasicReturn(IModel model, BasicReturnEventArgs args)
        {
            if (_onReturn != null)
                _onReturn(model, args);
        }

        private void ChannelShutdown(IModel model, ShutdownEventArgs reason)
        {
            "Shutting down rabbit channel. \r\n\t Exchange: {0} \r\n\t Queue: {1} \r\n\t Class {2} \r\n\t Method {3} \r\n\t Cause {4} \r\n\t ReplyCode {5} : {6}"
                .ToInfo<IBus>(
                    _endpoint.ExchangeName,
                    _endpoint.QueueName,
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
                if(_onReturn != null)
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
                        "Closing a RabbitMQ channel caused and exception\r\n\t {0}"
                            .ToError<IBus>(e);
                    }
                }
            }
        }
    }
}