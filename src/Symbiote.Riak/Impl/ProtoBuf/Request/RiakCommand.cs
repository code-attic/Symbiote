using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
