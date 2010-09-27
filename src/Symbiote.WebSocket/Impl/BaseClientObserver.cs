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

namespace Symbiote.WebSocket.Impl
{
    public abstract class BaseClientObserver :
        IObserver<Tuple<string,string>>,
        IDisposable
    {
        public Action<Tuple<string, string>> MessageReceived { get; set; }

        public virtual void OnNext(Tuple<string, string> value)
        {
            if(MessageReceived != null)
                MessageReceived(value);
        }

        public virtual void OnError(Exception error)
        {
            
        }

        public virtual void OnCompleted()
        {
            
        }

        protected BaseClientObserver(Action<Tuple<string, string>> messageReceived)
        {
            MessageReceived = messageReceived;
        }

        public void Dispose()
        {
            OnCompleted();
            MessageReceived = null;
        }
    }
}
