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

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamedPipeChannelDefinition : BaseChannelDefinition
    {
        public string Server { get; set; }
        public PipeDirection Direction { get; set; }
        public PipeAccessRights Rights { get; set; }
        public PipeTransmissionMode Mode { get; set; }
        public PipeOptions Options { get; set; }
        public TokenImpersonationLevel Impersonation { get; set; }
        public int ConnectionTimeout { get; set; }

        public override Type ChannelType
        {
            get { return typeof( NamedPipeChannel ); }
        }

        public override Type FactoryType
        {
            get { return typeof( NamedPipeChannelFactory ); }
        }

        public NamedPipeChannelDefinition( string name )
        {
            Name = name;
            Server = "Symbiote.Pipes.Host";
            Direction = PipeDirection.InOut;
            Rights = PipeAccessRights.FullControl;
            Mode = PipeTransmissionMode.Message;
            Options = PipeOptions.Asynchronous;
            Impersonation = TokenImpersonationLevel.Anonymous;
            ConnectionTimeout = 1000;
        }
    }
}