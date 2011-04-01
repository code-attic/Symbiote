using System;
using System.Threading;

namespace Symbiote.Core
{
	public class LoopNode<TNode>
		where TNode : LoopNode<TNode>
	{
		public int Id { get; set; }
		public Action<TNode> ResetNext { get; set; }
		public TNode Next { get; set; }
		public static SpinLock Lock { get; set; }
		
		public void AddNode( TNode newNode )
		{
			var lockTaken = false;
			Lock.Enter( ref lockTaken );
			newNode.Next = (TNode) this;
			
			if( ResetNext != null )
				ResetNext( newNode );
			Next = Next ?? newNode;			
			Action<TNode> defaultNext = x => Next = x;
			newNode.ResetNext = ResetNext ?? defaultNext;
			ResetNext = x => newNode.Next = x;
			Lock.Exit();
		}
		
		public void Remove()
		{
			var lockTaken = false;
			Lock.TryEnter( ref lockTaken );
			if( ResetNext != null )
			{
				ResetNext( Next );
				Next.ResetNext = ResetNext;
			}
			ResetNext = null;
			Next = null;
			Lock.Exit();
		}
		
		public LoopNode ( )
		{
		}
		
		static LoopNode()
		{
			Lock = new SpinLock( false );
		}
	}
}

