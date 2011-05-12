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

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeChangeHandler :
        IHandle<NodeUp>,
        IHandle<NodeDown>,
        IHandle<NodeHealth>
    {
        protected INodeRegistry Registry { get; set; }
        protected INodeChannelManager NodeChannelManager { get; set; }

        public Action<IEnvelope> Handle( NodeDown message )
        {
            Registry.RemoveNode( message.NodeId );
            return x => x.Acknowledge();
        }

        public Action<IEnvelope> Handle( NodeHealth message )
        {
            try
            {
                var nodeId = message.NodeId;
                if ( nodeId != null )
                {
                    if ( !Registry.HasNode( nodeId ) )
                    {
                        NodeChannelManager.AddNewOutgoingChannel( nodeId );
                        Registry.AddNode( nodeId );
                    }
                    Registry.RebalanceNode( nodeId, message.LoadScore );
                }
                return x => x.Acknowledge();
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
                return x => x.Reject( e.ToString() );
            }
        }

        public Action<IEnvelope> Handle( NodeUp message )
        {
            Registry.AddNode( message.NodeId );
            return x => x.Acknowledge();
        }

        public NodeChangeHandler( INodeRegistry registry, INodeChannelManager nodeChannelManager )
        {
            NodeChannelManager = nodeChannelManager;
            Registry = registry;
        }
    }
}