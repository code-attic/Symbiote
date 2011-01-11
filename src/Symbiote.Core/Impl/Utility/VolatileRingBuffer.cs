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
using System.Threading;
using System.Threading.Tasks;

namespace Symbiote.Core.Impl.Utility
{
    /// <summary>
    /// Caution: understand what this is before using it in your application!
    /// </summary>
    public class VolatileRingBuffer
    {
        public volatile int[] WriteIndex;
        public volatile object[] Ring;
        public volatile int[] Iteration;
        public bool Started;
        
        protected List<Func<object, object>> Transforms;
        protected bool Running;
        public int Capacity;
        public int LastIndex;
        public int LastStep;

        public int GetNextIndex(int index)
        {
            return index == LastIndex
                       ? 0
                       : index + 1;
        }

        public int GetPreviousStep(int step)
        {
            return step == 0
                       ? LastStep
                       : step - 1;
        }

        public void UpdateIteration(int step)
        {
            var current = Iteration[step];
            Iteration[step] = current == int.MaxValue
                                  ? 0
                                  : current + 1;
        }

        public void AddTransform(Func<object, object> transformer)
        {
            Transforms.Add(transformer);
            LastStep = Transforms.Count;
        }

        public void Write<T>(T value)
        {
            var current = WriteIndex[0];

            while (current == WriteIndex[LastStep] && Iteration[0] > Iteration[LastStep])
                Thread.Sleep(1);

            Ring[current] = value;
            var nextIndex = GetNextIndex(current);
            if (nextIndex < current)
                UpdateIteration(0);
            WriteIndex[0] = nextIndex;
        }

        public void Transform(Func<object, object> transformer, int step)
        {
            while(Running)
            {
                try
                {
                    while (!IsStepReady(step))
                        Thread.Sleep(3);

                    var current = WriteIndex[step];
                    if(Ring[current] != null)
                        Ring[current] = transformer( Ring[current] );

                    var nextIndex = GetNextIndex(current);
                    if (nextIndex < current)
                        UpdateIteration(step);
                    WriteIndex[step] = nextIndex;
                }
                catch (Exception e)
                {
                    Console.WriteLine( e );
                }
            }
        }

        public bool IsStepReady( int step )
        {
            return WriteIndex[step] != WriteIndex[step - 1]
                    || Iteration[step] != Iteration[step - 1];
        }

        public void Start()
        {
            Running = true;
            Action<Func<object, object>, int> step = Transform;
            int index = 1;
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

        public VolatileRingBuffer(int items)
        {
            Transforms = new List<Func<object, object>>();
            Capacity = items;
            LastIndex = items -1;
            Ring = new object[items];
            WriteIndex = new int[50];
            Iteration = new int[50];
        }
    }
}
