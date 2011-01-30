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
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeConfiguration
        : INodeConfiguration
    {
        protected readonly string NODE_FORMAT = "node.{0}";

        #region INodeConfiguration Members

        public INodeIdentityProvider IdentityProvider { get; set; }
        public TimeSpan HealthMonitorFrequency { get; set; }
        public bool AsProxy { get; set; }

        public string MeshChannel { get; set; }
        public string NodeChannel { get; set; }

        public string GetNodeChannelForId( string nodeId )
        {
            return NODE_FORMAT.AsFormat( nodeId );
        }

        #endregion

        public NodeConfiguration( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
            MeshChannel = "mesh";
            NodeChannel = NODE_FORMAT.AsFormat( identityProvider.Identity );
            HealthMonitorFrequency = TimeSpan.FromSeconds( 1 );
        }
    }
}