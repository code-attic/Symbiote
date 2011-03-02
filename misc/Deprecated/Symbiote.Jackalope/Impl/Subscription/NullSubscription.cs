using System;

namespace Symbiote.Jackalope.Impl
{
    public class NullSubscription : ISubscription
    {
        public IObservable<Envelope> MessageStream
        {
            get { return null; }
        }

        public void Dispose()
        {
            
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public bool Starting
        {
            get { return false; }
        }

        public bool Started
        {
            get { return false; }
        }

        public bool Stopping
        {
            get { return false; }
        }

        public bool Stopped
        {
            get { return false; }
        }

        public NullSubscription(string queueName)
        {
        }
    }
}