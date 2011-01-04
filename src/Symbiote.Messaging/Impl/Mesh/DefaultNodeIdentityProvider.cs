using System;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class DefaultNodeIdentityProvider
        : INodeIdentityProvider
    {
        public string Identity { get; protected set; }

        public DefaultNodeIdentityProvider( )
        {
            Identity = Guid.NewGuid().ToString();
        }
    }
}