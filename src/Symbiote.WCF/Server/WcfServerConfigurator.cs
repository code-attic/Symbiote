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
using System.Collections.Generic;

namespace Symbiote.Wcf.Server
{
    public class WcfServerConfigurator
    {
        public List<IWcfServiceConfiguration> ServiceConfigurations { get; protected set; }
        public string BaseAddress { get; set; }

        public WcfServerConfigurator AddService<TService>( Action<WcfServiceConfigurator<TService>> service )
            where TService : class
        {
            var wcfServiceConfigurator = new WcfServiceConfigurator<TService>();
            service( wcfServiceConfigurator );
            ServiceConfigurations.Add( wcfServiceConfigurator.GetConfiguration() );
            return this;
        }

        public WcfServerConfigurator( string baseAddress )
        {
            BaseAddress = baseAddress;
            ServiceConfigurations = new List<IWcfServiceConfiguration>();
        }
    }
}