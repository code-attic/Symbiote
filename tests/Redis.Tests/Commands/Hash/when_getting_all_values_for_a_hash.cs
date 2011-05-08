using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.Hash
{
    public class when_getting_all_values_for_a_hash :
        with_clean_db
    {
        protected static bool GetEmptyForNonexistantHash;
        protected static bool GotAllVals;

        private Because of = () =>
        {
            var key = "Test Hash Key Vals";
            var field = "Test Hash Field Vals {0}";
            var keyDict = new Dictionary<int, bool>();
            var rslt = client.HVals<int>(key);
            GetEmptyForNonexistantHash = rslt.Count() == 0;
            for (int i = 0; i < 5; i++)
            {
                client.HSet(key, field.AsFormat(i), i);
                keyDict.Add(i, false);
            }
            rslt = client.HVals<int>(key);
            var keyCt = 0;

            rslt.ForEach(v => keyDict[v] = true);
            GotAllVals = !keyDict.ContainsValue(false);
        };

        private It should_get_empty_list_for_nonexistant_hash = () => GetEmptyForNonexistantHash.ShouldBeTrue();
        private It should_get_all_values_in_the_hash = () => GotAllVals.ShouldBeTrue();

    }
}
