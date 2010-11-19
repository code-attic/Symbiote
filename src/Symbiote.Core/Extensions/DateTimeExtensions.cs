using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime UnixEpoch = new DateTime( 1970, 1, 1, 0, 0, 0 );
        public static readonly long UnixEpochInTicks = UnixEpoch.Ticks;

        public static long ToUnixTimestamp(this DateTime dt)
        {
            return ( dt.Ticks - UnixEpochInTicks ) / 10000000;
        }

        public static DateTime FromUnixTimestamp(this long timestamp)
        {
            return UnixEpoch.AddSeconds( timestamp );
        }
    }
}
