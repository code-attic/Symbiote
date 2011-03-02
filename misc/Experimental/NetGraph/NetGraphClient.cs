using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGraph;
using NetGraph.Impl.Config;
using NetGraph.Impl.Command;

namespace NetGraph
{
    public class NetGraphClient: INetGraphClient
    {
        protected NetGraphConfiguration Configuration { get; set; }
        public bool AddNode(string nodeId)
        {
            ValidateIdVal(nodeId);

            var command = new AddNodeCommand(nodeId);
            return command.Execute();

        }

        public bool AddNode<T>(string nodeId, T node)
        {
            ValidateIdVal(nodeId);

            var command = new AddNodeWithObjectSerializationCommand<T>(nodeId, node);
            return command.Execute();

        }

        public string AddRelationship(string nodeId1, string nodeId2, string relationshipType)
        {
            return AddRelationship(nodeId1, nodeId2, relationshipType, false);
        }

        public string AddRelationship(string nodeId1, string nodeId2, string relationshipType, bool isUndirected)
        {
            ValidateIdVal(nodeId1);
            ValidateIdVal(nodeId2);
            ValidateIdVal(relationshipType);

            var command = new AddRelationshipCommand(nodeId1, nodeId2, relationshipType, isUndirected, false);
            return command.Execute();
        }

        public bool AddToLookupSet(string keyNodeId, string relatedNodeId)
        {
            ValidateIdVal(keyNodeId);
            ValidateIdVal(relatedNodeId);

            var command = new AddToLookupSetCommand(keyNodeId, relatedNodeId);
            return command.Execute();

        }

        public bool AddAttribute(string attributeType, string attributeKey, string nodeId)
        {
            ValidateIdVal(attributeType);
            ValidateIdVal(attributeKey);
            ValidateIdVal(nodeId);

            var command = new AddAttributeCommand(attributeType, attributeKey, nodeId);
            return command.Execute();

        }

        public IEnumerable<string> GetRelatedNodes(string nodeId, EdgeDirection direction)
        {
            ValidateIdVal(nodeId);

            var command = new GetRelatedNodesCommand(nodeId, direction);
            return command.Execute();
            
        }

        public string[] GetMasterAttributeKeysForType(string attributeType)
        {
            ValidateIdVal(attributeType);

            var command = new GetMasterAttributeNodesByTypeCommand<string>(attributeType);
            return command.Execute();
            
        }

        public IEnumerable<string> GetNodesWithAttributes(IEnumerable<Tuple<string,string>> attributes)
        {
            var command = new GetNodesWithAttributesCommand<string>(attributes);
            return command.Execute();
        }

        public IEnumerable<string> GetCommonNodes(IEnumerable<string> keyIds)
        {
            var command = new GetCommonNodesCommand<string>(keyIds);
            return command.Execute();
        }

        public IEnumerable<string> GetStartNodesForRelationshipType(string relationshipType)
        {
            var command = new GetStartNodesForRelationshipTypeCommand(relationshipType);
            return command.Execute();
        }

        private static void ValidateIdVal(string id)
        {
            ValidateInputStringNotNull(id, "id");
        }

        private static void ValidateFieldVal(string field)
        {
            ValidateInputStringNotNull(field, "field");
        }

        private static void ValidateInputStringNotNull(string val, string exceptionString)
        {
            if (val == null)
                throw new ArgumentNullException(exceptionString);
        }
        private static void ValidateInputValueNotDefault<T>(T value)
        {
            if (ReferenceEquals(value, default(T)))
                throw new ArgumentNullException("value");
        }


        
        public NetGraphClient(NetGraphConfiguration configuration)
        {
            Configuration = configuration;
        }


    }
}
