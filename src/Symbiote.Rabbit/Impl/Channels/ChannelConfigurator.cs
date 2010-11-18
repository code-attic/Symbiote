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

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelConfigurator
    {
        public ChannelDefinition ChannelDefinition { get; protected set; }

        public ChannelConfigurator Direct(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.direct;
            return this;
        }

        public ChannelConfigurator Fanout(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.fanout;
            return this;
        }

        public ChannelConfigurator Topic(string exchangeName)
        {
            ChannelDefinition.Exchange = exchangeName;
            ChannelDefinition.Name = exchangeName;
            ChannelDefinition.ExchangeType = ExchangeType.topic;
            return this;
        }

        public ChannelConfigurator AutoDelete()
        {
            ChannelDefinition.AutoDelete = true;
            return this;
        }

        public ChannelConfigurator Durable()
        {
            ChannelDefinition.Durable = true;
            return this;
        }

        public ChannelConfigurator Immediate()
        {
            ChannelDefinition.Immediate = true;
            return this;
        }

        public ChannelConfigurator Internal()
        {
            ChannelDefinition.Internal = true;
            return this;
        }

        public ChannelConfigurator Mandatory()
        {
            ChannelDefinition.Mandatory = true;
            return this;
        }

        public ChannelConfigurator NoWait()
        {
            ChannelDefinition.NoWait = true;
            return this;
        }

        public ChannelConfigurator Passive()
        {
            ChannelDefinition.Passive = true;
            return this;
        }

        public ChannelConfigurator PersistentDelivery()
        {
            ChannelDefinition.PersistentDelivery = true;
            return this;
        }

        public ChannelConfigurator UseTransactions()
        {
            ChannelDefinition.Transactional = true;
            return this;
        }

        public ChannelConfigurator SerializeBy<TSerializer>()
        {
            ChannelDefinition.SerializerType = typeof(TSerializer);
            return this;
        }

        public ChannelConfigurator RouteBy<TMessage>(string routingKey)
        {
            ChannelDefinition.RouteBy<TMessage>( routingKey );
            return this;
        }

        public ChannelConfigurator RouteBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            ChannelDefinition.RouteBy( messageProperty );
            return this;
        }

        public ChannelConfigurator CorrelateBy<TMessage>(string correlationId)
        {
            ChannelDefinition.CorrelateBy<TMessage>( correlationId );
            return this;
        }

        public ChannelConfigurator CorrelateBy<TMessage>(Func<TMessage, string> messageProperty)
        {
            ChannelDefinition.CorrelateBy( messageProperty );
            return this;
        }

        public ChannelConfigurator()
        {
            ChannelDefinition = new ChannelDefinition();
        }
    }
}