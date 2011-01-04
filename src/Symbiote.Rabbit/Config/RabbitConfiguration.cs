﻿/* 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Rabbit.Config
{
    public class RabbitConfiguration
    {
        public bool AsNode { get; set; }
        public ConcurrentDictionary<string, IRabbitBroker> Brokers { get; set; }

        public RabbitConfiguration AddBroker(IRabbitBroker broker)
        {
            Brokers.GetOrAdd(broker.Name, broker);
            return this;
        }

        public RabbitConfiguration AddBroker(Action<RabbitBrokerFluentConfigurator> configurate)
        {
            var configurator = new RabbitBrokerFluentConfigurator();
            configurate(configurator);
            AddBroker(configurator.RabbitBroker);
            return this;
        }

        public RabbitConfiguration EnrollAsMeshNode()
        {
            AsNode = true;
            return this;
        }

        public RabbitConfiguration()
        {
            Brokers = new ConcurrentDictionary<string, IRabbitBroker>();
        }
    }


}
