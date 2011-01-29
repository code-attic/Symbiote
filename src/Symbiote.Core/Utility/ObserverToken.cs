using System;

namespace Symbiote.Core.Utility
{
    public class ObserverToken
        : IDisposable
    {
        public Guid Id { get; set; }
        public Action<Guid> Unsubscribe { get; set; }

        public ObserverToken( Action<Guid> unsubscribe )
        {
            Id = Guid.NewGuid();
            Unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            Unsubscribe( Id );
            Unsubscribe = null;
        }
    }
}
