using System;

namespace Symbiote.WebSocket
{
    public class WebSocketObserver : 
        IObserver<Tuple<string, string>>,
        IDisposable
    {
        public Action<Tuple<string, string>> MessageReceived { get; set; }
        public Action<string, Exception> ClientError { get; set; }
        public Action<string> ClientDisconnect { get; set; }
        
        public string ClientId { get; set; }

        public virtual void OnNext(Tuple<string, string> value)
        {
            try
            {
                if(MessageReceived != null)
                    MessageReceived(value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public virtual void OnError(Exception error)
        {
            if(ClientError != null)
                ClientError(ClientId, error);
        }

        public virtual void OnCompleted()
        {
            if(ClientDisconnect != null)
                ClientDisconnect(ClientId);
        }

        public void Dispose()
        {
            OnCompleted();
            MessageReceived = null;
            ClientError = null;
            ClientDisconnect = null;
        }
    }
}