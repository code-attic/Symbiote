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

namespace Symbiote.Rabbit.Impl.Channels
{
    public interface IRabbitChannelDetails
    {
        string Broker { get; set; }
        string Exchange { get; set; }
        ExchangeType ExchangeType { get; set; }
        string ExchangeTypeName { get; }
        bool Passive { get; set; }
        bool Durable { get; set; }
        bool AutoDelete { get; set; }
        bool Immediate { get; set; }
        bool Internal { get; set; }
        bool NoWait { get; set; }
        bool Mandatory { get; set; }
        bool Transactional { get; set; }
    }
}