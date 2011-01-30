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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Symbiote.Core.Extensions;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceConfiguration<TContract> : IWcfServiceConfiguration<TContract>
        where TContract : class
    {
        public string Address { get; set; }
        public Binding Binding { get; set; }
        public int Timeout { get; set; }
        public bool EnableHttpMetadataExchange { get; set; }
        public string MetadataExchangeUri { get; set; }

        public Type ServiceType
        {
            get { return typeof( TContract ); }
        }

        public Type ContractType
        {
            get { return typeof( TContract ).GetInterfaces().First(); }
        }

        public void UseDefaults()
        {
            Address = "{0}".AsFormat( typeof( TContract ).Name );
            Binding = new BasicHttpBinding( BasicHttpSecurityMode.None );
            Timeout = 600;
            EnableHttpMetadataExchange = true;
        }

        public WcfServiceConfiguration()
        {
            UseDefaults();
        }
    }
}