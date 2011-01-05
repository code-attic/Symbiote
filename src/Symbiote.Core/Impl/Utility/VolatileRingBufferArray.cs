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
using Symbiote.Core.Impl.Collections;

namespace Symbiote.Core.Impl.Utility
{
    /// <summary>
    /// Caution: may eat babies, data and your registers
    /// </summary>
    public class VolatileRingBufferArray
    {
        protected volatile int[,] WriteIndex;
        protected volatile object[,] Ring;
        protected volatile int[,] Iteration;
        protected List<Func<object, object>> Transforms;
        protected ExclusiveConcurrentDictionary<string, int> ActorIndex { get; set; }
        protected int Actors;
        protected bool Running;
        protected int Capacity;
        protected int LastIndex;
        protected int LastStep;

        public int GetNextIndex(int index)
        {
            return index == LastIndex
                       ? 0
                       : index + 1;
        }

        public int GetNextStep( int step )
        {
            return step == LastStep
                       ? 0
                       : step + 1;
        }

        public int GetPreviousStep(int step)
        {
            return step == 0
                       ? LastStep
                       : step - 1;
        }

        public void UpdateIteration(int actor, int step)
        {
            var current = Iteration[actor, step];
            Iteration[actor, step] = current == int.MaxValue
                                  ? 0
                                  : current + 1;
        }

        public void AddTransform(Func<object, object> transformer)
        {
            Transforms.Add(transformer);
            LastStep = Transforms.Count;
        }

        public void Write<T>(string actor, T value)
        {
            var actorIndex = ActorIndex.ReadOrWrite( actor, () => ++Actors );

            var current = WriteIndex[actorIndex, 0];
            while (current == WriteIndex[actorIndex, LastStep]
                && Iteration[actorIndex, 0] > Iteration[actorIndex, LastStep])
            {
                Thread.Sleep( 10 );
            }

            if(!Running)
                Start();
            Ring[actorIndex, current] = value;
            var nextIndex = GetNextIndex(current);
            if(nextIndex < current)
                UpdateIteration(actorIndex, 0);
            WriteIndex[actorIndex, 0] = nextIndex;
        }

        public void Transform(Func<object, object> transformer, int step)
        {
            while(Running)
            {
                if (Actors == 0)
                    Running = false;
                for (int i = 0; i < Actors; i++)
                {
                    TransformActor( transformer, i, step );
                }
            }
        }

        public void TransformsForActor(int actorIndex)
        {
            while(Running)
            {
                int step = 1;
                foreach (var transform in Transforms)
                {
                    TransformActor( transform, actorIndex, step++ );
                }
            }
        }

        public void TransformActor(Func<object, object> transformer, int actorIndex, int step)
        {
            try
            {
                var current = WriteIndex[actorIndex, step];
                if (WriteIndex[actorIndex, step] != WriteIndex[actorIndex, step - 1]
                    || Iteration[actorIndex, step] != Iteration[actorIndex, step - 1])
                {
                    Ring[actorIndex, current] = transformer(Ring[actorIndex, current]);
                    var nextIndex = GetNextIndex(current);
                    if (nextIndex < current)
                        UpdateIteration(actorIndex, step);
                    WriteIndex[actorIndex, step] = nextIndex;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Start()
        {
            Running = true;
            Action<Func<object, object>, int> step = Transform;

            int index = 1;
            Transforms
                .ForEach(t =>
                {
                    step.BeginInvoke(t, index++, null, null);
                });
        }

        public void Stop()
        {
            Running = false;
        }

        public VolatileRingBufferArray(int actors, int items)
        {
            ActorIndex = new ExclusiveConcurrentDictionary<string, int>();
            Transforms = new List<Func<object, object>>();
            Capacity = items;
            LastIndex = items -1;
            Ring = new object[actors, items];
            WriteIndex = new int[actors, 32];
            Iteration = new int[actors, 32];
            Running = true;
        }
    }
}