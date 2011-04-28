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
using System.Collections.Generic;
using System.Threading.Tasks;
using Symbiote.Core.Collections;

namespace Symbiote.Core.Concurrency
{
    public class MailboxManager<T>
        : IDisposable, IMailboxManager<T>
    {
        Action<string, T> Actor { get; set; }
        public ExclusiveConcurrentDictionary<string, MailboxNode<T>> Mailboxes { get; set; }
        public MailboxNode<T> Root { get; set; }
        public List<Task> Tasks { get; set; }
        public bool Running { get; set; }

        public void Process()
        {
            var node = Root;
            while(Running)
            {
                node.Mailbox.Process( Actor );
                node = node.Next;
            }
        }

        public void RemoveMailboxFromDictionary( string id )
        {
            Mailboxes.Remove( id );
        }
        
        public void Send( T message )
        {
            Send( "", message );
        }

        public void Send( string id, T message )
        {
            var node = Mailboxes
                .ReadOrWrite( id, () => Root.Add( id, () => RemoveMailboxFromDictionary( id ) ) );
            node.Mailbox.Write( message );
        }

        public Task SpawnTask()
        {
            var task = new Task( Process, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness );
            task.Start();
            return task;
        }

        public void Stop()
        {
            Running = false;
        }

        public MailboxManager( Action<string, T> actor )
        {
            Actor = actor;
            Root = new MailboxNode<T>( () => RemoveMailboxFromDictionary( "" ) );
            Mailboxes = new ExclusiveConcurrentDictionary<string, MailboxNode<T>>();
            Mailboxes.ReadOrWrite( "", () => Root );
            Tasks = new List<Task>();
            Tasks.Add( SpawnTask() );
            Running = true;
        }

        public void Dispose()
        {
            Stop();
            try
            {
                Task.WaitAll( Tasks.ToArray() );
                Tasks.ForEach( x => x.Dispose() );
            }
            catch ( Exception e )
            {
                Tasks.Clear();
            }
            Tasks = null;
        }
    }
}
