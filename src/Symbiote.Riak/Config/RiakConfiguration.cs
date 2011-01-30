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
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfiguration : IRiakConfiguration
    {
        public Dictionary<Type, BucketConfiguration> RegisteredBuckets { get; set; }
        public BucketConfiguration DefaultBucketSettings { get; set; }

        public List<RiakNode> Nodes { get; set; }
        public int ConnectionLimit { get; set; }

        public BucketConfiguration GetBucketForType<T>()
        {
            BucketConfiguration configuration = DefaultBucketSettings;
            return configuration;
        }

        public RiakConfiguration()
        {
            Nodes = new List<RiakNode>();
            DefaultBucketSettings = new BucketConfiguration();
            RegisteredBuckets = new Dictionary<Type, BucketConfiguration>();
            ConnectionLimit = 1;
        }
    }
}