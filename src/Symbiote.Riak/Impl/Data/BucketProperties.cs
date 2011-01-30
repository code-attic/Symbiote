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
namespace Symbiote.Riak.Impl.Data
{
    public class BucketProperties
    {
        public uint NValue { get; set; }
        public bool AllowMultiple { get; set; }

        public ProtoBuf.BucketProperties ToProtoBuf()
        {
            return new ProtoBuf.BucketProperties
                       {
                           AllowMultiple = AllowMultiple,
                           NValue = NValue
                       };
        }

        public BucketProperties( uint nValue, bool allowMultiple )
        {
            NValue = nValue;
            AllowMultiple = allowMultiple;
        }
    }
}