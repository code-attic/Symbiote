using System;
using System.Threading;
using Symbiote.Core;

namespace Net.Tests
{
    public class HttpStreamWatcher : IObserver<string>, IDisposable
    {
        protected bool MessageReceived { get; set; }
        protected string LastMessage { get; set; }
        protected DelimitedBuilder Builder { get; set; }

        public void OnNext(string value)
        {
            MessageReceived = true;
            Builder.Append(value);
            LastMessage = value;
        }

        public string GetMessage()
        {
            while(!MessageReceived)
            {
                Thread.Sleep(3000);
            }
            MessageReceived = false;
            return Builder.ToString();
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

        public HttpStreamWatcher()
        {
            Builder = new DelimitedBuilder("\r\n");
        }
    }
}