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

namespace Symbiote.Core.Utility
{
    public static class Pipeline
    {
        public static Func<TStart, TNext> Then<TStart, TResult, TNext>( this Func<TStart, TResult> start,
                                                                        Func<TResult, TNext> then )
        {
            return x => then( start( x ) );
        }

        public static Func<TStart, TNext> Then<TStart, TResult, TService, TNext>( this Func<TStart, TResult> start,
                                                                                  Func<TService, TResult, TNext> then )
        {
            var service = Assimilate.GetInstanceOf<TService>();
            return x => then( service, start( x ) );
        }

        public static Func<Ti, To> Start<Ti, To>( Func<Ti, To> funcy )
        {
            return funcy;
        }
    }
}