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
namespace Symbiote.Rabbit.Config
{
    public class RabbitBrokerFluentConfigurator
    {
        internal RabbitBroker RabbitBroker { get; set; }

        public RabbitBrokerFluentConfigurator Address( string address )
        {
            RabbitBroker.Address = address;
            return this;
        }

        public RabbitBrokerFluentConfigurator AMQP08()
        {
            RabbitBroker.Protocol = "AMQP_0_8";
            return this;
        }

        public RabbitBrokerFluentConfigurator AMQP09()
        {
            RabbitBroker.Protocol = "AMQP_0_9";
            return this;
        }

        public RabbitBrokerFluentConfigurator AMQP091()
        {
            RabbitBroker.Protocol = "AMQP_0_9_1";
            return this;
        }

        public RabbitBrokerFluentConfigurator BalanceGroup( int group )
        {
            RabbitBroker.BalanceGroup = group;
            return this;
        }

        public RabbitBrokerFluentConfigurator Broker( string brokerName )
        {
            RabbitBroker.Name = brokerName;
            return this;
        }

        public RabbitBrokerFluentConfigurator Defaults()
        {
            return this;
        }

        public RabbitBrokerFluentConfigurator Port( int port )
        {
            RabbitBroker.Port = port;
            return this;
        }

        public RabbitBrokerFluentConfigurator Password( string password )
        {
            RabbitBroker.Password = password;
            return this;
        }

        public RabbitBrokerFluentConfigurator User( string user )
        {
            RabbitBroker.User = user;
            return this;
        }

        public RabbitBrokerFluentConfigurator VirtualHost( string virtualHost )
        {
            RabbitBroker.VirtualHost = virtualHost;
            return this;
        }

        public RabbitBrokerFluentConfigurator()
        {
            RabbitBroker = new RabbitBroker();
        }
    }
}