using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Symbiote.Eidetic.Impl;
using Symbiote.Core.Extensions;

namespace Symbiote.Eidetic.Config
{
    public class DefaultMemcachedConfiguration : MemcachedClientConfiguration
    {
        public IList<IPEndPoint> ServerEndpoints
        {
            get { return Servers; }
        }

        public int MinPoolSize
        {
            get { return SocketPool.MinPoolSize; }
            set { SocketPool.MinPoolSize = value; }
        }

        public int MaxPoolSize
        {
            get { return SocketPool.MaxPoolSize; }
            set { SocketPool.MaxPoolSize = value; }
        }

        public TimeSpan TimeOut
        {
            get { return SocketPool.ConnectionTimeout; }
            set { SocketPool.ConnectionTimeout = value; }
        }

        public TimeSpan DeadTimeout
        {
            get { return SocketPool.DeadTimeout; }
            set { SocketPool.DeadTimeout = value; }
        }

        public void Initialize()
        {
            var section = 
                ConfigurationManager.GetSection("memcached") as IMemcachedConfig ?? new MemcachedDefaults();

            if (section == null)
            {
                SocketPool.MinPoolSize = 10;
                SocketPool.MaxPoolSize = 100;
                SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 10);
                SocketPool.DeadTimeout = new TimeSpan(0, 0, 30);
            }

            MinPoolSize = section.MinPoolSize;
            MaxPoolSize = section.MaxPoolSize;
            TimeOut = TimeSpan.FromSeconds(section.Timeout);
            DeadTimeout = TimeSpan.FromSeconds(section.DeadTimeout);

            section
                .Servers
                .ForEach(server => 
                    ServerEndpoints.Add(GetEndPoint(server.Address, server.Port))
                );
        }

        private IPEndPoint GetEndPoint(string address, int port)
        {
            var host = new IPHostEntry();
            host.HostName = address;
            var ipAddress = Dns.GetHostAddresses(address).First();
            return new IPEndPoint(ipAddress, port);
        }

        public DefaultMemcachedConfiguration()
        {
            NodeLocator = typeof(DefaultNodeLocator);
            KeyTransformer = typeof(Base64KeyTransformer);
            Transcoder = typeof(DefaultTranscoder);
            Initialize();
        }
    }
}
