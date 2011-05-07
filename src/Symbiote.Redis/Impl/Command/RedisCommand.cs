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
using System.Text;
using Symbiote.Core;
using Symbiote.Redis.Impl.Connection;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis.Impl.Command
{
    public abstract class RedisCommand<TReturn>
    {
        protected ICacheSerializer Serializer { get; set; }
        protected IConnectionProvider ConnectionProvider { get; set; }
        public Func<IConnection, TReturn> Command { get; protected set; }

        public byte[] Serialize<T>( T value )
        {
            return Serializer.Serialize( value );
        }


        public T Deserialize<T>(byte[] bytes)
        {
            return Serializer.Deserialize<T>( bytes );
        }

        public TReturn Execute()
        {
            TReturn value = default(TReturn);
            using( var handle = ConnectionProvider.Acquire() )
            {
                value = Command( handle.Connection );
            }
            return value;
        }

        protected RedisCommand()
        {
            Serializer = Assimilate.GetInstanceOf<IConditionalCacheSerializer>();
            ConnectionProvider = Assimilate.GetInstanceOf<IConnectionProvider>();
        }
    }
}