using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;
using Symbiote.Core.Extensions;

namespace NetGraph.Impl.Command
{
    public class GetRelatedNodesCommand : NetGraphCommand<IEnumerable<string>>
    {
        protected string NodeId { get; set; }
        protected EdgeDirection Direction { get; set; }

        public IEnumerable<string> GetRelatedNodes(IRedisClient redisClient)
        {
            string key = string.Empty;
            switch (Direction)
            {
                case EdgeDirection.Forward:
                    key = KeyProvider.ForwardKey(NodeId);
                    break;
                case EdgeDirection.Backward:
                    key = KeyProvider.BackwardKey(NodeId);
                    break;
                case EdgeDirection.Undirected:
                    key = KeyProvider.UndirectedKey(NodeId);
                    break;
            }
            return redisClient.SMembers<string>(key);
        }

        public GetRelatedNodesCommand(string nodeId, EdgeDirection direction)
        {
            NodeId = nodeId;
            Direction = direction;
            Command = GetRelatedNodes;
        }

    }
}
