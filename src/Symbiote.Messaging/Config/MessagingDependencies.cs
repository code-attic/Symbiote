using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Messaging.Impl;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Messaging.Config
{
    public class MessagingDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            return container =>
                       {
                           container.For<IBus>().Use<Bus>();
                           container.For<IChannelManager>().Use<ChannelManager>().AsSingleton();
                           container.For<IChannelIndex>().Use<ChannelIndex>().AsSingleton();
                           container.For<IDispatcher>().Use<DispatchManager>().AsSingleton();
                           container.For<ISubscriptionManager>().Use<SubscriptionManager>().AsSingleton();
                           container.For<INodeRegistry>().Use<NodeRegistry>().AsSingleton();
                           container.For<INodeIdentityProvider>().Use<DefaultNodeIdentityProvider>().AsSingleton();
                           container.For<INodeConfiguration>().Use<NodeConfiguration>().AsSingleton();
                           container.For<INode>().Use<Node>().AsSingleton();
                           container.For<INodeHealthMonitor>().Use<NodeHealthMonitor>().AsSingleton();
                       };
        }
    }
}