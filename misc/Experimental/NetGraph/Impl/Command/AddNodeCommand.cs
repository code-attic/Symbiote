using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class AddNodeCommand: NetGraphCommand<bool>
    {
        protected string NodeId { get; set; }

        public bool AddNode(IRedisClient redisClient)
        {
            //var rslt = redisClient.HSet(NodeId, GraphHashKey.ObjectType.ToString(), GraphObjectType.Node.ToString()); 
            //if (rslt)
            //{
                var rslt = redisClient.RPush(KeyProvider.NodeMasterKey(), NodeId);
            //}
            return rslt;
        }

        public AddNodeCommand(string nodeId)
        {
            NodeId = nodeId;
            Command = AddNode;
        }

    }
}
