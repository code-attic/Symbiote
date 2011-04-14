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

namespace Symbiote.Core.DI
{
    public class NullOpDependencyAdapter : IDependencyAdapter
    {
        private string _msg =
            "The Dependency Adapter has not been specified.  This can happen when your code is accessing the IoC container before Assimilate.Core() has been called.\r\n";

        public object GetInstance( Type serviceType )
        {
            throw new AssimilationException( _msg );
        }

        public object GetInstance( Type serviceType, string key )
        {
            throw new AssimilationException( _msg );
        }

        public IEnumerable<object> GetAllInstances( Type serviceType )
        {
            throw new AssimilationException( _msg );
        }

        public T GetInstance<T>()
        {
            throw new AssimilationException( _msg );
        }

        public T GetInstance<T>( string key )
        {
            throw new AssimilationException( _msg );
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            throw new AssimilationException( _msg );
        }

        public IEnumerable<Type> RegisteredPluginTypes { get; private set; }

        public Type GetDefaultTypeFor<T>()
        {
            throw new AssimilationException( _msg );
        }

        public IEnumerable<Type> GetTypesRegisteredFor<T>()
        {
            throw new AssimilationException( _msg );
        }

        public IEnumerable<Type> GetTypesRegisteredFor( Type type )
        {
            throw new AssimilationException( _msg );
        }

        public bool HasPluginFor<T>()
        {
            throw new AssimilationException( _msg );
        }

        public bool HasPluginFor( Type type )
        {
            throw new AssimilationException( _msg );
        }

        public void Register( IDependencyDefinition dependency )
        {
            throw new AssimilationException( _msg );
        }

        public void Reset()
        {
            throw new AssimilationException( _msg );
        }

        public void Scan( IScanInstruction scanInstruction )
        {
            throw new AssimilationException( _msg );
        }
    }
}