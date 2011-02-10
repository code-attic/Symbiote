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

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class ClientPipeEndpoint
        : IPipeEndpoint
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public NamedPipeClientStream Client { get; set; }
        public bool Running { get; set; }

        public PipeStream Stream { get { return Client; } }

        public bool Connected { get; set; }

        public void Connect( Action onConnect, Action onFailure )
        {
            Client = new NamedPipeClientStream(
                Definition.Machine,
                Definition.PipeName,
                Definition.Direction,
                Definition.Options,
                Definition.Impersonation);

            try
            {
                Client.Connect(Definition.ConnectionTimeout);
                Client.ReadMode = Definition.Mode;
                Connected = true;
                onConnect();
            }
            catch ( TimeoutException timeoutException )
            {
                onFailure();
            }
        }

        public void Close()
        {
            Connected = false;
            Client.Close();
        }

        public ClientPipeEndpoint( NamedPipeChannelDefinition definition )
        {
            Definition = definition;
        }

        public void Dispose()
        {
            if( Connected )
                Close();
        }
    }
}