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
using System.IO.Pipes;
using System.Security.Principal;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamedPipeChannelDefinition : BaseChannelDefinition
    {
        public int BufferSize { get; set; }
        public int ConnectionTimeout { get; set; }
        public const int DEFAULT_BUFFER_SIZE = 8 * 1024;
        public PipeDirection Direction { get; set; }
        public TokenImpersonationLevel Impersonation { get; set; }
        public bool IsServer { get; set; }
        public string Machine { get; set; }
        public PipeTransmissionMode Mode { get; set; }
        public PipeOptions Options { get; set; }
        public string PipeName { get; set; }
        public PipeAccessRights Rights { get; set; }

        public override Type ChannelType
        {
            get { return typeof( NamedPipeChannel ); }
        }

        public override Type FactoryType
        {
            get { return typeof( NamedPipeChannelFactory ); }
        }

        public NamedPipeChannelDefinition( )
        {
            Name = "symbiote.pipe";
            Machine = ".";
            Direction = PipeDirection.InOut;
            Rights = PipeAccessRights.FullControl;
            Mode = PipeTransmissionMode.Message;
            Options = PipeOptions.Asynchronous;
            Impersonation = TokenImpersonationLevel.Anonymous;
            BufferSize = DEFAULT_BUFFER_SIZE;
            ConnectionTimeout = 1000;
        }
    }
}