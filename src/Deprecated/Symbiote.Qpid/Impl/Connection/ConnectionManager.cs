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

using System.Linq;
using org.apache.qpid.client;
using Symbiote.Qpid.Config;

namespace Symbiote.Qpid.Impl.Connection
{
    public class ConnectionManager : IConnectionManager
    {
        protected QpidConfiguration Configuration { get; set; }

        protected IQpidBroker DefaultBroker { get { return Configuration.Brokers["default"]; } }

        public IClient GetConnection()
        {
            return DefaultBroker.GetClient();
        }

        public IClient GetConnection(string brokerName)
        {
            return Configuration.Brokers[brokerName].GetClient();
        }

        public ConnectionManager(QpidConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}