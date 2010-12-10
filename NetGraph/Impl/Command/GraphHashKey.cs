using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetGraph.Impl.Command
{
    public enum GraphHashKey
    {
        ObjectType,
        RelationshpType,
        ObjectSerialization,
        EdgeStartNode,
        EdgeEndNode
    }

    public enum GraphObjectType
    {
        Node,
        Edge
    }

}
