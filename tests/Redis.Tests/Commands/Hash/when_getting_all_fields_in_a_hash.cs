using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.Hash
{
    public class when_getting_all_fields_in_a_hash :
        with_clean_db
    {
        protected static bool GetRsltCountCorrect;
        protected static bool GetRsltFieldsCorrect;
        protected static bool GetRsltValsCorrect;

        private Because of = () =>
        {
            var key = "Test Hash Key Get All";
            var fieldFmt = "Test Hash Field Get All {0}";
            for( int i = 0; i < 5; i++ )
            {
                client.HSet(key, fieldFmt.AsFormat( i ), i);
            }
            var rslt = client.HGetAll<int>(key);
            GetRsltCountCorrect = rslt.Count == 5;
            GetRsltFieldsCorrect = true;
            GetRsltValsCorrect = true;
            for (int i = 0; i < 5; i++)
            {
                if (GetRsltFieldsCorrect)
                    GetRsltFieldsCorrect = rslt.ContainsKey(fieldFmt.AsFormat(i));
                if (GetRsltFieldsCorrect && GetRsltValsCorrect)
                    GetRsltValsCorrect = rslt[fieldFmt.AsFormat(i)] == i;
            }

        };

        private It should_get_the_correct_count_of_fields = () => GetRsltCountCorrect.ShouldBeTrue();
        private It should_get_the_correct_field_names = () => GetRsltFieldsCorrect.ShouldBeTrue();
        private It should_get_the_correct_field_values = () => GetRsltValsCorrect.ShouldBeTrue();

    }
}
