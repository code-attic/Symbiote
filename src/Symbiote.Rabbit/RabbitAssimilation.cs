// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.Rabbit.Config;

namespace Symbiote.Rabbit
{
    public static class RabbitAssimilation
    {
        public static IAssimilate Rabbit( this IAssimilate assimilate, Action<RabbitConfiguration> configurate )
        {
            var configuration = Assimilate.GetInstanceOf<RabbitConfiguration>();
            configurate( configuration );
            Assimilate.Dependencies( x => x.For<RabbitConfiguration>()
                                              .Use( configuration ) );
            if ( configuration.AsNode )
            {
                var initializer = Assimilate.GetInstanceOf<INodeChannelManager>();
                initializer.InitializeChannels();
            }
            return assimilate;
        }
    }
}