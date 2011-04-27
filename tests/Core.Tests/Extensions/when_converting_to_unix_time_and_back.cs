using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    public class when_converting_to_unix_time_and_back
    {
        public static DateTime Original;
        public static long UnixTime;
        public static DateTime UtcResult;
        public static TimeSpan Difference;

        private Because of = () => 
        {
            Original = DateTime.Now;
            UnixTime = Original.ToUnixTimestamp();
            UtcResult = UnixTime.FromUnixTimestamp();
            Difference = UtcResult.TimeOfDay - Original.TimeOfDay;
        };
        
        private It should_have_no_difference_in_original_and_conversion = () => Math.Abs(Difference.TotalSeconds).ShouldBeLessThan( 1 );
    }
}
