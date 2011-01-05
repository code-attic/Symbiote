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

namespace Symbiote.Core.Impl.DI
{
    public interface IDependencyRegistry
    {
        IEnumerable<Type> RegisteredPluginTypes { get; }
        Type GetDefaultTypeFor<T>();
        IEnumerable<Type> GetTypesRegisteredFor<T>();
        IEnumerable<Type> GetTypesRegisteredFor(Type type);
        bool HasPluginFor<T>();
        void Register(IDependencyDefinition dependency);
        void Scan(IScanInstruction scanInstruction);
    }
}