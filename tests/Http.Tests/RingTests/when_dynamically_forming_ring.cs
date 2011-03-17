using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Utility;
using Symbiote.Http.NetAdapter.SocketListener;

namespace Http.Tests.RingTests {
    
    public class MockAdapter
        : IClientSocketAdapter
    {
        public static int Total = 1;

        private string _id;
        public string Id
        {
            get 
            { 
                _id = _id ?? Total++.ToString();
                return _id;
            }
        }

        public Action<string> Remove { get; set; }

        public void Close()
        {
            
        }

        public bool Read()
        {
            return false;
        }

        public MockAdapter()
        {
            var tmp = Id;
        }
    }

    public abstract class with_root
    {
        public static ClientSocketNode Root;
        private Establish context = () => 
        { 
            Root = new ClientSocketNode();
        };
    }
    
    public class when_dynamically_forming_ring 
        : with_root
    {
        public static ClientSocketNode Node1;
        public static ClientSocketNode Node2;
        public static ClientSocketNode Node3;
        public static ClientSocketNode Node4;
        public static ClientSocketNode Node5;

        private Because of = () => 
        {
            Node1 = Root.Add( new MockAdapter() );
            Node2 = Root.Add( new MockAdapter() );
            Node3 = Root.Add( new MockAdapter() );
            Node4 = Root.Add( new MockAdapter() );
            Node5 = Root.Add( new MockAdapter() );
        };

        private It root_should_have_next = () => Root.Next.Id.ShouldEqual( "1" );
        private It root_should_have_prev = () => Root.Previous.Id.ShouldEqual( "5" );
        private It node1_should_have_next = () => Node1.Next.Id.ShouldEqual( "2" );
        private It node1_should_have_prev = () => Node1.Previous.Id.ShouldEqual( "" );
        private It node2_should_have_next = () => Node2.Next.Id.ShouldEqual( "3" );
        private It node2_should_have_prev = () => Node2.Previous.Id.ShouldEqual( "1" );
        private It node3_should_have_next = () => Node3.Next.Id.ShouldEqual( "4" );
        private It node3_should_have_prev = () => Node3.Previous.Id.ShouldEqual( "2" );
        private It node4_should_have_next = () => Node4.Next.Id.ShouldEqual( "5" );
        private It node4_should_have_prev = () => Node4.Previous.Id.ShouldEqual( "3" );
        private It node5_should_have_next = () => Node5.Next.Id.ShouldEqual( "" );
        private It node5_should_have_prev = () => Node5.Previous.Id.ShouldEqual( "4" );
    }

    public class when_tearing_down_ring
        : with_root
    {
        public static ClientSocketNode Node1;
        public static ClientSocketNode Node2;
        public static ClientSocketNode Node3;
        public static ClientSocketNode Node4;
        public static ClientSocketNode Node5;

        private Because of = () => 
        {
            Node1 = Root.Add( new MockAdapter() );
            Node2 = Root.Add( new MockAdapter() );
            Node3 = Root.Add( new MockAdapter() );
            Node4 = Root.Add( new MockAdapter() );
            Node5 = Root.Add( new MockAdapter() );

            Node3.Delete( "" );
        };

        private It root_should_have_next = () => Root.Next.Id.ShouldEqual( "1" );
        private It root_should_have_prev = () => Root.Previous.Id.ShouldEqual( "5" );
        private It node1_should_have_next = () => Node1.Next.Id.ShouldEqual( "2" );
        private It node1_should_have_prev = () => Node1.Previous.Id.ShouldEqual( "" );
        private It node2_should_have_next = () => Node2.Next.Id.ShouldEqual( "4" );
        private It node2_should_have_prev = () => Node2.Previous.Id.ShouldEqual( "1" );
        private It node4_should_have_next = () => Node4.Next.Id.ShouldEqual( "5" );
        private It node4_should_have_prev = () => Node4.Previous.Id.ShouldEqual( "2" );
        private It node5_should_have_next = () => Node5.Next.Id.ShouldEqual( "" );
        private It node5_should_have_prev = () => Node5.Previous.Id.ShouldEqual( "4" );
    }

    public class when_concurrently_forming_ring 
        : with_root
    {
        private static string result;
        private Because of = () => 
        {
            Enumerable.Range( 1, 5 )
                .AsParallel()
                .ForAll( x => Root.Add( new MockAdapter() ) );

            Thread.Sleep( 100 );
            result = Root.ToString();
        };

        private It root_should_have_next = () => result.Length.ShouldEqual( 10 );
    }
}
