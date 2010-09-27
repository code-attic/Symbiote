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

namespace Symbiote.Jackalope.Config
{
    public class AmqpFluentServerConfiguration
    {
        internal AmqpServer Server { get; set; }

        public AmqpFluentServerConfiguration Address(string address)
        {
            Server.Address = address;
            return this;
        }

        public AmqpFluentServerConfiguration Broker(string brokerName)
        {
            Server.Broker = brokerName;
            return this;
        }

        public AmqpFluentServerConfiguration Port(int port)
        {
            Server.Port = port;
            return this;
        }

        public AmqpFluentServerConfiguration User(string user)
        {
            Server.User = user;
            return this;
        }

        public AmqpFluentServerConfiguration Password(string password)
        {
            Server.Password = password;
            return this;
        }

        public AmqpFluentServerConfiguration VirtualHost(string virtualHost)
        {
            Server.VirtualHost = virtualHost;
            return this;
        }

        public AmqpFluentServerConfiguration AMQP08()
        {
            Server.Protocol = "AMQP_0_8";
            return this;
        }

        public AmqpFluentServerConfiguration AMQP09()
        {
            Server.Protocol = "AMQP_0_9";
            return this;
        }

        public AmqpFluentServerConfiguration AMQP091()
        {
            Server.Protocol = "AMQP_0_9_1";
            return this;
        }

        public AmqpFluentServerConfiguration()
        {
            Server = new AmqpServer();
        }
    }
}