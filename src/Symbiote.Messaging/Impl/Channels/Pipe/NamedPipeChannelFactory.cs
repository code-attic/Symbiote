namespace Symbiote.Messaging.Impl.Channels
{
    public class NamedPipeChannelFactory
        : IChannelFactory
    {
        public IChannel CreateChannel( IChannelDefinition definition )
        {
            return new NamedPipeChannel( definition as NamedPipeChannelDefinition );
        }
    }
}