using System;
using Machine.Specifications;
using Symbiote.Net;

namespace Http.Tests
{
	public class with_root_socket_node
	{
		public static LoopNode root;
		
		Establish context = () => 
		{
			root = new LoopNode( null ) { Id = 1 };
		};
	}
	
	public class when_adding_sockets_to_loop
		: with_root_socket_node
	{
		Because of = () => 
		{
			var node1 = new LoopNode( null ) { Id = 2 };
			var node2 = new LoopNode( null ) { Id = 3 };
			var node3 = new LoopNode( null ) { Id = 4 };
			var node4 = new LoopNode( null ) { Id = 5 };
			
			root.AddNode( node1 );
			root.AddNode( node2 );
			root.AddNode( node3 );
			root.AddNode( node4 );
		};
		
		It should_add_nodes_in_order = () =>
		{
			var current = root.Next;
			current.Id.ShouldEqual( 2 );
			current.Next.Id.ShouldEqual( 3 );
			current.Next.Next.Id.ShouldEqual( 4 );
			current.Next.Next.Next.Id.ShouldEqual( 5 );
			current.Next.Next.Next.Next.Id.ShouldEqual( 1 );
		};
	}
	
	public class when_removing_last_socket_from_loop
		: with_root_socket_node
	{
		Because of = () =>
		{
			var node1 = new LoopNode( null ) { Id = 2 };
			var node2 = new LoopNode( null ) { Id = 3 };
			var node3 = new LoopNode( null ) { Id = 4 };
			var node4 = new LoopNode( null ) { Id = 5 };
			
			root.AddNode( node1 );
			root.AddNode( node2 );
			root.AddNode( node3 );
			root.AddNode( node4 );
			
			root.Next.Next.Next.Next.Remove();
		};
		
		It should_maintain_nodes_in_order = () =>
		{
			var current = root.Next;
			current.Id.ShouldEqual( 2 );
			current.Next.Id.ShouldEqual( 3 );
			current.Next.Next.Id.ShouldEqual( 4 );
			current.Next.Next.Next.Id.ShouldEqual( 1 );
		};
	}
	
	public class when_removing_third_socket_from_loop
		: with_root_socket_node
	{
		Because of = () =>
		{
			var node1 = new LoopNode( null ) { Id = 2 };
			var node2 = new LoopNode( null ) { Id = 3 };
			var node3 = new LoopNode( null ) { Id = 4 };
			var node4 = new LoopNode( null ) { Id = 5 };
			
			root.AddNode( node1 );
			root.AddNode( node2 );
			root.AddNode( node3 );
			root.AddNode( node4 );
			
			root.Next.Next.Remove();
		};
		
		It should_maintain_nodes_in_order = () =>
		{
			var current = root.Next;
			current.Id.ShouldEqual( 2 );
			current.Next.Id.ShouldEqual( 4 );
			current.Next.Next.Id.ShouldEqual( 5 );
			current.Next.Next.Next.Id.ShouldEqual( 1 );
		};
	}
	
	public class when_removing_two_sockets_from_loop
		: with_root_socket_node
	{
		Because of = () =>
		{
			var node1 = new LoopNode( null ) { Id = 2 };
			var node2 = new LoopNode( null ) { Id = 3 };
			var node3 = new LoopNode( null ) { Id = 4 };
			var node4 = new LoopNode( null ) { Id = 5 };
			
			root.AddNode( node1 );
			root.AddNode( node2 );
			root.AddNode( node3 );
			root.AddNode( node4 );
			
			root.Next.Next.Remove();
			root.Next.Remove();
		};
		
		It should_maintain_nodes_in_order = () =>
		{
			var current = root.Next;
			current.Id.ShouldEqual( 4 );
			current.Next.Id.ShouldEqual( 5 );
			current.Next.Next.Id.ShouldEqual( 1 );
		};
	}
}

