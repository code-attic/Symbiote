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
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    [Serializable, DataContract( Name = "RpbBucketProps" )]
    public class BucketProperties
    {
        [DataMember( Order = 1, IsRequired = false, Name = "n_val" )]
        public uint NValue { get; set; }

        [DataMember( Order = 2, IsRequired = false, Name = "allow_mult" )]
        public bool AllowMultiple { get; set; }
    }
}