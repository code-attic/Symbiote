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
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannelDefinition
        : BaseChannelDefinition, IRabbitChannelDetails
    {
        public string Broker { get; set; }
        public string Exchange { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Passive { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public bool Immediate { get; set; }
        public bool Internal { get; set; }
        public bool NoWait { get; set; }
        public bool Mandatory { get; set; }
        public bool Transactional { get; set; }
        public override Type ChannelType { get { return typeof(RabbitChannel); } }
        public override Type FactoryType { get { return typeof(RabbitChannelFactory); } }
        
        public RabbitChannelDefinition() : base()
        {
            Broker = "default";
        }
    }

    public class RabbitChannelDefinition<TMessage>
        : BaseChannelDefinition<TMessage>, IRabbitChannelDetails
    {
        public string Broker { get; set; }
        public string Exchange { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public string ExchangeTypeName { get { return ExchangeType.ToString(); } }
        public bool Passive { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public bool Immediate { get; set; }
        public bool Internal { get; set; }
        public bool NoWait { get; set; }
        public bool Mandatory { get; set; }
        public bool Transactional { get; set; }
        public override Type ChannelType { get { return typeof(RabbitChannel<TMessage>); } }
        public override Type FactoryType { get { return typeof(RabbitChannelFactory<TMessage>); } }

        public RabbitChannelDefinition()
            : base()
        {
            Broker = "default";
        }
    }
}
