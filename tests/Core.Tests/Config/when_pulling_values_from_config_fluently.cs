using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Config;

namespace Core.Tests.Config
{
    public class when_pulling_values_from_config_fluently
    {
        static bool booleanValue;
        static string stringValue;
        static short shortValue;
        static int intValue;
        static long longValue;
        static decimal decimalValue;
        static double doubleValue;
        static float floatValue;
        static DateTime dateValue;
        static Guid guidValue;

        static string connectionString;
        static string provider;

        static string customSectionValue;

        private Because of = () => 
        {
            booleanValue = FromConfig.GetValue<bool>( "One" );
            stringValue = FromConfig.GetValue<string>( "string" );
            shortValue = FromConfig.GetValue<short>( "short" );
            intValue = FromConfig.GetValue<int>( "int" );
            longValue = FromConfig.GetValue<long>( "long" );
            decimalValue = FromConfig.GetValue<decimal>( "decimal" );
            doubleValue = FromConfig.GetValue<double>( "double" );
            floatValue = FromConfig.GetValue<float>( "float" );
            dateValue = FromConfig.GetValue<DateTime>( "datetime" );
            guidValue = FromConfig.GetValue<Guid>( "guid" );
        };

        //private It should_have_valid_boolean = () => booleanValue.ShouldBeTrue();
        //private It should_have_valid_string = () => stringValue.ShouldEqual( "this is a string!" );
        //private It should_have_valid_short = () => shortValue.ShouldEqual( (short) 2 );
        //private It should_have_valid_int = () => intValue.ShouldEqual( 64000 );
        //private It should_have_valid_long = () => longValue.ShouldEqual( 64000000 );
        //private It should_have_valid_decimal = () => decimalValue.ShouldEqual( 1.5m );
        //private It should_have_valid_double = () => doubleValue.ShouldEqual( 10.49 );
        //private It should_have_valid_float = () => floatValue.ShouldEqual( 1048.9831f );
        //private It should_have_valid_dateTime = () => dateValue.ShouldEqual( DateTime.Parse( "01/15/1983" ) );
        //private It should_have_valid_guid = () => guidValue.ShouldEqual( Guid.Parse("00000000-0000-0000-0000-000000000001" ) );
    }
}
