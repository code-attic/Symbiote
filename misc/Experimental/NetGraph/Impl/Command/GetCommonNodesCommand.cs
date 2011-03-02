using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class GetCommonNodesCommand<T> : NetGraphCommand<IEnumerable<T>>
    {
        protected IEnumerable<string> Keys { get; set; }

        public IEnumerable<T> GetCommonNodes(IRedisClient redisClient)
        {
            var rslt = redisClient.SInter<T>(Keys);
            return rslt;
        }

        public GetCommonNodesCommand(IEnumerable<string> keys)
        {
            Keys = keys;
            Command = GetCommonNodes;
        }

    }
}
