using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace NetGraph.Impl.Command
{
    public static class KeyProvider
    {
        private const string EDGE_FORWARD_KEY = "{0}:ForwardLink";
        private const string EDGE_BACKWARD_KEY = "{0}:BackwardLink";
        private const string EDGE_UNDIRECTED_KEY = "{0}:UndirectedLink";
        private const string RELATIONSHIP_TYPE_KEY = "Edge:RelationshipType:{0}";
        private const string ATTRIBUTE_MASTER_KEY = "AttributeMaster:{0}:{1}";
        private const string NODE_ATTRIBUTE_SET_KEY = "Attributes:{0}";
        private const string NODE_MASTER_KEY = "Nodes";

        public static string ForwardKey(string value)
        {
            return EDGE_FORWARD_KEY.AsFormat(value);
        }
        public static string BackwardKey(string value)
        {
            return EDGE_BACKWARD_KEY.AsFormat(value);
        }
        public static string UndirectedKey(string value)
        {
            return EDGE_UNDIRECTED_KEY.AsFormat(value);
        }
        public static string RelationshipKey(string value)
        {
            return RELATIONSHIP_TYPE_KEY.AsFormat(value);
        }
        public static string AttributeMasterKey(string attributeType, string attributekey)
        {
            return ATTRIBUTE_MASTER_KEY.AsFormat(attributeType, attributekey);
        }

        public static string NodeAttributeKey(string value)
        {
            return NODE_ATTRIBUTE_SET_KEY.AsFormat(value);
        }

        public static string NodeMasterKey()
        {
            return NODE_MASTER_KEY;
        }
    }
}
