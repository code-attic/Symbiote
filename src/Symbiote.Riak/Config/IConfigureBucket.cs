namespace Symbiote.Riak.Config
{
    public interface IConfigureBucket
    {
        IConfigureBucket Assign<T>();
        IConfigureBucket NodesForQuorumRead( uint nodes );
        IConfigureBucket NodesForQuorumWrite( uint nodes );
        IConfigureBucket WaitOnWritesBeforeAck();

    }
}