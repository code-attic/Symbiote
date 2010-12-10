using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Redis;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Core.Extensions;
using NetGraph;
using System.Runtime.Serialization;

namespace NetGraphDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon(x => x
                    .DisplayName("NetGraph demo")
                    .Description("NetGraph demo")
                    .Name("NetGraph demo")
                    .Arguments(args))
                //.Redis(x => x.AddServer("10.15.199.65").LimitPoolConnections(5))
//                .Redis(x => x.AddServer("10.15.199.4").LimitPoolConnections(5))
                .Redis(x => x.AddServer("127.0.0.1").LimitPoolConnections(5))
                .NetGraph(x => x.Configuration.RedisClient = x.Configuration.RedisClient)
                .AddConsoleLogger<NetGraphDemo>(x => x.Debug().MessageLayout(m => m.Message().Newline()))
                .RunDaemon();
        }
    }

    public class NetGraphDemo
        : IDaemon
    {
        protected INetGraphClient NetGraphCli { get; set; }

        public void Start()
        {

            var netGraphMgr = new NetGraphMgr(NetGraphCli);

            "Starting writes to storage".ToDebug<NetGraphDemo>();
            netGraphMgr.LoadRedisFromFile();

            "Starting relationship lookups".ToDebug<NetGraphDemo>();
            netGraphMgr.LookupByEed();
            netGraphMgr.LookupByPd();
            netGraphMgr.LookupBySd();

            "Starting relationship lookups by attribute".ToDebug<NetGraphDemo>();
            netGraphMgr.LookupEedByAttributes();

            "Starting relationship lookups by attribute from db only".ToDebug<NetGraphDemo>();
            netGraphMgr.LookupEedByAttributesDbOnly();
        }

        public void Stop()
        {

        }

        public NetGraphDemo(INetGraphClient netGraphClient)
        {
            NetGraphCli = netGraphClient;
        }

    }

}