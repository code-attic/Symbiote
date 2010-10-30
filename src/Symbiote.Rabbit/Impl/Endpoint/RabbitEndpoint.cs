using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class RabbitEndpoint
    {
        public IDictionary Arguments { get; set; }
        public bool AutoDelete { get; set; }
        public string Broker { get; set; }
        public bool CreatedOnBroker { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public string ExchangeName { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Internal { get; set; }
        public bool ImmediateDelivery { get; set; }
        public bool LoadBalance { get; set; }
        public bool NeedsResponseChannel { get; set; }
        public bool NoAck { get; set; }
        public bool NoWait { get; set; }
        public bool MandatoryDelivery { get; set; }
        public bool Passive { get; set; }
        public bool PersistentDelivery { get; set; }
        public string QueueName { get; set; }
        public List<string> RoutingKeys { get; set; }
        public bool UseTransactions { get; set; }

        public void BindQueue(IModel channel)
        {
            if (RoutingKeys.Count == 0)
                RoutingKeys = new List<string>(new[] { "" });
            try
            {
                RoutingKeys
                    .ForEach( x => channel.QueueBind( QueueName, ExchangeName, x, false, null ) );
            }
            catch (Exception x)
            {
                throw;
            }
        }

        public void BuildExchange(IModel channel)
        {
            channel.ExchangeDeclare(
                ExchangeName,
                ExchangeTypeName,
                Passive,
                Durable,
                AutoDelete,
                Internal,
                NoWait,
                Arguments);
        }

        public void BuildQueue(IModel channel)
        {
            channel.QueueDeclare(
                QueueName,
                Passive,
                Durable,
                Exclusive,
                AutoDelete,
                NoWait,
                Arguments);
        }

        public void CreateOnBroker(IConnectionManager manager)
        {
            if (!CreatedOnBroker)
            {
                var connection = manager.GetConnection(Broker);
                using (var channel = connection.CreateModel())
                {
                    if (!string.IsNullOrEmpty(ExchangeName))
                        BuildExchange(channel);

                    if (!string.IsNullOrEmpty(QueueName))
                        BuildQueue(channel);

                    if (!string.IsNullOrEmpty(ExchangeName) && !string.IsNullOrEmpty(QueueName))
                        BindQueue(channel);

                    CreatedOnBroker = true;
                }
            }
        }

        public RabbitEndpoint()
        {
            Broker = "default";
            ExchangeName = "";
            RoutingKeys = new List<string>();
            NoWait = false;
        }
    }
}
