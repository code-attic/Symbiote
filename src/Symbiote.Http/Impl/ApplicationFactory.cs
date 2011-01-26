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
using Symbiote.Core;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl
{
    public class ApplicationFactory : IApplicationFactory
    {
        public Type ApplicationType { get; set; }

        public IApplication CreateApplication()
        {
            return Assimilate.GetInstanceOf(ApplicationType) as IApplication;
        }

        public ApplicationFactory(Type applicationType)
        {
            ApplicationType = applicationType;
        }
    }

    public class ApplicationFactory<T> : IApplicationFactory
        where T : IApplication
    {
        public IApplication CreateApplication()
        {
            return Assimilate.GetInstanceOf<T>();
        }
    }
}