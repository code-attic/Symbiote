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
using System.Collections.Concurrent;
using System.Threading;

namespace Symbiote.Core.Fibers
{
    public class Mailbox<T>
    {
        public string Id { get; set; }
        public bool Processing { get; set; }
        public object ProcessLock { get; set; }
        public ConcurrentQueue<T> Messages { get; set; }

        public void Write( T message )
        {
            Messages.Enqueue( message );
        }

        public void Process( Action<string, T> action )
        {
            if( !Processing && Messages.Count > 0 )
            {
                try
                {
                    Processing = true;
                    lock( ProcessLock )
                    {
                        T message;
                        do
                        {
                            if ( Messages.TryDequeue( out message ) )
                                try
                                {
                                    action( Id, message );
                                }
                                catch( Exception e )
                                {
                                    var x = e;
                                }
                                finally
                                {
                                    
                                }
                        } while ( Messages.Count > 0 );
                    }
                }
                finally
                {
                    Processing = false;
                }
            }
            else
            {
                Thread.Sleep( 0 );
            }
        }

        public Mailbox( string id )
        {
            Id = id;
            ProcessLock = new object();
            Messages = new ConcurrentQueue<T>();
        }
    }
}