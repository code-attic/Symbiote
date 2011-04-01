// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System;
using System.Linq;

namespace Symbiote.Core.Fibers
{
	public class IONode
	{
		public Func<byte[], Action<IAsyncResult>, object, IAsyncResult> Writer { get; set; }
		public Action<IAsyncResult> WriterComplete { get; set; }
		public Func<byte[], int, int, Action<IAsyncResult>, object, IAsyncResult> Reader { get; set; }
		public Func<IAsyncResult, int> ReaderComplete { get; set; }
		public Action<IONode> SetPreviousNodesNext { get; set; }
		public byte[] Buffer { get; set; }
		public int BufferSize { get; set; }
		public IONode Next { get; set; }
		
		public void Remove()
		{
			if( SetPreviousNodesNext != null )
				SetPreviousNodesNext( Next );
			Next = null;
			Writer = null;
			WriterComplete = null;
			Reader = null;
			ReaderComplete = null;
			SetPreviousNodesNext = null;
		}
		
		public void Read( Action<ArraySegment<byte>> onRead )
		{
			Reader( Buffer, 0, BufferSize, ReadComplete, onRead );
		}
			       
		public void ReadComplete( IAsyncResult result )
        {
			try 
			{
				var read = ReaderComplete( result );
				var bytes = Buffer.GetRange( 0, read );
				var action = result.AsyncState as Action<ArraySegment<byte>>;
				action( new ArraySegment<byte>( bytes, 0, read ) );
			} 
			catch (Exception ex) 
			{
				
			}
		}
		
		public void Write( ArraySegment<byte> segment, Action onComplete )
		{
			Writer( segment.GetRange(), WriteComplete, onComplete );
		}
		
		public void WriteComplete( IAsyncResult result )
		{
			try
			{
				WriterComplete( result );
				Action onComplete = result.AsyncState as Action;
			}
			catch
			{
			}
		}
		
		public IONode( 
		              Action<IONode> setPrevious,
		              Func<byte[], Action<IAsyncResult>, object, IAsyncResult> writer,
		              Action<IAsyncResult> writerComplete,
		              Func<byte[], int, int, Action<IAsyncResult>, object, IAsyncResult> reader,
		              Func<IAsyncResult, int> readerComplete )
		{
			SetPreviousNodesNext = setPrevious;
			Writer = writer;
			WriterComplete = writerComplete;
			Reader = reader;
			ReaderComplete = readerComplete;
		}
	}
	
	
	
    public class MailboxNode<T>
    {
        public string Id { get; set; }
        public MailboxNode<T> Next { get; set; }
        public MailboxNode<T> Previous { get; set; }
        public Mailbox<T> Mailbox { get; set; }
        public Action Remove { get; set; }

        public void Delete()
        {
            Mailbox = null;
            if( !Next.Id.Equals( Id ) )
                Previous.Next = Next;
            if( !Previous.Id.Equals( Id ) )
                Next.Previous = Previous;
            Remove();
        }

        public MailboxNode<T> Add( string id, Action remove )
        {
            var newNode = new MailboxNode<T>( id, this, Next, remove );
            Next.Previous = newNode;
            Next = newNode;
            return newNode;
        }

        public MailboxNode( Action remove )
        {
            Id = "";
            Mailbox = new Mailbox<T>( "" );
            Next = this;
            Previous = this;
            Remove = remove;
        }

        public MailboxNode( string id, MailboxNode<T> previous, MailboxNode<T> next, Action remove )
        {
            Id = id;
            Mailbox = new Mailbox<T>( id );
            Previous = previous;
            Next = next;
            Remove = remove;
        }
    }
}