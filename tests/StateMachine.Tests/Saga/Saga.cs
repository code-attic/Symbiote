using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;

namespace StateMachine.Tests.Saga
{
    public interface IState
    {
        
    }

    public interface IHaveState
    {
        IState State { get; set; }
        void ChangeState(IState state);

    }

    public abstract class HandleMessageFor<TMessage, TSaga>
        : IMessageHandler<TMessage>
    {
        protected abstract void ProcessMessage(TMessage message);

        public void Process(TMessage message, IMessageDelivery messageDelivery)
        {
            
        }
    }

    public abstract class Saga : 
        IHaveState, 
        ICorrelate, 
        IObservable<IState>
    {
        protected ConcurrentBag<IObserver<IState>> observers { get; set; }

        public string CorrelationId { get; set; }
        public IState State { get; set; }


        public IDisposable Subscribe(IObserver<IState> observer)
        {
            observers.Add(observer);
            return new ObserverHandle();
        }

        public virtual void ChangeState(IState state)
        {
            State = state;
            observers.ForEach(x => x.OnNext(state));
        }

        protected Saga()
        {
            observers = new ConcurrentBag<IObserver<IState>>();
        }

        public class ObserverHandle
            : IDisposable
        {
            public void Dispose()
            {
                
            }
        }
    }

    
}
