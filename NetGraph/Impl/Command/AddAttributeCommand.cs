using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class AddAttributeCommand : NetGraphCommand<bool>
    {
        protected string AttributeKey { get; set; }
        protected string AttributeType { get; set; }
        protected string NodeId { get; set; }

        public bool AddNode(IRedisClient redisClient)
        {
            var rslt = redisClient.SAdd(KeyProvider.AttributeMasterKey(AttributeType, AttributeKey), NodeId);
            if (rslt)
            {
                rslt = redisClient.SAdd(KeyProvider.NodeAttributeKey(NodeId), KeyProvider.AttributeMasterKey(AttributeType, AttributeKey));
                	
            }
            return rslt;
        }

        public AddAttributeCommand(string attributeType, string attributeKey, string nodeId)
        {
            AttributeType = attributeType;
            AttributeKey = attributeKey;
            NodeId = nodeId;
            Command = AddNode;
        }

    }
}
