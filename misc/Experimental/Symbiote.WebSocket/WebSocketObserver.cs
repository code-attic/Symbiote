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

namespace Symbiote.WebSocket
{
    public class WebSocketObserver : 
        IObserver<Tuple<string, string>>,
        IDisposable
    {
        public Action<Tuple<string, string>> MessageReceived { get; set; }
        public Action<string, Exception> ClientError { get; set; }
        public Action<string> ClientDisconnect { get; set; }
        
        public string ClientId { get; set; }

        public virtual void OnNext(Tuple<string, string> value)
        {
            try
            {
                if(MessageReceived != null)
                    MessageReceived(value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public virtual void OnError(Exception error)
        {
            if(ClientError != null)
                ClientError(ClientId, error);
        }

        public virtual void OnCompleted()
        {
            if(ClientDisconnect != null)
                ClientDisconnect(ClientId);
        }

        public void Dispose()
        {
            OnCompleted();
            MessageReceived = null;
            ClientError = null;
            ClientDisconnect = null;
        }
    }
}