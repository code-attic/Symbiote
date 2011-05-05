// /* 
// Copyright 2008-2011 Jim Cowart & Alex Robson
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
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class when_testing_lifecycle
        : with_assimilation
    {
        static int count;
        private Because of = () => 
                                 { 
                                     var instance = Assimilate.GetInstanceOf<IEventListenerManager>();
                                     instance = Assimilate.GetInstanceOf<IEventListenerManager>();

                                     count = EventListenerManager.Instance;
                                 };
        
        private It should_result_in_singleton_instances = () => count.ShouldEqual( 1 );
    }
}