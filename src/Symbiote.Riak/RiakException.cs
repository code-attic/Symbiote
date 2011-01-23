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
using Symbiote.Core.Extensions;
using Symbiote.Riak.Impl.ProtoBuf;

namespace Symbiote.Riak
{
    public class RiakException
        : Exception
    {
        public uint ErrorCode { get; set; }
        public string Message { get; set; }

        public RiakException(Impl.ProtoBuf.Response.Error error)
        {
            ErrorCode = error.Code;
            Message = error.Message.FromBytes();
        }

        public RiakException( string message )
        {
            Message = message;
        }

        public override string ToString()
        {
            return "{0} - {1}".AsFormat( ErrorCode, Message );
        }
    }
}
