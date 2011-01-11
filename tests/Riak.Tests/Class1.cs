using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Riak.Impl;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Request;
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

        private It should_retrieve_client_id = () => Id.ShouldEqual( "test" );
    }

    public class when_sending_ping_manually
    {
        public static TcpClient Client { get; set; }
        public static byte[] response { get; set; }

        private Because of = () =>
        {
            Client = new TcpClient("192.168.1.105", 8081);
            var stream = Client.GetStream();

            stream.Write( 
                BitConverter.GetBytes( IPAddress.HostToNetworkOrder( 1 ) )
                .Concat( new [] { Convert.ToByte( 1 ) })
                .ToArray(),
                0,
                5
                );
            response = new byte[5];
            stream.Read( response, 0, 5 );
        };

        private It should_get_response_of_two = () => response[4].ShouldEqual( Convert.ToByte( 2 ) );
    }

    public class when_sending_ping
    {
        public static TcpClient Client { get; set; }
        public static byte[] response { get; set; }

        private Because of = () =>
        {
            Client = new TcpClient("192.168.1.105", 8081);
            var stream = Client.GetStream();
            var riakSerializer = new RiakSerializer();
            var generated = riakSerializer.GetCommandBytes(new Ping());
            stream.Write(
                generated,
                0,
                generated.Length
                );
            response = new byte[5];
            stream.Read(response, 0, 5);
        };

        private It should_get_response_of_two = () => response[4].ShouldEqual(Convert.ToByte(2));
    }

    public class when_creating_ping_command
        : with_assimilate
    {
        public static byte[] Manual;
        public static byte[] Generated;

        private Because of = () =>
        {
            Manual = BitConverter.GetBytes( IPAddress.HostToNetworkOrder( 1 ) )
                .Concat( new[] { Convert.ToByte( 1 ) } )
                .ToArray();

            var riakSerializer = new RiakSerializer();
            Generated = riakSerializer.GetCommandBytes( new Ping() );
        };

        private It should_have_equivalent_approaches = () => Manual.ShouldEqual( Generated );
        private It should_have_the_same_bytes = () => Manual.SequenceEqual( Generated );
    }
}
