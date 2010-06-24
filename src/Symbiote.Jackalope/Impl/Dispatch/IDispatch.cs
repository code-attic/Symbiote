using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Symbiote.Jackalope.Impl
{
    public interface IDispatch
    {
        bool CanHandle(object payload);
        IEnumerable<Type> Handles { get; }
        void Dispatch(Envelope envelope);
    }

    public interface IDispatch<TMessage> : IDispatch
    {
    }
}