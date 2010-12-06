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
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Http;

namespace Symbiote.Couch.Impl.Commands
{
    public class ChangeStreamCommand : BaseCouchCommand
    {
        public IHttpAction BeginStreaming<TModel>(int since, Action<string, ChangeRecord> onUpdate, AsyncCallback streamInterrupted)
        {
            Action<CouchUri, int, Action<string, ChangeRecord>> proxy = action.GetContinuousResponse;
            proxy.BeginInvoke(CreateUri<TModel>(), since, onUpdate, streamInterrupted, null);
            return action;
        }

        public IHttpAction BeginStreaming(string database, int since, Action<string, ChangeRecord> onUpdate, AsyncCallback streamInterrupted)
        {
            Action<CouchUri, int, Action<string, ChangeRecord>> proxy = action.GetContinuousResponse;
            proxy.BeginInvoke(CreateUri(database), since, onUpdate, streamInterrupted, null);
            return action;
        }

        public ChangeStreamCommand(IHttpAction action, ICouchConfiguration configuration) : base(action, configuration)
        {
        }
    }
}
