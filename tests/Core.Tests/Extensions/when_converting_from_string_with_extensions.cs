using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    public class when_converting_from_string_with_extensions
    {
        private It should_convert_to_bool = () => "true".ConvertTo<bool>().ShouldBeTrue();
        private It should_convert_to_short = () => "1".ConvertTo<short>().ShouldEqual( (short) 1 );
        private It should_convert_to_int = () => "10".ConvertTo<int>().ShouldEqual( 10 );
        private It should_convert_to_long = () => "100000000".ConvertTo<long>().ShouldEqual( 100000000 );
        private It should_convert_to_decimal = () => "10.123498109".ConvertTo<decimal>().ShouldEqual( 10.123498109m );
        private It should_convert_to_float = () => "134598.193410".ConvertTo<float>().ShouldEqual( 134598.193410f );
        private It should_convert_to_guid = () => "00000008-0004-0004-0004-000000000012".ConvertTo<Guid>().ShouldEqual( Guid.Parse( "00000008-0004-0004-0004-000000000012" ) );

    }
}
