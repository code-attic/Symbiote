using System;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using StructureMap;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl
{
    public class ChannelProxy : IChannelProxy
    {
        private IModel _channel;
        private IAmqpEndpointConfiguration _configuration;
        private IMessageSerializer _messageSerializer;
        private Action<IModel, BasicReturnEventArgs> _onReturn;
        private string _protocol;
        
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

        public string QueueName
        {
            get { return _configuration.QueueName ?? ""; }
        }

        public IMessageSerializer Serializer
        {
            get
            {
                _messageSerializer = _messageSerializer ?? ObjectFactory.GetInstance<IMessageSerializer>();
                return _messageSerializer;
            }
        }

        public void Acknowledge(ulong tag, bool multiple)
        {
            _channel.BasicAck(tag, multiple);
        }

        public virtual Envelope Dequeue()
        {
             BasicDeliverEventArgs result = null;
            QueueingBasicConsumer consumer = null;
            try
            {
                consumer = GetConsumer();
                result = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                if (result != null)
                {
                    var message = Serializer.Deserialize(result.Body);
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

            var contentType = "text/plain";
            var stream = Serializer.Serialize(body);
            IBasicProperties properties = CreatePublishingProperties(contentType);
            properties.ReplyToAddress = new PublicationAddress(EndpointConfiguration.ExchangeType.ToString(), EndpointConfiguration.ExchangeName, routingKey);
            Channel.BasicPublish(_configuration.ExchangeName, routingKey, properties, stream);
        }

        public void Send<T>(T body, string routingKey, bool mandatory, bool immediate) where T : class
        {
            if (body == default(T))
                return;

            var contentType = "text/plain";
            var stream = Serializer.Serialize(body);
            IBasicProperties properties = CreatePublishingProperties(contentType);
            properties.ReplyToAddress = new PublicationAddress(EndpointConfiguration.ExchangeType.ToString(), EndpointConfiguration.ExchangeName, routingKey);
            Channel.BasicPublish(_configuration.ExchangeName, routingKey, mandatory, immediate, properties, stream);
        }

        public void Reject(ulong tag, bool requeue)
        {
            //This call is currently unimplemented for AMQP 0.8
            if(_protocol != "AMQP_0_8")
                Channel.BasicReject(tag, requeue);
            Channel.Dispose();
        }

        public void Reply<T>(PublicationAddress address, IBasicProperties properties, T response)
            where T : class
        {
            //This call is currently unimplemented for AMQP 0.8
            if (_protocol != "AMQP_0_8")
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
            if(correlated != null)
            {
                properties.CorrelationId = correlated.CorrelationId;
            }
        }

        protected void SetMessageCorrelation(object message, BasicGetResult result)
        {
            var correlated = message as ICorrelate;
            if(correlated != null)
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

        public ChannelProxy(IModel channel, string protocol, IAmqpEndpointConfiguration endpointConfiguration)
        {
            _channel = channel;
            _protocol = protocol;
            _configuration = endpointConfiguration;
            _onReturn = ObjectFactory.GetInstance<Action<IModel, BasicReturnEventArgs>>();
            _channel.BasicReturn += new BasicReturnEventHandler(_onReturn);
            _channel.ModelShutdown += ChannelShutdown;
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