using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public static class BigEndianExtensions
    {
        public static uint ToggleEndianicity(this uint value)
        {
            return (uint)(((
                (value & 0x000000FF) << 24) |
                ((value & 0x0000FF00) << 8) |
                ((value & 0x00FF0000) >> 8) |
                ((value & 0xFF000000) >> 24)));
        }
    }
}
