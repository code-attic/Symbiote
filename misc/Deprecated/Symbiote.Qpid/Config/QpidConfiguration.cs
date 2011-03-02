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
using System.Collections.Concurrent;

namespace Symbiote.Qpid.Config
{
    public class QpidConfiguration
    {
        public ConcurrentDictionary<string, IQpidBroker> Brokers { get; set; }

        public void AddBroker(IQpidBroker broker)
        {
            Brokers.GetOrAdd(broker.Name, broker);
        }

        public void AddBroker(Action<QpidBrokerFluentConfigurator> configurate)
        {
            var configurator = new QpidBrokerFluentConfigurator();
            configurate(configurator);
            AddBroker(configurator.QpidBroker);
        }

        public QpidConfiguration()
        {
            Brokers = new ConcurrentDictionary<string, IQpidBroker>();
        }
    }
}