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
using Symbiote.Core.Utility;

namespace Symbiote.Http.NetAdapter.SocketListener
{
    public class ClientSocketNode
    {
        public string Id { get { return ClientSocket == null ? "" : ClientSocket.Id; } }
        public ClientSocketNode Next { get; set; }
        public ClientSocketNode Previous { get; set; }
        public IClientSocketAdapter ClientSocket { get; set; }
        public Action<string> Remove { get; set; }
        public static readonly object ChangeLock = new object();

        public void Delete( string id )
        {
            lock( ChangeLock )
            {
                Previous.Next = Next;
                Next.Previous = Previous;
                ClientSocket = null;
                if( Remove != null )
                    Remove( id );
            }
        }

        public void Process()
        {
            try
            {
                if( ClientSocket != null && !ClientSocket.Read() )
                {
                    Delete( Id );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        public ClientSocketNode Add( IClientSocketAdapter adapter )
        {
            lock( ChangeLock )
            {
                return new ClientSocketNode( Previous, this, adapter );
            }
        }

        public ClientSocketNode()
        {
            Next = this;
            Previous = this;
            ClientSocket = null;
        }

        public ClientSocketNode( ClientSocketNode previous, ClientSocketNode next, IClientSocketAdapter clientSocket )
        {
            Next = next;
            Previous = previous;
            Previous.Next = this;
            Next.Previous = this;
            ClientSocket = clientSocket;
            Remove = ClientSocket.Remove;
            ClientSocket.Remove = Delete;
        }

        public override string ToString()
        {
            var builder = new DelimitedBuilder("-");
            var node = this;
            do
            {
                builder.Append( node.Id );
                node = node.Next;
            } while ( node.Id != Id );
            return builder.ToString();
        }
    }
}