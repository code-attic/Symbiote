using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retlang.Channels;
using Retlang.Fibers;
using Symbiote.Jackalope;

namespace Jackalope.Tests.RetLanging
{
    public interface IFiberFactory
    {
        IFiber CreateFiber();
    }

    public interface IChannelFactory
    {
        IChannel<TMessage> CreateChannel<TMessage>();
    }

    public class CorrelationChannel<TMessage>
        where TMessage : ICorrelate
    {
        protected ConcurrentDictionary<string, IFiber> Fibers { get; set; }
        protected IFiberFactory FiberFactory { get; set; }
        protected Action<TMessage> Handle { get; set; }

        public void HandleWith(Action<TMessage> handleMessage)
        {
            Handle = handleMessage;
        }

        public bool Publish(TMessage message)
        {
            var correlationId = message.CorrelationId;
            if(!Fibers.ContainsKey(correlationId))
            {
                SubscribeToId(correlationId);
            }
            return false;
        }

        public IDisposable Subscribe(IFiber fiber, Action<TMessage> receive)
        {
            throw new NotImplementedException();
        }

        public IDisposable SubscribeToBatch(IFiber fiber, Action<IList<TMessage>> receive, int intervalInMs)
        {
            throw new NotImplementedException();
        }

        public IDisposable SubscribeToKeyedBatch<K>(IFiber fiber, Converter<TMessage, K> keyResolver, Action<IDictionary<K, TMessage>> receive, int intervalInMs)
        {
            throw new NotImplementedException();
        }

        public IDisposable SubscribeToLast(IFiber fiber, Action<TMessage> receive, int intervalInMs)
        {
            throw new NotImplementedException();
        }

        public void ClearSubscribers()
        {
            throw new NotImplementedException();
        }

        public IDisposable SubscribeOnProducerThreads(IProducerThreadSubscriber<TMessage> subscriber)
        {
            throw new NotImplementedException();
        }

        public void SubscribeToId(string correlationId)
        {
            var fiber = FiberFactory.CreateFiber();
            
        }


        public CorrelationChannel(IFiberFactory fiberFactory)
        {
            FiberFactory = fiberFactory;
            Fibers = new ConcurrentDictionary<string, IFiber>();
        }
    }

    public class Matrix<TKey, TMessage>
    {
        
        
    }
}
