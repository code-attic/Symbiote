/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Symbiote.Core.Impl.Utility
{
    /// <summary>
    /// Caution: understand what this is before using it in your application!
    /// </summary>
    public class VolatileRingBuffer2
    {
        public volatile RingBufferIndex[] Indices;
        protected volatile int[] WriterFunctionIndex;
        protected volatile int[,] TransformFunctionIndex;
        public volatile object[] Ring;

        protected Func<object, bool>[] WriteFunctions;
        protected Action<Func<object, object>, int>[] TransformFunctions;

        protected List<Func<object, object>> Transforms;
        protected bool Running;
        public bool Started;
        public int Capacity;

        public void AddTransform(Func<object, object> transformer)
        {
            Transforms.Add(transformer);
        }

        public void Write<T>(T value)
        {
            try
            {
                while (!WriteFunctions[WriterFunctionIndex[Indices[0]]](value))
                    Thread.Sleep( TimeSpan.FromTicks( 10 ) );
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
            }
        }

        public void Transform(Func<object, object> transformer, int step)
        {
            while(Running)
            {
                TransformFunctions[TransformFunctionIndex[step, Indices[step]]]( transformer, step );
            }
        }

        public void Start()
        {
            Running = true;
            Action<Func<object, object>, int> step = Transform;
            int index = 0;
            Transforms
                .ForEach(t =>
                {
                    var task = Task.Factory.StartNew( () => Transform( t, index++ ), TaskCreationOptions.LongRunning );
                });
        }

        public void Stop()
        {
            Running = false;
        }

        public VolatileRingBuffer2(int items)
        {
            Capacity = items;
            WriteFunctions = new Func<object, bool>[]
            {
                x =>
                {
                    var ringBufferIndex = Indices[0];
                    WriterFunctionIndex[ringBufferIndex] = 1;
                    Ring[ringBufferIndex] = x;
                    TransformFunctionIndex[0, ringBufferIndex] = 1;
                    Indices[0]++;
                    return true;
                },
                x =>
                {
                    return false;
                }
            };

            TransformFunctions = new Action<Func<object, object>, int>[]
            {
                (x,y) =>
                {
                    Thread.Sleep( 1 );
                },
                (x,y) =>
                {
                    var ringBufferIndex = Indices[y];
                    TransformFunctionIndex[y, ringBufferIndex] = 0;
                    Ring[ringBufferIndex] = x( Ring[ringBufferIndex] );
                    if (y == Transforms.Count - 1)
                        WriterFunctionIndex[ringBufferIndex] = 0;
                    else
                        TransformFunctionIndex[y + 1, ringBufferIndex] = 1;
                    Indices[y]++;
                }
            };
            WriterFunctionIndex = new int[items + 1];
            TransformFunctionIndex = new int[50, items + 1];
            Ring = new object[items + 1];
            Indices = Enumerable.Range( 0, 50 ).Select( x => new RingBufferIndex(1, Capacity) ).ToArray();
            Transforms = new List<Func<object, object>>();
        }
    }
}
