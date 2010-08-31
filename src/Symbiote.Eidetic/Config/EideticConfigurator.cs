using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Enyim.Caching.Configuration;
using Symbiote.Core;
using Symbiote.Core.Utility;
using Symbiote.Eidetic.Impl;

namespace Symbiote.Eidetic.Config
{
    public class EideticConfigurator
    {
        public MemcachedClientConfiguration Configuration { get; set; }

        public EideticConfigurator MinPoolSize(int minPoolSize)
        {
            Configuration.SocketPool.MinPoolSize = minPoolSize;
            return this;
        }

        public EideticConfigurator MaxPoolSize(int maxPoolSize)
        {
            Configuration.SocketPool.MaxPoolSize = maxPoolSize;
            return this;
        }

        public EideticConfigurator TimeOut(TimeSpan timeOut)
        {
            Configuration.SocketPool.ConnectionTimeout = timeOut;
            return this;
        }

        public EideticConfigurator DeadTimeOut(TimeSpan deadTimeOut)
        {
            Configuration.SocketPool.DeadTimeout = deadTimeOut;
            return this;
        }

        public EideticConfigurator ReceiveTimeOut(TimeSpan receiveTimeOut)
        {
            Configuration.SocketPool.ReceiveTimeout = receiveTimeOut;
            return this;
        }

        public EideticConfigurator AddServer(string server, int port)
        {
            Configuration.Servers.Add(GetEndPoint(server, port));
            return this;
        }

        public EideticConfigurator AddLocalServer()
        {
            Configuration.Servers.Add(GetEndPoint("localhost", 11211));
            return this;
        }

        public EideticConfigurator UseForDistributedLockManagement()
        {
            Assimilate.Dependencies(x => x.For<ILockManager>().Use<CacheLockManager>());
            return this;
        }

        public EideticConfigurator()
        {
            Configuration = new DefaultMemcachedConfiguration();
        }

        private IPEndPoint GetEndPoint(string address, int port)
        {
            var host = new IPHostEntry();
            host.HostName = address;
            var ipAddress = Dns.GetHostAddresses(address).First();
            return new IPEndPoint(ipAddress, port);
        }
    }
}
