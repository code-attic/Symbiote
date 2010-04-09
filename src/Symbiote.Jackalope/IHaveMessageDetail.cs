using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace Symbiote.Jackalope
{
    public interface IHaveMessageDetail
    {
        IBasicProperties MessageProperties { get; set; }
    }
}
