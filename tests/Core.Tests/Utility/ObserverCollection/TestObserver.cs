using System;

namespace Core.Tests.Utility.ObserverCollection
{
    public class TestObserver : IObserver<string>
    {
        public bool GotEvent { get; set; }
        public bool GotError { get; set; }
        public bool GotCompletion { get; set; }

        public void OnNext( string value )
        {
            GotEvent = true;
        }

        public void OnError( Exception error )
        {
            GotError = true;
        }

        public void OnCompleted()
        {
            GotCompletion = true;
        }
    }
}