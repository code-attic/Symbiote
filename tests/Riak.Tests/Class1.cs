using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        //public static string Ip = "10.15.198.214";
        //public static string Ip = "10.15.199.62";
        public static string Ip = "192.168.1.105";

        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Riak( x => x.AddNode( r => r.Address( Ip ).ForProtocolBufferPort( 8081 ) ) );
        };
    }

    public class with_riak_server
        : with_assimilate
    {
        public static IRiakServer Server { get; set; }
        public static IKeyValueStore KeyValues { get; set; }

        private Establish context = () =>
        {
            Server = Assimilate.GetInstanceOf<IRiakServer>();
            KeyValues = Assimilate.GetInstanceOf<IKeyValueStore>();
        };
    }

    public class when_writing_and_retrieving_value
        : with_riak_server
    {
        public static int value;

        private Because of = () =>
        {
            KeyValues.Persist( "test", 100 );
            var value = KeyValues.Get<int>( "test" );
            value++;
            KeyValues.Persist( "test", value );
            value = KeyValues.Get<int>( "test" );
        };
        
        private It should_be_incremented = () => value.ShouldEqual( 101 );
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
        : with_assimilate
    {
        public static TcpClient Client { get; set; }
        public static byte[] response { get; set; }

        private Because of = () =>
        {
            Client = new TcpClient( Ip, 8081);
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
        : with_assimilate
    {
        public static TcpClient Client { get; set; }
        public static byte[] response { get; set; }

        private Because of = () =>
        {
            Client = new TcpClient( Ip, 8081);
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
