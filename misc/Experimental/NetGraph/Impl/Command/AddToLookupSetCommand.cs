using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class AddToLookupSetCommand : NetGraphCommand<bool>
    {
        protected string KeyNodeId { get; set; }
        protected string RelatedNodeId { get; set; }

        public bool AddNode(IRedisClient redisClient)
        {
            var rslt = redisClient.SAdd(KeyNodeId, RelatedNodeId);
            return rslt;
        }

        public AddToLookupSetCommand(string keyNodeId, string relatedNodeId)
        {
            KeyNodeId = keyNodeId;
            RelatedNodeId = relatedNodeId;
            Command = AddNode;
        }

    }
}
