using System.Collections.Generic;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedDefaults : IMemcachedConfig
    {
        private int _minPoolSize = 10;
        private int _maxPoolSize = 100;
        private int _timeout = 10;
        private int _deadTimeout = 30;

        public int MinPoolSize
        {
            get { return _minPoolSize; }
            set { _minPoolSize = value; }
        }
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set { _maxPoolSize = value; }
        }
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public int DeadTimeout
        {
            get { return _deadTimeout; }
            set { _deadTimeout = value; }
        }
        public IEnumerable<MemcachedServer> Servers
        {
            get
            {
                yield return new MemcachedServer()
                             {
                                 Address = "localhost",
                                 Port = 11211
                             };
            }   
        }
    }
}