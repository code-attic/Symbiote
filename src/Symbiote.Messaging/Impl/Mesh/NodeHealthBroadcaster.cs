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
    public class NodeHealthBroadcaster
        : INodeHealthBroadcaster
    {
        public INodeConfiguration Configuration { get; set; }
        public IBus Bus { get; set; }

        #region INodeHealthBroadcaster Members

        public void OnNext( NodeHealth value )
        {
            Bus.Publish( Configuration.MeshChannel, value );
        }

        public void OnError( Exception error )
        {
            // do nothing
        }

        public void OnCompleted()
        {
            // do nothing
        }

        #endregion

        public NodeHealthBroadcaster( INodeConfiguration configuration, IBus bus )
        {
            Configuration = configuration;
            Bus = bus;
        }
    }
}