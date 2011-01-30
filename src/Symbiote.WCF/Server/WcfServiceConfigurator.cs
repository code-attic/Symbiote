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
using System.ServiceModel.Channels;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceConfigurator<TContract>
        where TContract : class
    {
        protected IWcfServiceConfiguration<TContract> Configuration { get; set; }

        public IWcfServiceConfiguration<TContract> GetConfiguration()
        {
            return Configuration;
        }

        public WcfServiceConfigurator<TContract> Binding( Binding binding )
        {
            Configuration.Binding = binding;
            return this;
        }

        public WcfServiceConfigurator<TContract> DisableMetadataExchange()
        {
            Configuration.EnableHttpMetadataExchange = false;
            return this;
        }

        public WcfServiceConfigurator<TContract> MexAddress( string metadataExchangeUri )
        {
            Configuration.MetadataExchangeUri = metadataExchangeUri;
            return this;
        }

        public WcfServiceConfigurator<TContract> Timeout( TimeSpan timespan )
        {
            Configuration.Timeout = timespan.Milliseconds;
            return this;
        }

        public WcfServiceConfigurator()
        {
            Configuration = new WcfServiceConfiguration<TContract>();
        }
    }
}