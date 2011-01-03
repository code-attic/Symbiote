namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeRegistry
    {
        void AddNode( string nodeId );
        string GetNodeFor<T>(T value);
        bool HasNode( string nodeId );
        void RebalanceNode(string nodeId, decimal loadScore);
        void RemoveNode( string nodeId );
    }
}
