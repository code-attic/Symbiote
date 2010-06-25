using System;
using System.Collections.Generic;

namespace Symbiote.Jackalope.Impl.Dispatch
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