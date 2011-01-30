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
using Symbiote.Core.Futures;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class Node
        : INode
    {
        public INodeRegistry Nodes { get; set; }
        public IBus Bus { get; set; }
        public INodeConfiguration Configuration { get; set; }

        #region INode Members

        public void Publish<T>( T message )
        {
            Publish( message, x => { } );
        }

        public void Publish<T>( T message, Action<IEnvelope> modifyEnvelope )
        {
            var channelName = GetChannelForMessage( message );
            try
            {
                Bus.Publish( channelName, message, modifyEnvelope );
            }
            catch ( Exception e )
            {
                // blame it all on the channel
                HandleNodeDeath( channelName );
                throw;
            }
        }

        public Future<R> Request<T, R>( T message )
        {
            return Request<T, R>( message, x => { } );
        }

        public Future<R> Request<T, R>( T message, Action<IEnvelope> modifyEnvelope )
        {
            var channelName = GetChannelForMessage( message );
            try
            {
                return Bus.Request<T, R>( channelName, message, modifyEnvelope );
            }
            catch ( Exception e )
            {
                // blame it all on the channel
                HandleNodeDeath( channelName );
                throw;
            }
        }

        #endregion

        public void HandleNodeDeath( string deadNode )
        {
            Nodes.RemoveNode( deadNode );
            Bus.Publish( Configuration.MeshChannel, new NodeDown {NodeId = deadNode} );
        }

        public string GetChannelForMessage<T>( T message )
        {
            return Configuration.GetNodeChannelForId( Nodes.GetNodeFor( message ) );
        }

        public Node( INodeRegistry nodes, IBus bus, INodeConfiguration configuration )
        {
            Nodes = nodes;
            Bus = bus;
            Configuration = configuration;
        }
    }
}