using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.Hash
{
    public class when_setting_many_values_at_once :
        with_clean_db
    {
        protected static bool SetValsCorrectly;

        private Because of = () =>
        {
            var key = "Test Hash Key Len";
            var field = "Test Hash Field Len {0}";
            var valDict = new Dictionary<string, int>();
            for (int i = 0; i < 5; i++)
            {
                valDict.Add(field.AsFormat( i ), i  );
            }
            client.HSet(key, valDict);

            var rslt = client.HGetAll<int>(key);
            bool isEqual = true;
            foreach (KeyValuePair<string, int> kvpExpected in valDict)
            {
                if (isEqual)
                    isEqual = rslt.Contains(kvpExpected);
            }
            SetValsCorrectly = isEqual && (rslt.Count == valDict.Count);
          
        };

        private It should_set_all_values_correctly = () => SetValsCorrectly.ShouldBeTrue();

    }
}
