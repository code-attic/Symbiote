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

using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl.Endpoint
{
    public interface IEndPoint
    {
        IAmqpEndpointConfiguration EndpointConfiguration { get; }
        IEndPoint Broker(string broker);
        IEndPoint LoadBalanced();
        IEndPoint QueueName(string queueName);
        IEndPoint RoutingKeys(params string[] routingKeys);
        IEndPoint Exchange(string exchangeName, ExchangeType exchange);
        IEndPoint Durable();
        IEndPoint Exclusive();
        IEndPoint Passive();
        IEndPoint AutoDelete();
        IEndPoint Immediate();
        IEndPoint Internal();
        IEndPoint Mandatory();
        IEndPoint NoWait();
        IEndPoint NoAck();
        IEndPoint PersistentDelivery();
        bool Initialized { get; set; }
    }
}