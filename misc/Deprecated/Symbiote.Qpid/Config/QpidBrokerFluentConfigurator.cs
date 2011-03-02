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

namespace Symbiote.Qpid.Config
{
    public class QpidBrokerFluentConfigurator
    {
        internal QpidBroker QpidBroker { get; set; }

        public QpidBrokerFluentConfigurator Address(string address)
        {
            QpidBroker.Address = address;
            return this;
        }

        public QpidBrokerFluentConfigurator Broker(string brokerName)
        {
            QpidBroker.Name = brokerName;
            return this;
        }

        public QpidBrokerFluentConfigurator Defaults()
        {
            return this;
        }

        public QpidBrokerFluentConfigurator Port(int port)
        {
            QpidBroker.Port = port;
            return this;
        }

        public QpidBrokerFluentConfigurator Password(string password)
        {
            QpidBroker.Password = password;
            return this;
        }

        public QpidBrokerFluentConfigurator User(string user)
        {
            QpidBroker.User = user;
            return this;
        }

        public QpidBrokerFluentConfigurator VirtualHost(string virtualHost)
        {
            QpidBroker.VirtualHost = virtualHost;
            return this;
        }

        public QpidBrokerFluentConfigurator()
        {
            QpidBroker = new QpidBroker();
        }
    }
}