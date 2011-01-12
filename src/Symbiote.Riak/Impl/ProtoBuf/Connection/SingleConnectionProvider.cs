namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class SingleConnectionProvider
        : IConnectionProvider
    {
        protected IConnectionFactory Factory { get; set; }

        public IConnectionHandle Acquire()
        {
            return new SingleConnectionHandle( Factory.GetConnection() );
        }

        public SingleConnectionProvider( IConnectionFactory factory )
        {
            Factory = factory;
        }
    }
}