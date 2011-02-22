using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Persistence;
using Symbiote.Core.Serialization;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using Symbiote.StructureMapAdapter;
using Symbiote.Riak;

namespace Riak.Tests
{
    public class with_assimilate
    {
        //public static string Ip = "10.15.198.214";
        //public static string Ip = "10.15.199.62";
        //public static string Ip = "10.15.198.71";
        public static string Ip = "192.168.1.105";

        private Establish context = () =>
        {
            Assimilate
                .Initialize()
                .Riak( x => x.AddNode( r => r.Address( Ip ).ForProtocolBufferPort( 8087 ) ) );
        };
    }

    public class with_riak_server
        : with_assimilate
    {
        public static IRiakClient Client { get; set; }
        public static IKeyValueStore KeyValues { get; set; }

        private Establish context = () =>
        {
            Client = Assimilate.GetInstanceOf<IRiakClient>();
            KeyValues = Assimilate.GetInstanceOf<IKeyValueStore>();
        };
    }

    public class when_writing_and_retrieving_value_with_server
        : with_riak_server
    {
        public static string value;

        private Because of = () =>
        {
            var document = new Document<string>("this is a string", null);
            document.ContentType = "text/plain";

            Client.Delete<string>("test", "a", 1);
            Client.Persist("test", "a", null, document, 1, 1);
            value = Client.Get<string>( "test", "a", 1 ).Value;
        };
        
        private It should_be_incremented = () => value.ShouldEqual("this is a string");
    }

    public class when_using_key_value_store
        : with_riak_server
    {
        public static string value;

        private Because of = () =>
        {
            var persisted = KeyValues.Persist( "test", "this is a test" );
            KeyValues.Persist("test", "Actually, this is NOT a test...");
            value = KeyValues.Get<string>("test");
            var deleted = KeyValues.Delete<string>( "test" );
        };

        private It should_get_message = () => value.ShouldEqual("Actually, this is NOT a test...");
    }

    public class when_getting_a_bit_silly
        : with_riak_server
    {
        public static readonly string _speedtest = "speedTest{0}";
        public static Stopwatch watch { get; set; }

        private Because of = () =>
        {
            watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                KeyValues.Persist( _speedtest.AsFormat(i), i );
            }
            watch.Stop();
            for (int i = 0; i < 1000; i++)
            {
                KeyValues.Delete<int>(_speedtest.AsFormat(i));
            }
        };
        
        private It should_persist_1000_in_1_second = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1000 );
    }

    public class when_pinging_riak
        : with_riak_server
    {
        public static bool Success { get; set; }

        private Because of = () =>
        {
            Success = Client.Ping();
        };
        
        private It should_ping = () => Success.ShouldBeTrue();
    }

    public class when_setting_client_id
        : with_riak_server
    {
        public static string Id;

        private Because of = () =>
        {
            Client.SetClientId( "test" );
            Id = Client.GetClientId();
        };

        private It should_retrieve_client_id = () => Id.ShouldEqual( "test" );
    }

    public class when_sending_get_manually
        : with_assimilate
    {
        public static TcpClient Client { get; set; }
        public static byte[] response { get; set; }

        private Because of = () =>
        {
            Client = new TcpClient( Ip, 8087);
            var stream = Client.GetStream();

            var factory = new ProtoBufCommandFactory();
            var command = factory.CreateGet( "test", "a", 1 );
            var content = command.ToProtocolBuffer();

            var message = BitConverter
                .GetBytes( IPAddress.HostToNetworkOrder( 1 + content.Length ) )
                .Concat( new [] { Convert.ToByte( 9 ) })
                .Concat( content )
                .ToArray();

            stream.Write( 
                message,
                0,
                5 + content.Length
                );
            response = new byte[16 * 1024];
            //stream.Read( response, 0, 16 * 1024 );

            var serializer = new RiakSerializer();
            var result = serializer.GetResult( stream );
            var doc = result as GetResult;
        };

        private It should_get_response_of_two = () => response[4].ShouldEqual( Convert.ToByte( 10 ) );
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

    public class when_serializing_string
    {
        public static string result;

        private Because of = () =>
        {
            var original = "test";
            var bytes = original.ToProtocolBuffer();
            result = bytes.FromProtocolBuffer<string>();
        };

        private It should_get_string_back = () => result.ShouldEqual( "test" );
    }
}
