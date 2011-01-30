// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using RabbitMQ.Client;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelDefinition
        : BaseChannelDefinition, IRabbitChannelDetails
    {
        public bool CreatedOnBroker { get; set; }
        public bool PersistentDelivery { get; set; }

        public override Type ChannelType
        {
            get { return typeof( RabbitChannel ); }
        }

        public override Type FactoryType
        {
            get { return typeof( RabbitChannelFactory ); }
        }

        public bool AutoDelete { get; set; }
        public string Broker { get; set; }
        public bool Durable { get; set; }
        public string Exchange { get; set; }
        public ExchangeType ExchangeType { get; set; }

        public string ExchangeTypeName
        {
            get { return ExchangeType.ToString(); }
        }

        public bool Immediate { get; set; }
        public bool Internal { get; set; }
        public bool Mandatory { get; set; }
        public bool NoWait { get; set; }
        public bool Passive { get; set; }
        public bool Transactional { get; set; }

        public void BuildExchange( IModel channel )
        {
            channel.ExchangeDeclare(
                Exchange,
                ExchangeTypeName,
                Passive,
                Durable,
                AutoDelete,
                Internal,
                NoWait,
                null );
        }

        public void CreateOnBroker( IConnectionManager manager )
        {
            if ( !CreatedOnBroker )
            {
                var connection = manager.GetConnection( Broker );
                using( var channel = connection.CreateModel() )
                {
                    BuildExchange( channel );
                    CreatedOnBroker = true;
                }
            }
        }

        public ChannelDefinition()
        {
            Broker = "default";
        }
    }
}