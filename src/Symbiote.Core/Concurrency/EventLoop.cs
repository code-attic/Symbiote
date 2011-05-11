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
using System.Threading.Tasks;

namespace Symbiote.Core.Concurrency
{
    public class EventLoop :
        IEventLoop
    {
        public bool Running { get; set; }
        public ConcurrentQueue<Action> ActionQueue { get; set; }
        public ManualResetEventSlim Wait { get; set; }
        
        public void Loop()
        {
            Action action = null;
            while( Running )
            {
                if( ActionQueue.TryDequeue( out action ) )
                {
                    try
                    {
                        action();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex );
                    }
                }
                else 
                {
                    Wait.Reset();
                    Wait.Wait();
                }
            }
        }

        public void Enqueue( Action action ) 
        {
            ActionQueue.Enqueue( action );
            Wait.Set();
        }

        public void Start( int workers ) 
        {
            Running = true;
            for( int i = 0; i < workers; i ++ )
                Task.Factory.StartNew( Loop );
        }

        public void Stop() 
        {
            Running = false;
            Wait.Set();
        }

        public EventLoop( ) 
        {
            ActionQueue = new ConcurrentQueue<Action>();
            Wait = new ManualResetEventSlim( false );
        }
    }
}