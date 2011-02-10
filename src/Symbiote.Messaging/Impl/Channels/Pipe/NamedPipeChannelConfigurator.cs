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
using System.IO.Pipes;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamedPipeChannelConfigurator
    {
        public NamedPipeChannelDefinition Definition { get; protected set; }

        public NamedPipeChannelConfigurator Name( string name )
        {
            Definition.Name = name;
            return this;
        }

        public NamedPipeChannelConfigurator Pipe( string pipeName )
        {
            Definition.PipeName = pipeName;
            return this;
        }

        public NamedPipeChannelConfigurator Mode( PipeTransmissionMode mode )
        {
            Definition.Mode = mode;
            return this;
        }

        public NamedPipeChannelConfigurator Server( string name )
        {
            Definition.Machine = name;
            return this;
        }

        public NamedPipeChannelConfigurator AsServer()
        {
            Definition.IsServer = true;
            return this;
        }

        public NamedPipeChannelConfigurator()
        {
            Definition = new NamedPipeChannelDefinition();
        }
    }
}
