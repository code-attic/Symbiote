using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetGraph
{
    public enum EdgeDirection
    {
        Forward,
        Backward,
        Undirected
    }

    public enum RelationshipType
    {
        Edge,
        CreditDebit,
        Offset
    }
}
