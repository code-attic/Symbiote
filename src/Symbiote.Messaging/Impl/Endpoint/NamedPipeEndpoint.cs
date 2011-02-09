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
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Endpoint
{
    public class NamedPipeEndpoint
    {
        public const int DEFAULT_BUFFER_SIZE = 8 * 1024;
        public string PipeName { get; set; }
        public Type SerializerType { get; set; }
        public int BufferSize { get; set; }
        public NamedPipeServerStream Stream { get; set; }

        public void OpenStream()
        {
            Stream = new NamedPipeServerStream( 
                PipeName, 
                PipeDirection.InOut, 
                1,
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous,
                DEFAULT_BUFFER_SIZE,
                DEFAULT_BUFFER_SIZE);
        }

        public void Close()
        {
            if (Stream.IsConnected)
                Stream.Disconnect();
            Stream.Close();
        }

        public NamedPipeEndpoint(string name)
        {
            PipeName = name;
            SerializerType = typeof( MessageOptimizedSerializer );
            BufferSize = DEFAULT_BUFFER_SIZE;
        }
    }
}