using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitChannel<TMessage>(this IBus bus, Action<RabbitEndpointFluentConfigurator> configurate)
            where TMessage : class
        {
            var endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            endpoints.ConfigureEndpoint<TMessage>(configurate);
            return bus;
        }

        public static void CommitChannelOf<TMessage>(this IBus bus)
            where TMessage : class
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor<TMessage>() as RabbitChannel<TMessage>;
            channel.Proxy.Channel.TxCommit();
        }

        public static void ReplyTo<TReply, TMessage>(this IBus bus, RabbitEnvelope<TMessage> message, TReply reply)
            where TMessage : class
            where TReply : class, IRouteByKey
        {
            reply.RoutingKey = message.ReplyToKey;
            bus.Send(message.ReplyToExchange, reply);
        }

        public static void RollbackChannelOf<TMessage>(this IBus bus)
            where TMessage : class
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor<TMessage>() as RabbitChannel<TMessage>;
            channel.Proxy.Channel.TxRollback();
        }
    }
}
