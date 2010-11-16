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
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class RabbitChannelConfigurator
    {
        public RabbitChannelDefinition ChannelDefinition { get; protected set; }

        public RabbitChannelConfigurator Direct(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.direct;
            return this;
        }

        public RabbitChannelConfigurator Fanout(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.fanout;
            return this;
        }

        public RabbitChannelConfigurator Topic(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.topic;
            return this;
        }

        public RabbitChannelConfigurator AutoDelete()
        {
            ChannelDefinition.AutoDelete = true;
            return this;
        }

        public RabbitChannelConfigurator Durable()
        {
            ChannelDefinition.Durable = true;
            return this;
        }

        public RabbitChannelConfigurator Immediate()
        {
            ChannelDefinition.Immediate = true;
            return this;
        }

        public RabbitChannelConfigurator Internal()
        {
            ChannelDefinition.Internal = true;
            return this;
        }

        public RabbitChannelConfigurator Mandatory()
        {
            ChannelDefinition.Mandatory = true;
            return this;
        }

        public RabbitChannelConfigurator NoWait()
        {
            ChannelDefinition.NoWait = true;
            return this;
        }

        public RabbitChannelConfigurator Passive()
        {
            ChannelDefinition.Passive = true;
            return this;
        }

        public RabbitChannelConfigurator UseTransactions()
        {
            ChannelDefinition.Transactional = true;
            return this;
        }

        public RabbitChannelConfigurator RouteBy<TMessage>(string routingKey)
        {
            ChannelDefinition.RouteBy<TMessage>( routingKey );
            return this;
        }

        public RabbitChannelConfigurator RouteBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            ChannelDefinition.RouteBy( messageProperty );
            return this;
        }

        public RabbitChannelConfigurator CorrelateBy<TMessage>(string correlationId)
        {
            ChannelDefinition.CorrelateBy<TMessage>( correlationId );
            return this;
        }

        public RabbitChannelConfigurator CorrelateBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            ChannelDefinition.CorrelateBy( messageProperty );
            return this;
        }

        public RabbitChannelConfigurator()
        {
            ChannelDefinition = new RabbitChannelDefinition();
        }
    }
}