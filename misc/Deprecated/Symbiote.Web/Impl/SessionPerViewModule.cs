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
using System.Web;
using Symbiote.Hibernate;

namespace Symbiote.Web.Impl
{
    public class SessionPerViewModule : IHttpModule
    {
        private readonly ISessionModule _module;

        public SessionPerViewModule(ISessionModule module)
        {
            _module = module;
        }

        #region IHttpModule Members

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public virtual void EndRequest(object sender, EventArgs e)
        {
            _module.EndSession();
        }

        public virtual void BeginRequest(object sender, EventArgs e)
        {
            _module.BeginSession();
        }


        public void Dispose()
        {
            _module.Dispose();
        }

        #endregion
    }
}