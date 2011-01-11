using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Riak.Impl;
using Symbiote.StructureMap;
using Symbiote.Riak;

namespace Riak.Tests
{
    public class with_assimilate
    {
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Riak( x => x.AddNode( r => r.Address( "192.168.1.105" ).ForProtocolBufferPort( 8081 ) ) );
        };
    }

    public class with_riak_server
        : with_assimilate
    {
        public static IRiakServer Server { get; set; }

        private Establish context = () =>
        {
            Server = Assimilate.GetInstanceOf<IRiakServer>();
        };
    }

    public class when_pinging_riak
        : with_riak_server
    {
        public static bool Success { get; set; }

        private Because of = () =>
        {
            Success = Server.Ping();
        };
        
        private It should_ping = () => Success.ShouldBeTrue();
    }

    public class when_setting_client_id
        : with_riak_server
    {
        public static string Id;

        private Because of = () =>
        {
            Server.SetClientId( "test" );
            Id = Server.GetClientId();
        };

        private It should_ping = () => Id.ShouldEqual( "test" );
    }
}
