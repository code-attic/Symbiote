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
using Symbiote.Wcf.Client;

namespace Symbiote.Wcf
{
    public class WcfClientConfigurator
    {
        public WcfClientConfigurator RegisterService<TContract, TStrategy>()
            where TStrategy : IServiceClientConfigurationStrategy<TContract>
            where TContract : class
        {
            Assimilate.Dependencies( x => x
                                              .For<IServiceClientConfigurationStrategy<TContract>>()
                                              .Use<TStrategy>() );
            return this;
        }

        public WcfClientConfigurator RegisterService<TContract>( Action<IServiceConfiguration> configurationDelegate )
            where TContract : class
        {
            Assimilate.Dependencies( x => x
                                              .For<IServiceClientConfigurationStrategy<TContract>>()
                                              .CreateWith(
                                                  context =>
                                                  new DelegateConfigurationStrategy<TContract>( configurationDelegate ) ) );
            return this;
        }

        public WcfClientConfigurator RegisterService<TContract>()
            where TContract : class
        {
            return this;
        }
    }
}