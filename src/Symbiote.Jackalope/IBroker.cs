using System.Threading;

namespace Symbiote.Jackalope
{
    public interface IBroker
    {
        void Start(string queueName);
        void Stop();
    }
}