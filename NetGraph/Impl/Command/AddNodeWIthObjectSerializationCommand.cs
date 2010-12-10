using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class AddNodeWithObjectSerializationCommand<T> : NetGraphCommand<bool>
    {
        protected string NodeId { get; set; }
        protected T Node { get; set; }

        public bool AddNode(IRedisClient redisClient)
        {
            //var rslt = redisClient.HSet(NodeId, GraphHashKey.ObjectType.ToString(), GraphObjectType.Node.ToString());
            //if (rslt)
            //{
                var rslt = redisClient.HSet(NodeId, GraphHashKey.ObjectSerialization.ToString(), Node);
            //}
            if (rslt)
            {
                rslt = redisClient.RPush(KeyProvider.NodeMasterKey(), NodeId);
            }
            return rslt;
        }

        public AddNodeWithObjectSerializationCommand(string nodeId, T node)
        {
            NodeId = nodeId;
            Node = node;
            Command = AddNode;
        }

    }
}
