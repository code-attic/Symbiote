/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Symbiote.Core;
using Symbiote.Riak.Impl.ProtoBuf.Connection;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    public abstract class RiakCommand<TCommand, TResult>
        where TCommand : class
    {
        protected IConnectionProvider ConnectionProvider { get; set; }

        public virtual TResult Execute()
        {
            using(var handle = ConnectionProvider.Acquire())
            {
                var result = handle.Connection.Send(this as TCommand);
                if (result is Error)
                {
                    throw new RiakException(result as Error);
                }
                else
                    return (TResult) result;
            }
        }

        protected RiakCommand()
        {
            ConnectionProvider = Assimilate.GetInstanceOf<IConnectionProvider>();
        }
    }
}
