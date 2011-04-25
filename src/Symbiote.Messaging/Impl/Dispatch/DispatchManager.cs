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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Core.Fibers;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class DispatchManager
        : IDispatcher
    {
        public ConcurrentDictionary<Type, IDispatchMessage> Dispatchers { get; set; }
        //public Director<IEnvelope> Fibers { get; set; }
        public ConcurrentDictionary<string, IDispatchMessage> ResponseDispatchers { get; set; }
        public ManualResetEventSlim Signal { get; set; }
        public IEventLoop Loop { get; set; }
        public Random RandomMailbox { get; set; }

        public int Count { get; set; }

        public void Send<TMessage>( IEnvelope<TMessage> envelope )
        {
            var id = string.IsNullOrEmpty(
                envelope.CorrelationId )
                         ? ( envelope.MessageId.GetHashCode()%100 ).ToString()
                         : envelope.CorrelationId;

            Loop.Enqueue( () =>
                {
                    SendToHandler( id, envelope );
                });
        }

        public void Send( IEnvelope envelope )
        {
            var id = string.IsNullOrEmpty(
                envelope.CorrelationId )
                         ? ( envelope.MessageId.GetHashCode()%100 ).ToString()
                         : envelope.CorrelationId;
            Loop.Enqueue( () =>
                {
                    SendToHandler( id, envelope );
                });
        }

        //public void Send<TMessage>( IEnvelope<TMessage> envelope )
        //{
        //    Count++;
        //    Fibers.Send(
        //        string.IsNullOrEmpty(
        //            envelope.CorrelationId )
        //            ? (envelope.MessageId.GetHashCode() % 100).ToString()
        //            : envelope.CorrelationId,
        //        envelope );
        //}

        //public void Send( IEnvelope envelope )
        //{
        //    Count++;
        //    Fibers.Send(
        //        string.IsNullOrEmpty(
        //            envelope.CorrelationId )
        //            ? (envelope.MessageId.GetHashCode() % 100).ToString()
        //            : envelope.CorrelationId,
        //        envelope );
        //}

        public void ExpectResponse<TResponse>( string correlationId, Action<TResponse> onResponse )
        {
            ResponseDispatchers.TryAdd( correlationId, new ResponseDispatcher<TResponse>( onResponse ) );
        }

        public void SendToHandler( string id, IEnvelope envelope )
        {
            IDispatchMessage dispatcher;
            if ( Dispatchers.TryGetValue( envelope.MessageType, out dispatcher ) )
            {
                dispatcher.Dispatch( envelope );
            }
            else
            {
                IEnumerable<Type> inheritanceChain = Reflector.GetInheritanceChain(envelope.MessageType);
                Type key = null;
                if ( ( key = Dispatchers.Keys.FirstOrDefault( k => inheritanceChain.Contains( k ) ) ) != null )
                {
                    dispatcher = Dispatchers[key];
                    Dispatchers.AddOrUpdate( envelope.MessageType, x => dispatcher, ( x, y ) => dispatcher );
                    dispatcher.Dispatch( envelope );
                }
                // message is a response to a request message
                else if ( ResponseDispatchers.TryRemove( envelope.CorrelationId, out dispatcher ) )
                {
                    dispatcher.Dispatch( envelope );
                }
            }
        }

        public void WireupDispatchers()
        {
            if ( Dispatchers.Count == 0 )
            {
                var dispatchers = Assimilate.GetAllInstancesOf<IDispatchMessage>();
                dispatchers
                    .ForEach( x => x.Handles.ForEach( y => Dispatchers.AddOrUpdate( y, x, ( t, m ) => x ) ) );
            }
            // prevent duplicate instantiations
            var agency = Assimilate.GetInstanceOf<IAgency>();
            agency.RegisterActorOf( "", this );
            // prime director
            //Fibers.Send("", new Envelope<PrimeDirector>(new PrimeDirector()) { CorrelationId = "" });
            Loop.Enqueue( () => SendToHandler( "", new Envelope<PrimeDirector>( new PrimeDirector()) { CorrelationId = "" } ) );
            Signal.Wait( 100 );
        }

        public DispatchManager()
        {
            Dispatchers = new ConcurrentDictionary<Type, IDispatchMessage>();
            ResponseDispatchers = new ConcurrentDictionary<string, IDispatchMessage>();
            //Fibers = new Director<IEnvelope>( SendToHandler );
            Loop = new EventLoop();
            Loop.Start( 2 );
            Signal = new ManualResetEventSlim();
            WireupDispatchers();
        }
    }

    public interface IEventLoop
    {
        void Enqueue( Action action );
        void Start( int workers );
        void Stop();
    }

    public class EventLoop :
        IEventLoop
    {
        public bool Running { get; set; }
        public ConcurrentQueue<Action> ActionQueue { get; set; }
        //public ManualResetEventSlim Wait { get; set; }
        public ManualResetEvent Wait { get; set; }
        public CancellationToken Cancel { get; set; }
        
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
                    //Thread.Sleep( 0 );
                    Wait.Reset();
                    //Wait.Wait( Cancel );
                    Wait.WaitOne();
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
            Cancel.WaitHandle.Close();
            Wait.Set();
        }

        public EventLoop( ) 
        {
            ActionQueue = new ConcurrentQueue<Action>();
            //Wait = new ManualResetEventSlim( false, 10 );
            Wait = new ManualResetEvent( false );
            Cancel = new CancellationToken();
        }
    }
}