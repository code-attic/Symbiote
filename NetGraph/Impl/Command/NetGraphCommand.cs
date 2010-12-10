using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGraph.Impl.Connection;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public abstract class NetGraphCommand<TReturn>
    {
        public Func<IRedisClient, TReturn> Command { get; protected set; }

        public TReturn Execute()
        {
            TReturn value = default(TReturn);
            using (var handle = ConnectionHandle.Acquire())
            {
                value = Command(handle.Client);
            }
            return value;
        }

//        protected NetGraphCommand()
//        {
////            Serializer = ServiceLocator.Current.GetInstance<ICacheSerializer>();
//        }
    }

}
