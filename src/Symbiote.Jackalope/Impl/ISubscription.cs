using System;

namespace Symbiote.Jackalope.Impl
{
    public interface ISubscription: IDisposable
    {
        void Start();
        void Stop();
        bool Starting { get; }
        bool Started { get; }
        bool Stopping { get; }
        bool Stopped { get; }
    }
}