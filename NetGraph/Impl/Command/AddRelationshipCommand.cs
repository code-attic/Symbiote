using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;
using Symbiote.Core.Extensions;

namespace NetGraph.Impl.Command
{
    public class AddRelationshipCommand : NetGraphCommand<string>
    {
        protected string NodeId1 { get; set; }
        protected string NodeId2 { get; set; }
        protected string RelationshipType { get; set; }
        protected bool IsUndirected {get; set;}
        protected bool CreateRelationshipEntry { get; set; }
        

        public string AddRelationship(IRedisClient redisClient)
        {
            bool rslt = true;
            string relId = string.Empty;
            var sList = new List<Tuple<string, string>>();

            if (CreateRelationshipEntry)
            {
                relId = Guid.NewGuid().ToString();
                var relDict = new Dictionary<string, string>();
                relDict.Add(GraphHashKey.ObjectType.ToString(), GraphObjectType.Edge.ToString());
                relDict.Add(GraphHashKey.RelationshpType.ToString(), RelationshipType);
                relDict.Add(GraphHashKey.EdgeStartNode.ToString(), NodeId1);
                relDict.Add(GraphHashKey.EdgeEndNode.ToString(), NodeId2);
                rslt = redisClient.HSet(relId, relDict);
                sList.Add(new Tuple<string, string>(KeyProvider.RelationshipKey(RelationshipType), relId));
            }


            if (rslt)
            {
                if (IsUndirected)
                {
                    sList.Add(new Tuple<string, string>(KeyProvider.UndirectedKey(NodeId1), NodeId2));
                    sList.Add(new Tuple<string, string>(KeyProvider.UndirectedKey(NodeId2), NodeId1));
                }
                else
                {
                    sList.Add(new Tuple<string, string>(KeyProvider.ForwardKey(NodeId1), NodeId2));
                    sList.Add(new Tuple<string, string>(KeyProvider.BackwardKey(NodeId2), NodeId1));
                }
                var rsltList = redisClient.SAdd(sList);
                rsltList.ForEach( x =>
                    {
                        if (rslt)
                            rslt = x;
                    }
                    );
            }

            if (rslt)
                return relId;
            else
                return null;
        }

        public AddRelationshipCommand(string nodeId1, string nodeId2, string relationshipType, bool isUndirected, bool createRelationshipEntry)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
            IsUndirected = isUndirected;
            RelationshipType = relationshipType;
            CreateRelationshipEntry = createRelationshipEntry;
            Command = AddRelationship;
        }

    }
}
