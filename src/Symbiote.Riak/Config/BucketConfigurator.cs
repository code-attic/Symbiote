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

namespace Symbiote.Riak.Config
{
    public class BucketConfigurator
        : IConfigureBucket
    {
        public string BucketName { get; set; }
        public List<Type> Types { get; set; }
        public BucketConfiguration BucketConfiguration { get; set; }

        public IConfigureBucket Assign<T>()
        {
            Types.Add( typeof( T ) );
            return this;
        }

        public IConfigureBucket NodesForQuorumRead( uint nodes )
        {
            BucketConfiguration.QuorumReadNodes = nodes;
            return this;
        }

        public IConfigureBucket NodesForQuorumWrite( uint nodes )
        {
            BucketConfiguration.QuorumWriteNodes = nodes;
            return this;
        }

        public IConfigureBucket WaitOnWritesBeforeAck()
        {
            BucketConfiguration.WaitOnWrites = true;
            return this;
        }

        public void LoadDictionary( IDictionary<Type, BucketConfiguration> lookup )
        {
            Types.ForEach( x => lookup[x] = BucketConfiguration );
        }

        public BucketConfigurator( string bucketName )
        {
            BucketName = bucketName;
            Types = new List<Type>();
        }
    }
}