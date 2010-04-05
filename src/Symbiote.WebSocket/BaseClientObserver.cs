using System;

namespace Symbiote.WebSocket
{
    public abstract class BaseClientObserver :
        IObserver<Tuple<string,string>>,
        IDisposable
    {
        public Action<Tuple<string, string>> MessageReceived { get; set; }

        public virtual void OnNext(Tuple<string, string> value)
        {
            if(MessageReceived != null)
                MessageReceived(value);
        }

        public virtual void OnError(Exception error)
        {
            
        }

        public virtual void OnCompleted()
        {
            
        }

        protected BaseClientObserver(Action<Tuple<string, string>> messageReceived)
        {
            MessageReceived = messageReceived;
        }

        public void Dispose()
        {
            OnCompleted();
            MessageReceived = null;
        }
    }
}
