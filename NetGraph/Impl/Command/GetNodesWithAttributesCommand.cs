using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class GetNodesWithAttributesCommand<T> : NetGraphCommand<IEnumerable<T>>
    {
        protected IEnumerable<Tuple<string, string>> Attributes { get; set; }

        public IEnumerable<T> GetNodes(IRedisClient redisClient)
        {
            var rslt = redisClient.SInter<T>(Attributes.Select(x => KeyProvider.AttributeMasterKey(x.Item1, x.Item2)));
            return rslt;
        }

        public GetNodesWithAttributesCommand(IEnumerable<Tuple<string,string>> attributes)
        {
            Attributes = attributes;
            Command = GetNodes;
        }

    }
}
