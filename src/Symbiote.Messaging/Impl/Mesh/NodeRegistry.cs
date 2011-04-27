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
using Symbiote.Core.Extensions;
using Symbiote.Core.Hashing;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeRegistry
        : INodeRegistry
    {
        protected LoadBalancer<string> Nodes { get; set; }

        #region INodeRegistry Members

        public void AddNode( string nodeId )
        {
            Nodes.AddNode( nodeId, nodeId );
        }

        public string GetNodeFor<T>( T value )
        {
            return Nodes.GetNode( value );
        }

        public bool HasNode( string nodeId )
        {
            return Nodes.HasNode( nodeId );
        }

        public void RebalanceNode( string nodeId, decimal loadScore )
        {
            var total = Nodes.AliasCount*loadScore;
            Nodes.RebalanceNodeTo( nodeId, (int) total );
            "Rebalancing Node '{0}' to {1} virtual nodes."
                .ToDebug<INode>( nodeId, (int) total );
        }

        public void RemoveNode( string nodeId )
        {
            Nodes.RemoveNode( nodeId );
        }

        #endregion

        public NodeRegistry()
        {
            Nodes = new LoadBalancer<string>( 10000 );
        }
    }
}