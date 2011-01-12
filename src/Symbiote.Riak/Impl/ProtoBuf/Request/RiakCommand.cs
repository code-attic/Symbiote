using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Riak.Impl.ProtoBuf.Connection;

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
                return (TResult) handle.Connection.Send(this as TCommand);
            }
        }

        protected RiakCommand()
        {
            ConnectionProvider = Assimilate.GetInstanceOf<IConnectionProvider>();
        }
    }
}
