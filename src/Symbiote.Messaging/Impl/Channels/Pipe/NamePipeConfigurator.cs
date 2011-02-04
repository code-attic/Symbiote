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
using System.Security.Principal;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamePipeConfigurator
    {
        public NamedPipeChannelDefinition Defintion { get; set; }

        public NamePipeConfigurator Server( string server )
        {
            Defintion.Server = server;
            return this;
        }

        public NamePipeConfigurator Direction( PipeDirection direction )
        {
            Defintion.Direction = direction;
            return this;
        }

        public NamePipeConfigurator Rights( PipeAccessRights rights )
        {
            Defintion.Rights = rights;
            return this;
        }

        public NamePipeConfigurator Mode( PipeTransmissionMode mode )
        {
            Defintion.Mode = mode;
            return this;
        }

        public NamePipeConfigurator Options( PipeOptions options )
        {
            Defintion.Options = options;
            return this;
        }

        public NamePipeConfigurator Impersonation( TokenImpersonationLevel impersonationLevel )
        {
            Defintion.Impersonation = impersonationLevel;
            return this;
        }

        public NamePipeConfigurator ConnectTimeout( int msTimeout )
        {
            Defintion.ConnectionTimeout = msTimeout;
            return this;
        }

        public NamePipeConfigurator( string channelName )
        {
            Defintion = new NamedPipeChannelDefinition( channelName );
        }
    }
}