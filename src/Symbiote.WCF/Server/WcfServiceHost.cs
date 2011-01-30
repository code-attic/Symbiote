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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Symbiote.Core.Extensions;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceHost : IDisposable
    {
        protected WcfServerConfigurator ServerConfiguration { get; set; }
        protected List<ServiceHost> ServiceHosts { get; set; }

        public void Dispose()
        {
            Stop();
            ServiceHosts.Clear();
        }

        public void Start()
        {
            ServiceHosts.ForEach( x => x.Open() );
        }

        public void Stop()
        {
            ServiceHosts.ForEach( x => x.Close() );
        }

        private ServiceHost BuildHost( IWcfServiceConfiguration configuration )
        {
            var serviceUri = configuration.Address;
            var host = new ServiceHost( configuration.ServiceType, new Uri( ServerConfiguration.BaseAddress ) );
            host.AddServiceEndpoint(
                configuration.ContractType,
                configuration.Binding,
                serviceUri );

            if ( configuration.EnableHttpMetadataExchange )
            {
                var mexUriString =
                    "{0}/{1}".AsFormat( configuration.MetadataExchangeUri ?? ServerConfiguration.BaseAddress,
                                        configuration.Address );

                host.Description.Behaviors.Add(
                    new ServiceMetadataBehavior
                        {
                            HttpGetEnabled = true,
                            HttpGetUrl = new Uri( mexUriString )
                        } );

                host.AddServiceEndpoint( typeof( IMetadataExchange ), configuration.Binding, "mex" );
            }

            host.CloseTimeout = TimeSpan.FromMilliseconds( configuration.Timeout );
            return host;
        }

        public WcfServiceHost( WcfServerConfigurator serverConfiguration )
        {
            ServerConfiguration = serverConfiguration;
            ServiceHosts = serverConfiguration.ServiceConfigurations.Select( BuildHost ).ToList();
        }
    }
}