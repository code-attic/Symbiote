using System.Collections.Generic;

namespace Symbiote.Eidetic.Config
{
    public interface IMemcachedConfig
    {
        int MinPoolSize { get; set; }
        int MaxPoolSize { get; set; }
        int Timeout { get; set; }
        int DeadTimeout { get; set; }
        IEnumerable<MemcachedServer> Servers { get; }
    }
}