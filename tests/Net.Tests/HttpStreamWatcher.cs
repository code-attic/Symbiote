using System;
using System.Threading;

namespace Net.Tests
{
    public class HttpStreamWatcher : IObserver<string>, IDisposable
    {
        protected bool MessageReceived { get; set; }
        protected string LastMessage { get; set; }

        public void OnNext(string value)
        {
            MessageReceived = true;
            LastMessage = value;
        }

        public string GetMessage()
        {
            while(!MessageReceived)
            {
                Thread.Sleep(3000);
            }
            MessageReceived = false;
            return LastMessage;
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}