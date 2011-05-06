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
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Saga;

namespace Symbiote.Daemon.BootStrap
{
    public class MinionSaga
        : Saga<Minion>
    {
        public override Action<StateMachine<Minion>> Setup()
        {
            return x =>
                       {
                            x.ConditionsAreMutuallyExclusive();

                            x.When( a => a.Running )
                               .On<ApplicationDeleted>( ( a, h ) =>
                                   {
                                       a.ShutItDown();
                                       return e => e.Acknowledge();
                                   } );

                            x.When( a => a.Running && !a.Starting )
                               .On<ApplicationChanged>( ( a, h ) =>
                                    {
                                        a.ShutItDown();
                                        a.StartUp();
                                        return e => e.Acknowledge();
                                    } );

                            x.When( a => !a.Running )
                               .On<ApplicationChanged>( ( a, h ) =>
                                   {
                                       a.StartUp();
                                       return e => e.Acknowledge();
                                   } )
                               .On<NewApplication>( ( a, h ) =>
                                   {
                                       a.StartUp();
                                       return e => e.Acknowledge();
                                   } );

                            x.When( a => true )
                               .On<NewApplication>( ( a, h ) =>
                                   {
                                       a.StartUp();
                                       return e => e.Acknowledge();
                                   } );
                       };
        }

        public MinionSaga( StateMachine<Minion> stateMachine ) : base( stateMachine )
        {
        }
    }
}