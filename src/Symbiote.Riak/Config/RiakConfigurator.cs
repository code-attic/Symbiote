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
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfigurator
    {
        public RiakConfiguration Configuration { get; protected set; }

        public RiakConfigurator AddLocalNode()
        {
            Configuration.Nodes.Add( new RiakNode() );
            return this;
        }

        public RiakConfigurator AddNode( Action<RiakNode> riakNodeConfiguration )
        {
            var node = new RiakNode();
            riakNodeConfiguration( node );
            Configuration.Nodes.Add( node );
            return this;
        }

        public RiakConfigurator RegisterBucket( string bucketName, Action<IConfigureBucket> assignments )
        {
            var assignment = new BucketConfigurator( bucketName );
            assignments( assignment );
            assignment.LoadDictionary( Configuration.RegisteredBuckets );
            return this;
        }

        public RiakConfigurator()
        {
            Configuration = new RiakConfiguration();
        }
    }
}