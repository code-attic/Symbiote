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
using System.Linq;
using System.Threading.Tasks;

namespace Symbiote.Core.Utility
{
    public class RingBuffer
        : IDisposable
    {
        public bool Running { get; protected set; }
        public int Size { get; set; }
        public int[] PreviousStepLookup { get; set; }
        public RingBufferCell Head { get; set; }
        public RingBufferCell[] Steps { get; set; }
        public List<Func<object, object>> Transforms { get; protected set; }
        public List<Task> Tasks { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
            var current = Head;
            while ( current != null )
            {
                var next = current.Next;
                current.Dispose();
                current = next;
            }
        }

        #endregion

        public void Write<T>( T value )
        {
            if ( !Running )
                Start();
            Steps[0] = Steps[0].Transform( 0, x => value );
        }

        public void AddTransform( Func<object, object> transform )
        {
            Transforms.Add( transform );
            var transformCount = Transforms.Count;
            SetLastWrite( transformCount );
        }

        public void SetLastWrite( int transformCount )
        {
            var current = Head;
            while ( !current.Tail )
            {
                current.LastWriter = transformCount;
                current = current.Next;
            }
            current.LastWriter = transformCount;
            var countUp = -1;
            PreviousStepLookup = Enumerable.Range( countUp++, transformCount + 1 ).ToArray();
            PreviousStepLookup[0] = transformCount;
        }

        public void Process( int step, Func<object, object> transform )
        {
            while ( Running )
                Steps[step] = Steps[step].Transform( step, transform );
        }

        public void Start()
        {
            Running = true;
            int step = 1;
            Tasks = Transforms
                .Select( f => Task.Factory.StartNew( () => Process( step++, f ), TaskCreationOptions.LongRunning ) )
                .ToList();
        }

        public void Stop()
        {
            Running = false;
            Tasks = new List<Task>();
        }

        public RingBuffer( int size )
        {
            Transforms = new List<Func<object, object>>();
            Size = size;
            Head = RingBufferCell.Build( this, Size );
            Steps = Enumerable.Repeat( Head, 10 ).ToArray();
        }
    }
}