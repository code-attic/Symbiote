using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Redis;

namespace Redis.Tests.Commands.Hash
{
    class when_getting_multiple_fields_in_a_hash :
        with_clean_db
    {
        protected static bool GetRsltCountCorrect;
        protected static bool GetExceptionWithInvalidVal;
        protected static bool GetRsltValsCorrect;

        private Because of = () =>
        {
            var key = "Test Hash Key Get All";
            var fieldFmt = "Test Hash Field Get All {0}";
            var fieldCt = 5;
            var fields = new string[fieldCt];
            for (int i = 0; i < fieldCt; i++)
            {
                fields[i] = fieldFmt.AsFormat(i);
                client.HSet(key, fields[i], fields[i]);
            }

            var rslt = client.HGet<string>(key,fields);
            GetRsltValsCorrect = true;
            int idx = 0;
            rslt.ForEach( v =>
                              {
                                    if (GetRsltValsCorrect)
                                    {
                                        GetRsltValsCorrect = v == fields[idx];
                                    }
                                    idx++;
                              });
            GetRsltCountCorrect = idx == fieldCt;

            //If you include a field that doesn't exist then we should get an exception
            GetExceptionWithInvalidVal = false;
            client.HDel(key, fields[0]);
            try
            {
                rslt = client.HGet<string>(key, fields);
            }
            catch ( ValueNotInDatabaseException ex)
            {

                GetExceptionWithInvalidVal = true;
            }
        };

        private It should_get_the_correct_count_of_fields = () => GetRsltCountCorrect.ShouldBeTrue();
        private It should_get_the_correct_field_values_for_valid_fields = () => GetRsltValsCorrect.ShouldBeTrue();
        private It should_get_ValueNotInDatabsaeException_for_invalid_fields = () => GetExceptionWithInvalidVal.ShouldBeTrue();

    }
}
