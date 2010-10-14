﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Rabbit.Config
{
    public class RabbitConfiguration
    {
        public ConcurrentDictionary<string, IRabbitBroker> Brokers { get; set; }

        public void AddBroker(IRabbitBroker broker)
        {
            Brokers.GetOrAdd(broker.Name, broker);
        }

        public RabbitConfiguration()
        {
            Brokers = new ConcurrentDictionary<string, IRabbitBroker>();
        }
    }


}