using System.Threading;

namespace Symbiote.Jackalope.Impl
{
    public interface IBroker
    {
        void Start(string queueName, ManualResetEvent reset);
        void Stop();
    }
}