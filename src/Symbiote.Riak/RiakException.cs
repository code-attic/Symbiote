using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
