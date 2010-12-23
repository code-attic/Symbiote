namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeRegistry
    {
        void AddNode( string nodeId );
        void RemoveNode( string nodeId );
        string GetNodeFor<T>( T value );
        void RebalanceNode( string nodeId, decimal loadScore );
    }
}
