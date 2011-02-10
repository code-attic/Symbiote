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
using System.IO.Pipes;
using Symbiote.Core.Futures;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class ServerPipeEndpoint
        : IPipeEndpoint
    {
        public NamedPipeServerStream Server { get; set; }
        public bool Running { get; set; }

        public PipeStream Stream { get { return Server; } }

        public bool Connected { get; set; }

        public void Connect( Action onConnect, Action onFailure )
        {
            Future.Of(
                x =>
                Server.BeginWaitForConnection(x, null),
                x =>
                    {
                        Server.EndWaitForConnection(x);
                        return true;
                    })
                .OnValue(x =>
                             {
                                 if (x)
                                 {
                                     Connected = true;
                                     Running = true;
                                     onConnect();
                                 }
                                 else
                                     Connect( onConnect, onFailure );
                             })
                .OnFailure(() =>
                               {
                                   onFailure();
                                   return true;
                               })
                .Start()
                .LoopWhile( () => Running );
        }

        public void Close()
        {
            Connected = false;
            Running = false;
            Server.Close();
        }

        public ServerPipeEndpoint( NamedPipeServerStream serverStream )
        {
            Server = serverStream;
        }

        public void Dispose()
        {
            if(Connected)
                Close();
        }
    }
}