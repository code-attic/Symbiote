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
using System.Configuration;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedServer : ConfigurationElement
    {
        [ConfigurationProperty( "address", IsRequired = true )]
        public string Address
        {
            get { return this["address"].ToString(); }
            set { this["address"] = value; }
        }

        [ConfigurationProperty( "port", IsRequired = true )]
        public int Port
        {
            get { return (int) this["port"]; }
            set { this["port"] = value; }
        }
    }
}