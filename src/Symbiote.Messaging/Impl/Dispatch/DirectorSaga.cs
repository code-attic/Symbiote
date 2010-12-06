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
using Symbiote.Actor;
using Symbiote.Actor.Impl.Saga;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class DirectorSaga
        : Saga<DispatchManager>
    {
        public override Action<StateMachine<DispatchManager>> Setup()
        {
            return machine =>
            {
                machine.Unconditionally()
                    .On<PrimeDirector>( director => 
                                        director.Signal.Set() );
            };
        }

        public DirectorSaga( StateMachine<DispatchManager> stateMachine ) : base( stateMachine ) {}
    }
}