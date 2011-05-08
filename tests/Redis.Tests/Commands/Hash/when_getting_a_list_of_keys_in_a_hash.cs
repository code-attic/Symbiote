using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.Hash
{
    public class when_getting_a_list_of_keys_in_a_hash :
        with_clean_db
    {
        protected static bool GetEmptyForNonexistantHash;
        protected static bool GotAllKeys;

        private Because of = () =>
        {
            var key = "Test Hash Key Keys";
            var field = "Test Hash Field Keys {0}";
            var keyDict = new Dictionary<string, bool>();
            var rslt = client.HKeys(key);
            GetEmptyForNonexistantHash = rslt.Count() == 0;
            for( int i = 0; i < 5; i++ )
            {
                client.HSet(key, field.AsFormat(i), i);
                keyDict.Add(field.AsFormat( i ),false);
            }
            rslt = client.HKeys(key);
            var keyCt = 0;

            rslt.ForEach(v => keyDict[v] = true ) ;
            GotAllKeys = !keyDict.ContainsValue(false);
        };

        private It should_get_empty_list_for_nonexistant_hash = () => GetEmptyForNonexistantHash.ShouldBeTrue();
        private It should_get_all_keys_in_the_hash = () => GotAllKeys.ShouldBeTrue();

    }
}
