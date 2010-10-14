using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Rabbit.Config;

namespace Rabbit.Tests
{
    public class with_rabbit_configuration
    {
        protected static RabbitConfigurator rabbit { get; set; }
    }

    public class when_configuring_rabbit
        : with_rabbit_configuration
    {
        private Because of = () =>
                                 {
                                     rabbit
                                         .Server(s => s
                                             .Address("")
                                             .Port(123)
                                             .VirtualHost("")
                                             .AMQP091()
                                             .User("")
                                             .Password("")
                                             .Broker("")
                                             .BalanceGroup(0));
                                 };
    }
}
