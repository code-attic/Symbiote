using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetGraph
{
    public interface INetGraphClient
    {
        bool AddNode<T>(string id, T node);
        bool AddNode(string id);
        string AddRelationship(string nodeId1, string nodeId2, string relationshipType);
        string AddRelationship(string nodeId1, string nodeId2, string relationshipType, bool isUndirected);
        IEnumerable<string> GetRelatedNodes(string nodeId, EdgeDirection direction);
        IEnumerable<string> GetNodesWithAttributes(IEnumerable<Tuple<string,string>> attributes);
        bool AddToLookupSet(string keyNodeId, string relatedNodeId);
        bool AddAttribute(string attributeType, string attributeKey, string nodeId);
        IEnumerable<string> GetCommonNodes(IEnumerable<string> keyIds);
        IEnumerable<string> GetStartNodesForRelationshipType(string relationshipType);
        string[] GetMasterAttributeKeysForType(string attributeType);
    }
}
