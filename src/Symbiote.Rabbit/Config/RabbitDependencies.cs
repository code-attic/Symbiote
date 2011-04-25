using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Node;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Config
{
    public class RabbitDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            return x =>
                       {
                           x.For<RabbitConfiguration>()
                               .Use<RabbitConfiguration>()
                               .AsSingleton();
                           x.For<IConnectionManager>()
                               .Use<ConnectionManager>()
                               .AsSingleton();
                           x.For<IChannelProxyFactory>()
                               .Use<ChannelProxyFactory>();
                           x.For<IEndpointManager>()
                               .Use<EndpointManager>();
                           x.For<IConnectionManager>()
                               .Use<ConnectionManager>();
                           x.For<IEndpointIndex>()
                               .Use( new EndpointIndex() );
                           x.For<INodeChannelManager>()
                               .Use<RabbitNodeChannelManager>().AsSingleton();
                       };
        }
    }
}