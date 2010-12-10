using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;
using Symbiote.Core.Extensions;

namespace NetGraph.Impl.Command
{
    public class GetStartNodesForRelationshipTypeCommand : NetGraphCommand<IEnumerable<string>>
    {
        protected string RelationshipType { get; set; }

        public IEnumerable<string> GetNodes(IRedisClient redisClient)
        {
            var edges = redisClient.SMembers<string>(KeyProvider.RelationshipKey(RelationshipType));
            var nodeDict = new Dictionary<string, string>();
            var nodes = new List<string>();
            edges.ForEach( e =>
                {
                    var eed = redisClient.HGet<string>(e, GraphHashKey.EdgeStartNode.ToString());
                    if (!nodeDict.ContainsKey(eed))
                    {
                        nodeDict.Add(eed, null);
                        nodes.Add(eed);
                    }
                }
                );
            nodeDict = null;
            return nodes;
        }

        public GetStartNodesForRelationshipTypeCommand(string relationshipType)
        {
            RelationshipType = relationshipType;
            Command = GetNodes;
        }

    }
}
