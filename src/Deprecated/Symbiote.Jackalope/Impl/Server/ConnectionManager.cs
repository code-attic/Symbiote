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
using System.Linq;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl.Server
{
    public class ConnectionManager : IConnectionManager
    {
        protected IAmqpConfigurationProvider Provider { get; set; }

        protected IBroker DefaultBroker { get { return Provider.Brokers["default"]; } }

        public string Protocol
        {
            get { return Provider.Brokers.Values.First().Protocol; }
        }

        public IEnumerable<IConnection> GetBrokerConnections(string brokerName)
        {
            return Provider.Brokers[brokerName].GetConnections();
        }

        public IConnection GetConnection()
        {
            return DefaultBroker.GetConnection();
        }

        public IConnection GetConnection(string brokerName)
        {
            return Provider.Brokers[brokerName].GetConnection();
        }

        public IConnection GetConnection<T>(T id)
        {
            return DefaultBroker.GetBalancedConnection(id.ToString());
        }

        public IConnection GetConnection<T>(T id, string brokerName)
        {
            return Provider.Brokers[brokerName].GetBalancedConnection(id.ToString());
        }

        public ConnectionManager(IAmqpConfigurationProvider configurationProvider)
        {
            Provider = configurationProvider;
        }
    }
}